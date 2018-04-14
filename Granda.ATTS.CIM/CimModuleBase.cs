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
using Granda.AATS.Log;
using Granda.ATTS.CIM.Data.ENUM;
using Granda.ATTS.CIM.Data.Message;
using Granda.ATTS.CIM.Data.Report;
using Granda.ATTS.CIM.Extension;
using Granda.ATTS.CIM.Model;
using Granda.ATTS.CIM.Scenario;
using Secs4Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using static Granda.ATTS.CIM.Extension.ExtensionHelper;
namespace Granda.ATTS.CIM
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
        IInitializeScenario,
        IRCSCallBack,
        IAMSCallBack,
        IClock,
        IEqtTerminalService,
        IRecipeManagement,
        IDataCollection
    {
        #region 属性
        /// <summary>
        /// 设备ID
        /// </summary>
        private static short DeviceId { get; set; } = 1;
        private static SecsGem secsGemService = null;
        /// <summary>
        /// 场景控制器集合
        /// </summary>
        protected readonly Dictionary<Scenarios, IScenario> scenarioControllers = new Dictionary<Scenarios, IScenario>();
        #endregion

        #region 公开的事件
        /// <summary>
        /// 设备控制状态变化事件
        /// </summary>
        public event EventHandler<TEventArgs<EquipmentStatus>> ControlStateChanged;
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
        public event EventHandler<TEventArgs<RemoteControlCommandRequest>> OnRemoteControlCommandRequest;
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
        /// <summary>
        /// 连接状态变化事件
        /// </summary>
        public event EventHandler<TEventArgs<ConnectionState>> ConnectionChanged;
        /// <summary>
        /// 发生错误事件, 此为静态事件， 通过类名直接注册
        /// </summary>
        public static event EventHandler<TEventArgs<Exception>> ErrorOccured;
        /// <summary>
        /// Trace Data Initialization Request event
        /// </summary>
        public event EventHandler<TEventArgs<TraceDataInitializationRequest>> OnTraceDataInitializationRequest;
        /// <summary>
        /// Formatted Status Request event
        /// </summary>
        public event EventHandler<TEventArgs<SFCD>> OnFormattedStatusRequest;

        /// <summary>
        /// Equipment Constants Request event
        /// </summary>
        public event EventHandler<TEventArgs<string[]>> OnEquipmentConstantsRequest;
        /// <summary>
        /// Enable Disable event report event
        /// </summary>
        public event EventHandler<TEventArgs<string[]>> OnEnableDisableEventReportRequest;
        /// <summary>
        /// Selected Equipment Status Request event
        /// </summary>
        public event EventHandler<TEventArgs<string[]>> OnSelectedEquipmentStatusRequest;
        /// <summary>
        /// Formatted Process Program Request event
        /// </summary>
        public event EventHandler<TEventArgs<FormattedProcessProgramRequest>> OnFormattedProcessProgramRequest;
        /// <summary>
        /// Current EPPD Request event 
        /// </summary>
        public event EventHandler<TEventArgs<CurrentEPPDRequest>> OnCurrentEPPDRequest;
        /// <summary>
        /// Current Alarm List Request
        /// </summary>
        public event EventHandler<TEventArgs<CurrentAlarmListRequest>> OnCurrentAlarmListRequest;
        /// <summary>
        /// Alarm Enable Disable Request Event
        /// </summary>
        public event EventHandler<TEventArgs<AlarmEnableDisableRequest>> OnAlarmEnableDisableRequest;
        #endregion

        #region 构造方法
        /// <summary>
        /// 默认构造方法
        /// </summary>
        public CimModuleBase()
        {
            Thread.CurrentThread.Name = "Main";
            LogAdapter.WriteLog(new LogRecord(LogLevel.INFO, "Initialize CIM Module."));

        }
        /// <summary>
        /// 构造方法， 提供SecsGem参数
        /// </summary>
        /// <param name="secsGem"></param>
        /// <param name="deviceId">设备Id号， 默认为1</param>
        public CimModuleBase(SecsGem secsGem, short deviceId) : this()
        {
            secsGemService = secsGem;
            secsGemService.Tracer = new MyTracer();
            secsGemService.ConnectionChanged += SecsGemService_ConnectionChanged;
            secsGemService.PrimaryMessageRecived += SecsGemService_PrimaryMessageRecived;
            DeviceId = deviceId;
        }
        /// <summary>
        /// 构造方法，提供创建SecsGem所需参数
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="isActive"></param>
        /// <param name="deviceId">设备Id号， 默认为1</param>
        public CimModuleBase(string ipAddress, int port, bool isActive, short deviceId) : this()
        {
            secsGemService = new SecsGem(IPAddress.Parse(ipAddress), port, isActive);
            secsGemService.Tracer = new MyTracer();
            secsGemService.ConnectionChanged += SecsGemService_ConnectionChanged;
            secsGemService.PrimaryMessageRecived += SecsGemService_PrimaryMessageRecived;
            DeviceId = deviceId;
        }
        /// <summary>
        /// 场景初始化
        /// <para>接口初始化和事件初始化不可同时使用，即如果初始化了相应场景的接口之后，对应场景下所有的事件将不会再被响应</para>
        /// </summary>
        /// <param name="itializeScenario">initialize场景回调方法接口</param>
        /// <param name="rCSCallBack">Remote Control场景回调方法接口</param>
        /// <param name="aMSCallBack">Alart Management场景回调方法接口</param>
        /// <param name="clock">Clock场景回调方法接口</param>
        /// <param name="eqtTerminalService">Equipment Terminal Service场景回调方法接口</param>
        /// <param name="recipeManagement">Prcess Program(Recipe) Management场景回调方法接口</param>
        /// <param name="dataCollection">Data Collection场景回调方法接口</param>
        public void ScenarioInitialize(
            IInitializeScenario itializeScenario = null,
            IRCSCallBack rCSCallBack = null,
            IAMSCallBack aMSCallBack = null,
            IClock clock = null,
            IEqtTerminalService eqtTerminalService = null,
            IRecipeManagement recipeManagement = null,
            IDataCollection dataCollection = null)
        {
            scenarioControllers.Add(Scenarios.Intialize_Scenario, new InitializeScenario(itializeScenario ?? this));
            scenarioControllers.Add(Scenarios.Remote_Control, new RemoteControl(rCSCallBack ?? this));
            scenarioControllers.Add(Scenarios.Alarm_Management, new AlarmManagement(aMSCallBack ?? this));
            scenarioControllers.Add(Scenarios.Clock, new Clock(clock ?? this));
            scenarioControllers.Add(Scenarios.Equipment_Terminal_Service, new EqtTerminalService(eqtTerminalService ?? this));
            scenarioControllers.Add(Scenarios.Recipe_Management, new RecipeManagement(recipeManagement ?? this));
            scenarioControllers.Add(Scenarios.Data_Collection, new DataCollection(dataCollection ?? this));
        }
        #endregion

        #region 事件响应方法
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
        #endregion

        #region 静态方法
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
            try
            {
                return secsGemService.SendMessage(DeviceId, s, f, false, systemBytes, item, key, value);
            }
            catch (Exception ex)
            {
                ErrorOccured?.Invoke(secsGemService, new TEventArgs<Exception>(ex));
                return null;
            }
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
            try
            {
                return secsGemService.SendMessage(DeviceId, s, f, replyExpected, -1, item, key, value);
            }
            catch (Exception ex)
            {
                ErrorOccured?.Invoke(secsGemService, new TEventArgs<Exception>(ex));
                return null;
            }

        }
        #endregion

        #region 接口方法，触发事件，无需调用
        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        public void UpdateControlState(EquipmentStatus controlState)
        {
            ControlStateChanged?.Invoke(this, new TEventArgs<EquipmentStatus>(controlState));
        }

        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        public void UpdateDateTime(string dateTimeStr)
        {
            DateTimeUpdate?.Invoke(this, new TEventArgs<string>(dateTimeStr));
        }

        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        public void RemoteControlCommandRequestEvent(RemoteControlCommandRequest hostCommand)
        {
            OnRemoteControlCommandRequest?.Invoke(this, new TEventArgs<RemoteControlCommandRequest>(hostCommand));
        }

        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        public void UpdateAlarmStatus(bool isEnable)
        {
            AlarmStatusUpdated?.Invoke(this, new TEventArgs<bool>(isEnable));
        }

        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        public void ReceiveTestMessage(string[] messages)
        {
            ReceiveDisplayMessage?.Invoke(this, new TEventArgs<string[]>(messages));
        }

        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        public void SendMessageDone(string[] messages)
        {
            SendDisplayMessageDone?.Invoke(this, new TEventArgs<string[]>(messages));
        }
        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        private void SecsGemService_ConnectionChanged(object sender, TEventArgs<ConnectionState> e)
        {
            Debug.WriteLine("connection state change: " + e.Data.ToString());
            ConnectionChanged?.Invoke(this, new TEventArgs<ConnectionState>(e.Data));
        }

        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        public virtual void SelectedEquipmentStatusRequestEvent(string[] data)
        {
            OnSelectedEquipmentStatusRequest?.Invoke(this, new TEventArgs<string[]>(data));
        }

        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        public void EnableDisableEventReportEvent(string[] ceidArr)
        {
            OnEnableDisableEventReportRequest?.Invoke(this, new TEventArgs<string[]>(ceidArr));
        }
        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        /// <param name="data"></param>
        public void EquipmentConstantsRequestEvent(string[] data)
        {
            OnEquipmentConstantsRequest?.Invoke(this, new TEventArgs<string[]>(data));
        }
        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        /// <param name="sfcd"></param>
        public void FormattedStatusRequestEvent(SFCD sfcd)
        {
            OnFormattedStatusRequest?.Invoke(this, new TEventArgs<SFCD>(sfcd));
        }
        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        /// <param name="traceDataInitializationRequest"></param>
        public void TraceDataInitializationRequestEvent(TraceDataInitializationRequest traceDataInitializationRequest)
        {
            OnTraceDataInitializationRequest?.Invoke(this, new TEventArgs<TraceDataInitializationRequest>(traceDataInitializationRequest));
        }
        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        /// <param name="currentEPPDRequest"></param>
        public void CurrentEPPDRequestEvent(CurrentEPPDRequest currentEPPDRequest)
        {
            OnCurrentEPPDRequest?.Invoke(this, new TEventArgs<CurrentEPPDRequest>(currentEPPDRequest));
        }
        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        /// <param name="formattedProcessProgramRequest"></param>
        public void FormattedProcessProgramRequestEvent(FormattedProcessProgramRequest formattedProcessProgramRequest)
        {
            OnFormattedProcessProgramRequest?.Invoke(this, new TEventArgs<FormattedProcessProgramRequest>(formattedProcessProgramRequest));
        }
        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        /// <param name="alarmEnableDisableJob"></param>
        public void AlarmEnableDisableRequestEvent(AlarmEnableDisableRequest alarmEnableDisableJob)
        {
            OnAlarmEnableDisableRequest?.Invoke(this, new TEventArgs<AlarmEnableDisableRequest>(alarmEnableDisableJob));
        }

        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        /// <param name="currentAlarmListJob"></param>
        public void CurrentAlarmListRequestEvent(CurrentAlarmListRequest currentAlarmListJob)
        {
            OnCurrentAlarmListRequest?.Invoke(this, new TEventArgs<CurrentAlarmListRequest>(currentAlarmListJob));
        }
        #endregion
    }
}
