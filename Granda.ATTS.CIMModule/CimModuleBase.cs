#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: CimModuleBase
// Author: Ivan JL Zhang    Date: 2018/4/4 10:36:38    Version: 1.0.0
// Description: 
//   用于处理CIM模块消息的基类
// 
// Revision History: 
// <Author>  		<Date>     	 	<Revision>  		<Modification>
// 	
//----------------------------------------------------------------------------*/
#endregion
using Granda.ATTS.CIMModule.Extension;
using Granda.ATTS.CIMModule.Model;
using Granda.ATTS.CIMModule.Scenario;
using Secs4Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using static Granda.ATTS.CIMModule.Extension.ExtensionHelper;
using static Granda.ATTS.CIMModule.Scenario.AlarmManagement;
using static Granda.ATTS.CIMModule.Scenario.Clock;
using static Granda.ATTS.CIMModule.Scenario.InitializeScenario;
using static Granda.ATTS.CIMModule.Scenario.RemoteControl;

namespace Granda.ATTS.CIMModule
{
    /// <summary>
    /// CIM模块运行主入口
    /// </summary>
    public class CimModuleBase : IItializeScenario, IRCSCallBack, IAMSCallBack, IClock
    {
        public static short DeviceId { get; set; } = 1;
        private static SecsGem secsGemService = null;
        protected readonly Dictionary<Scenarios, IScenario> scenarioControllers = new Dictionary<Scenarios, IScenario>();
        public CimModuleBase(SecsGem secsGem)
        {
            scenarioControllers.Add(Scenarios.Intialize_Scenario, new InitializeScenario(this));
            scenarioControllers.Add(Scenarios.Remote_Control, new RemoteControl(this));
            scenarioControllers.Add(Scenarios.Alarm_Management, new AlarmManagement(this));
            scenarioControllers.Add(Scenarios.Clock, new Clock(this));
            secsGemService = secsGem;
            secsGemService.Tracer = new MyTracer();
            secsGemService.ConnectionChanged += SecsGemService_ConnectionChanged;
            secsGemService.PrimaryMessageRecived += SecsGemService_PrimaryMessageRecived;
        }

        public CimModuleBase(string ipAddress, int port, bool isActive)
        {
            secsGemService = new SecsGem(IPAddress.Parse(ipAddress), port, isActive);
            secsGemService.Tracer = new MyTracer();
            secsGemService.ConnectionChanged += SecsGemService_ConnectionChanged;
            secsGemService.PrimaryMessageRecived += SecsGemService_PrimaryMessageRecived;
        }
        #region 公开的事件
        public event EventHandler<TEventArgs<ControlState>> ControlStateChanged;
        public event EventHandler<TEventArgs<string>> DateTimeUpdate;
        public event EventHandler<TEventArgs<HostCommand>> ProcessStateReport;
        public event EventHandler<TEventArgs<bool>> AlarmStatusUpdated;
        #endregion
        private void SecsGemService_PrimaryMessageRecived(object sender, TEventArgs<SecsMessage> e)
        {
            var message = e.Data;
            string sf = message.GetSFString();
            switch (sf)
            {
                case "S1F1":
                case "S1F13":
                case "S1F17":
                case "S1F15":
                case "S2F17":// equipment requests host time
                case "S6F11":
                    scenarioControllers[Scenarios.Intialize_Scenario].HandleSecsMessage(message);
                    break;
                case "S2F41":
                    scenarioControllers[Scenarios.Remote_Control].HandleSecsMessage(message);
                    break;
                case "S5F3":
                case "S5F103":
                    scenarioControllers[Scenarios.Alarm_Management].HandleSecsMessage(message);
                    break;
                case "S2F31":
                    scenarioControllers[Scenarios.Clock].HandleSecsMessage(message);
                    break;
                default:
                    break;
            }
        }

        private void SecsGemService_ConnectionChanged(object sender, TEventArgs<ConnectionState> e)
        {
            Debug.WriteLine("connection state change: " + e.Data.ToString());
        }
        /// <summary>
        /// 发送secondary message
        /// </summary>
        /// <param name="s"></param>
        /// <param name="f"></param>
        /// <param name="systemBytes"></param>
        /// <param name="item"></param>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static SecsMessage SendMessage(byte s, byte f, int systemBytes, Item item = null, int value = 0, string key = "")
        {
            return secsGemService.SendMessage(DeviceId, s, f, false, systemBytes, item, key, value);
        }
        /// <summary>
        /// 发送primary message
        /// </summary>
        /// <param name="s"></param>
        /// <param name="f"></param>
        /// <param name="replyExpected"></param>
        /// <param name="item"></param>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static SecsMessage SendMessage(byte s, byte f, bool replyExpected, Item item = null, int value = 0, string key = "")
        {
            return secsGemService.SendMessage(DeviceId, s, f, replyExpected, -1, item, key, value);
        }


        public void UpdateControlState(ControlState controlState)
        {
            ControlStateChanged?.Invoke(this, new TEventArgs<ControlState>(controlState));
        }

        public void UpdateDateTime(string dateTimeStr)
        {
            DateTimeUpdate?.Invoke(this, new TEventArgs<string>(dateTimeStr));
        }

        public void UpdateProcessReportState(HostCommand hostCommand)
        {
            ProcessStateReport?.Invoke(this, new TEventArgs<HostCommand>(hostCommand));
        }

        public void UpdateAlarmStatus(bool isEnable)
        {
            AlarmStatusUpdated?.Invoke(this, new TEventArgs<bool>(isEnable));
        }
    }
}
