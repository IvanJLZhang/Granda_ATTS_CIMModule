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
using static Granda.ATTS.CIMModule.Scenario.EqtTerminalService;
using static Granda.ATTS.CIMModule.Scenario.InitializeScenario;
using static Granda.ATTS.CIMModule.Scenario.RemoteControl;
using static Granda.ATTS.CIMModule.Scenario.RecipeManagement;
using static Granda.ATTS.CIMModule.Scenario.DataCollection;

namespace Granda.ATTS.CIMModule
{
    /// <summary>
    /// CIM模块运行主入口的基类
    /// 实现在：
    /// <para>Initialize Scenario,</para>
    /// <para>Remote Control,</para>
    /// <para>Alarm Management,</para>
    /// <para>Clock,</para>
    /// <para>Equipment Terminal Service,</para>
    /// <para>Process Program(Recipe) Management,</para>
    /// <para>Data Collection</para>
    /// 等场景下primary message的处理
    /// </summary>
    public class CimModuleBase :
        IItializeScenario,
        IRCSCallBack,
        IAMSCallBack,
        IClock,
        IEqtTerminalService,
        IRecipeManagement,
        IDataCollection
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public static short DeviceId { get; set; } = 1;
        private static SecsGem secsGemService = null;
        /// <summary>
        /// 场景控制器集合
        /// </summary>
        protected readonly Dictionary<Scenarios, IScenario> scenarioControllers = new Dictionary<Scenarios, IScenario>();
        /// <summary>
        /// 默认构造方法
        /// </summary>
        public CimModuleBase()
        {
            scenarioControllers.Add(Scenarios.Intialize_Scenario, new InitializeScenario(this));
            scenarioControllers.Add(Scenarios.Remote_Control, new RemoteControl(this));
            scenarioControllers.Add(Scenarios.Alarm_Management, new AlarmManagement(this));
            scenarioControllers.Add(Scenarios.Clock, new Clock(this));
            scenarioControllers.Add(Scenarios.Equipment_Terminal_Service, new EqtTerminalService(this));
            scenarioControllers.Add(Scenarios.Recipe_Management, new RecipeManagement(this));
            scenarioControllers.Add(Scenarios.Data_Collection, new DataCollection(this));
        }
        /// <summary>
        /// 构造方法， 提供SecsGem参数
        /// </summary>
        /// <param name="secsGem"></param>
        public CimModuleBase(SecsGem secsGem) : this()
        {
            secsGemService = secsGem;
            secsGemService.Tracer = new MyTracer();
            secsGemService.ConnectionChanged += SecsGemService_ConnectionChanged;
            secsGemService.PrimaryMessageRecived += SecsGemService_PrimaryMessageRecived;
        }
        /// <summary>
        /// 构造方法，提供创建SecsGem所需参数
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="isActive"></param>
        public CimModuleBase(string ipAddress, int port, bool isActive) : this()
        {
            secsGemService = new SecsGem(IPAddress.Parse(ipAddress), port, isActive);
            secsGemService.Tracer = new MyTracer();
            secsGemService.ConnectionChanged += SecsGemService_ConnectionChanged;
            secsGemService.PrimaryMessageRecived += SecsGemService_PrimaryMessageRecived;
        }
        #region 公开的事件
        /// <summary>
        /// 设备控制状态变化事件
        /// ControlState:         
        /// OFFLINE = 111，
        /// ONLINE_LOCAL = 112，
        /// ONLINE_REMOTE = 113，
        /// EQT_STATUS_CHANGE = 114
        /// </summary>
        public event EventHandler<TEventArgs<ControlState>> ControlStateChanged;
        /// <summary>
        /// Date and Time 更新事件
        /// </summary>
        public event EventHandler<TEventArgs<string>> DateTimeUpdate;
        /// <summary>
        /// Remote Control Scen下Host所发送的命令
        /// HostCommand:
        /// START,
        /// CANCEL,
        /// ABORT,
        /// PAUSE,
        /// RESUME,
        /// OPERATOR_CALL,
        /// </summary>
        public event EventHandler<TEventArgs<HostCommand>> ProcessStateReport;
        /// <summary>
        /// Host发送Enable/Disable Alarm指令事件
        /// </summary>
        public event EventHandler<TEventArgs<bool>> AlarmStatusUpdated;
        /// <summary>
        /// 接收Equipment Terminal Service Scen下Display Message事件，
        /// （消息有每次最大十条限制）
        /// </summary>
        public event EventHandler<TEventArgs<string[]>> ReceiveDisplayMessage;
        /// <summary>
        /// 发送Equipment Terminal Service Scen下Display Message消息成功事件，
        /// （消息有每次最大十条限制）
        /// </summary>
        public event EventHandler<TEventArgs<string[]>> SendDisplayMessageDone;
        #endregion
        private void SecsGemService_PrimaryMessageRecived(object sender, TEventArgs<SecsMessage> e)
        {
            var message = e.Data;
            string sf = message.GetSFString();
            IScenario scenario = null;
            switch (sf)
            {
                case "S1F1":
                case "S1F13":
                case "S1F17":
                case "S1F15":
                case "S2F17":// equipment requests host time
                case "S6F11":
                    scenario = scenarioControllers[Scenarios.Intialize_Scenario];
                    break;
                case "S2F41":
                    scenario = scenarioControllers[Scenarios.Remote_Control];
                    break;
                case "S5F3":
                case "S5F103":
                    scenario = scenarioControllers[Scenarios.Alarm_Management];
                    break;
                case "S2F31":
                    scenario = scenarioControllers[Scenarios.Clock];
                    break;
                case "S10F1":
                case "S10F5":
                    scenario = scenarioControllers[Scenarios.Equipment_Terminal_Service];
                    break;
                case "S7F19":
                case "S7F25":
                    scenario = scenarioControllers[Scenarios.Recipe_Management];
                    break;
                case "S6F3":
                case "S2F23":
                case "S6F1":
                case "S1F3":
                case "S1F5":
                case "S2F13":
                case "S2F15":
                case "S2F37":
                    scenario = scenarioControllers[Scenarios.Data_Collection];
                    break;
                default:
                    break;
            }
            scenario?.HandleSecsMessage(message);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlState"></param>
        public void UpdateControlState(ControlState controlState)
        {
            ControlStateChanged?.Invoke(this, new TEventArgs<ControlState>(controlState));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTimeStr"></param>
        public void UpdateDateTime(string dateTimeStr)
        {
            DateTimeUpdate?.Invoke(this, new TEventArgs<string>(dateTimeStr));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostCommand"></param>
        public void UpdateProcessReportState(HostCommand hostCommand)
        {
            ProcessStateReport?.Invoke(this, new TEventArgs<HostCommand>(hostCommand));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isEnable"></param>
        public void UpdateAlarmStatus(bool isEnable)
        {
            AlarmStatusUpdated?.Invoke(this, new TEventArgs<bool>(isEnable));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        public void ReceiveTestMessage(string[] messages)
        {
            ReceiveDisplayMessage?.Invoke(this, new TEventArgs<string[]>(messages));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        public void SendMessageDone(string[] messages)
        {
            SendDisplayMessageDone?.Invoke(this, new TEventArgs<string[]>(messages));
        }
    }
}
