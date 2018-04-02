using Granda.ATTS.CIMModule.Model;
using Granda.ATTS.CIMModule.Scenario;
using Secs4Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Granda.ATTS.CIMModule.Extension.ExtensionHelper;
using static Granda.ATTS.CIMModule.Scenario.InitializeScenario;

namespace Granda.ATTS.CIMModule
{
    /// <summary>
    /// CIM模块运行主入口
    /// </summary>
    public class CimModuleProcess : IItializeScenario
    {
        public static short DeviceId { get; set; } = 1;
        private static SecsGem secsGemService = null;
        private readonly Dictionary<Scenarios, IScenario> scenarioControllers = new Dictionary<Scenarios, IScenario>();
        public CimModuleProcess(SecsGem secsGem)
        {
            scenarioControllers.Add(Scenarios.Intialize_Scenario, new InitializeScenario(this));

            secsGemService = secsGem;
            secsGemService.ConnectionChanged += SecsGemService_ConnectionChanged;
            secsGemService.PrimaryMessageRecived += SecsGemService_PrimaryMessageRecived;
        }

        public CimModuleProcess(string ipAddress, int port, bool isActive)
        {
            secsGemService = new SecsGem(IPAddress.Parse(ipAddress), port, isActive, 1024);
            secsGemService.ConnectionChanged += SecsGemService_ConnectionChanged;
            secsGemService.PrimaryMessageRecived += SecsGemService_PrimaryMessageRecived;
        }

        public event EventHandler<TEventArgs<ControlState>> ControlStateChanged;
        public event EventHandler<TEventArgs<string>> DateTimeUpdate;

        private void SecsGemService_PrimaryMessageRecived(object sender, TEventArgs<SecsMessage> e)
        {
            var message = e.Data;
            switch (message.GetSFString())
            {
                case "S1F13":
                case "S1F17":
                case "S1F15":
                    scenarioControllers[Scenarios.Intialize_Scenario].HandleSecsMessage(message);
                    break;
                default:
                    break;
            }
        }

        #region 公开的方法
        /// <summary>
        /// local端设置online/offline状态
        /// </summary>
        /// <param name="onLine"></param>
        /// <returns></returns>
        public bool LaunchOnOffLineProcess(bool onLine)
        {
            var initi = scenarioControllers[Scenarios.Intialize_Scenario] as InitializeScenario;
            if (onLine)
            {
                return initi.LaunchOnlineProcess();
            }
            else
            {
                return initi.LaunchOfflineProcess();
            }
        }
        #endregion

        private void SecsGemService_ConnectionChanged(object sender, TEventArgs<ConnectionState> e)
        {
            Debug.WriteLine("connection state change: " + e.Data.ToString());
        }



        public static SecsMessage SendMessage(byte s, byte f, bool replyExpected, int systemBytes, Item item = null, int ceid = 0)
        {
            if (secsGemService.State == ConnectionState.Selected)
            {
                return secsGemService.SendMessage(DeviceId, s, f, replyExpected, systemBytes, item, ceid);
            }
            return null;
        }

        public void UpdateControlState(ControlState controlState)
        {
            ControlStateChanged?.Invoke(this, new TEventArgs<ControlState>(controlState));
        }

        public void UpdateDateTime(string dateTimeStr)
        {
            DateTimeUpdate?.Invoke(this, new TEventArgs<string>(dateTimeStr));
        }
    }
}
