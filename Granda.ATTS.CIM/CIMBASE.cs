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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Granda.AATS.Log;
using Granda.ATTS.CIM.Data.ENUM;
using Granda.ATTS.CIM.Data.Message;
using Granda.ATTS.CIM.Extension;
using Granda.ATTS.CIM.Model;
using Granda.ATTS.CIM.Scenario;
using Granda.ATTS.CIM.StreamType;
using Granda.HSMS;
using Granda.HSMS.Sml;
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
    public class CIMBASE :
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
        public event EventHandler<CIMEventArgs<CRST>> ControlStateChanged;
        /// <summary>
        /// Date and Time 更新事件
        /// </summary>
        public event EventHandler<CIMEventArgs<string>> DateTimeUpdate;
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
        public event EventHandler<CIMEventArgs<RemoteControlCommandRequest>> OnRemoteControlCommandRequest;
        /// <summary>
        /// 接收Equipment Terminal Service Scen下Display Message事件，
        /// （消息有每次最大十条限制）
        /// </summary>
        public event EventHandler<CIMEventArgs<string[]>> ReceiveDisplayMessage;
        /// <summary>
        /// 发送Equipment Terminal Service Scen下Display Message消息成功事件，
        /// （消息有每次最大十条限制）
        /// </summary>
        public event EventHandler<CIMEventArgs<string[]>> SendDisplayMessageDone;
        /// <summary>
        /// 连接状态变化事件
        /// </summary>
        public event EventHandler<CIMEventArgs<ConnectionStatus>> ConnectionChanged;
        /// <summary>
        /// Trace Data Initialization Request event
        /// </summary>
        public event EventHandler<CIMEventArgs<TraceDataInitializationRequest>> OnTraceDataInitializationRequest;
        /// <summary>
        /// Formatted Status Request event
        /// </summary>
        public event EventHandler<CIMEventArgs<SFCD>> OnFormattedStatusRequest;

        /// <summary>
        /// Equipment Constants Request event
        /// </summary>
        public event EventHandler<CIMEventArgs<string[]>> OnEquipmentConstantsRequest;
        /// <summary>
        /// Enable Disable event report event
        /// </summary>
        public event EventHandler<CIMEventArgs<string[]>> OnEnableDisableEventReportRequest;
        /// <summary>
        /// Selected Equipment Status Request event
        /// </summary>
        public event EventHandler<CIMEventArgs<string[]>> OnSelectedEquipmentStatusRequest;
        /// <summary>
        /// Formatted Process Program Request event
        /// </summary>
        public event EventHandler<CIMEventArgs<FormattedProcessProgramRequest>> OnFormattedProcessProgramRequest;
        /// <summary>
        /// Current EPPD Request event 
        /// </summary>
        public event EventHandler<CIMEventArgs<CurrentEPPDRequest>> OnCurrentEPPDRequest;
        /// <summary>
        /// Current Alarm List Request
        /// </summary>
        public event EventHandler<CIMEventArgs<CurrentAlarmListRequest>> OnCurrentAlarmListRequest;
        /// <summary>
        /// Alarm Enable Disable Request Event
        /// </summary>
        public event EventHandler<CIMEventArgs<AlarmEnableDisableRequest>> OnAlarmEnableDisableRequest;
        #endregion

        #region 构造方法
        /// <summary>
        /// 默认构造方法
        /// </summary>
        public CIMBASE()
        {
            //Thread.CurrentThread.Name = "Main";
            LogAdapter.WriteLog(new LogRecord(LogLevel.INFO, "Initialize CIM Module."));

        }
        /// <summary>
        /// 构造方法， 提供SecsGem参数
        /// </summary>
        /// <param name="secsGem"></param>
        /// <param name="deviceId">设备Id号， 默认为1</param>
        public CIMBASE(SecsGem secsGem, short deviceId) : this()
        {
            secsGemService = secsGem;
            secsGemService.ConnectionChanged += SecsGemService_ConnectionChanged;
            secsGemService.PrimaryMessageReceived += SecsGemService_PrimaryMessageReceived;
            DeviceId = deviceId;
        }
        /// <summary>
        /// 构造方法，提供创建SecsGem所需参数
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="isActive"></param>
        /// <param name="deviceId">设备Id号， 默认为1</param>
        public CIMBASE(string ipAddress, int port, bool isActive, short deviceId) : this()
        {
            secsGemService = new SecsGem(isActive, IPAddress.Parse(ipAddress), port);
            secsGemService.ConnectionChanged += SecsGemService_ConnectionChanged;
            secsGemService.PrimaryMessageReceived += SecsGemService_PrimaryMessageReceived;
            DeviceId = deviceId;
        }

        private void SecsGemService_PrimaryMessageReceived(object sender, PrimaryMessageWrapper e)
        {
            var message = e.Message;
            string sf = message.GetSFString();
            IScenario scenario = null;
            LogAdapter.WriteLog(new LogRecord(LogLevel.INFO, "receive primary message\r\n" + message.ToSml()));
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
                    CheckSF(e);
                    break;
            }
            message.SystenBytes = e.MessageId;
            if (scenario != null)
            {
                var ret = scenario.HandleSecsMessage(message);
                if (!ret)
                {// Illegal Data
                    message.S9F7(Item.B(e.Header.EncodeTo(new byte[10])));
                }
            }
        }


        private void CheckSF(PrimaryMessageWrapper primaryMessage)
        {
            var secsMessage = primaryMessage.Message;
            var item = Item.B(primaryMessage.Header.EncodeTo(new byte[10]));
            switch (secsMessage.S)
            {
                case 1:
                case 2:
                case 5:
                case 6:
                case 7:
                case 9:
                case 10:// Unrecognized Function Type
                    secsMessage.S9F5(item);
                    break;
                default:// Unrecognized Stream Type
                    secsMessage.S9F3(item);
                    break;
            }
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
            secsGemService.Start();
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
                var result = secsGemService.SendMessage(DeviceId, s, f, false, systemBytes, item, key, value);
                result.Wait();
                return result.Result;
            }
            catch (Exception ex)
            {
                WriteLog(LogLevel.ERROR, "Send Secondary Message error.", ex);
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
                var result = secsGemService.SendMessage(DeviceId, s, f, replyExpected, -1, item, key, value);
                result.Wait();
                if (result.Result != null)
                    WriteLog(LogLevel.INFO, "Receive Secondary message: \r\n" + result.Result.ToSml());
                return result.Result;
            }
            catch (Exception ex)
            {
                WriteLog(LogLevel.ERROR, "Send primary Message error.", ex);
                return null;
            }

        }
        /// <summary>
        /// 写入Log信息
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="logMessage"></param>
        /// <param name="ex"></param>
        public static void WriteLog(LogLevel logLevel, string logMessage, Exception ex = null)
        {
            LogAdapter.WriteLog(new LogRecord(logLevel, logMessage, ex));
        }
        #endregion

        #region 接口方法，触发事件，无需调用
        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        public void UpdateControlState(CRST controlState)
        {
            ControlStateChanged?.Invoke(this, new CIMEventArgs<CRST>(controlState, false));
        }

        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        public void UpdateDateTime(string dateTimeStr)
        {
            DateTimeUpdate?.Invoke(this, new CIMEventArgs<string>(dateTimeStr, false));
        }

        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        public void RemoteControlCommandRequestEvent(RemoteControlCommandRequest hostCommand)
        {
            OnRemoteControlCommandRequest?.Invoke(this, new CIMEventArgs<RemoteControlCommandRequest>(hostCommand));
        }

        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        public void ReceiveTestMessage(string[] messages)
        {
            ReceiveDisplayMessage?.Invoke(this, new CIMEventArgs<string[]>(messages));
        }

        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        public void SendMessageDone(string[] messages)
        {
            SendDisplayMessageDone?.Invoke(this, new CIMEventArgs<string[]>(messages));
        }
        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        private void SecsGemService_ConnectionChanged(object sender, TEventArgs<ConnectionState> e)
        {
            Debug.WriteLine("connection state change: " + e.Data.ToString());
            ConnectionChanged?.Invoke(this, new CIMEventArgs<ConnectionStatus>((ConnectionStatus)Enum.Parse(typeof(ConnectionStatus), e.Data.ToString()), false));
        }

        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        public virtual void SelectedEquipmentStatusRequestEvent(string[] data, bool needReply = true)
        {
            OnSelectedEquipmentStatusRequest?.Invoke(this, new CIMEventArgs<string[]>(data, needReply));
        }

        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        public void EnableDisableEventReportRequestEvent(string[] ceidArr)
        {
            OnEnableDisableEventReportRequest?.Invoke(this, new CIMEventArgs<string[]>(ceidArr));
        }
        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        public void EquipmentConstantsRequestEvent(string[] data, bool needReply = true)
        {
            OnEquipmentConstantsRequest?.Invoke(this, new CIMEventArgs<string[]>(data, needReply));
        }
        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        public void FormattedStatusRequestEvent(SFCD sfcd, bool needReply = true)
        {
            OnFormattedStatusRequest?.Invoke(this, new CIMEventArgs<SFCD>(sfcd, needReply));
        }
        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        public void TraceDataInitializationRequestEvent(TraceDataInitializationRequest traceDataInitializationRequest, bool needReply = true)
        {
            OnTraceDataInitializationRequest?.Invoke(this, new CIMEventArgs<TraceDataInitializationRequest>(traceDataInitializationRequest, needReply));
        }
        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        public void CurrentEPPDRequestEvent(CurrentEPPDRequest currentEPPDRequest, bool needReply = true)
        {
            OnCurrentEPPDRequest?.Invoke(this, new CIMEventArgs<CurrentEPPDRequest>(currentEPPDRequest, needReply));
        }
        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        public void FormattedProcessProgramRequestEvent(FormattedProcessProgramRequest formattedProcessProgramRequest, bool needReply = true)
        {
            OnFormattedProcessProgramRequest?.Invoke(this, new CIMEventArgs<FormattedProcessProgramRequest>(formattedProcessProgramRequest, needReply));
        }
        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        public void AlarmEnableDisableRequestEvent(AlarmEnableDisableRequest alarmEnableDisableJob)
        {
            OnAlarmEnableDisableRequest?.Invoke(this, new CIMEventArgs<AlarmEnableDisableRequest>(alarmEnableDisableJob));
        }

        /// <summary>
        /// 接口方法，触发事件，无需调用
        /// </summary>
        public void CurrentAlarmListRequestEvent(CurrentAlarmListRequest currentAlarmListJob, bool needReply = true)
        {
            OnCurrentAlarmListRequest?.Invoke(this, new CIMEventArgs<CurrentAlarmListRequest>(currentAlarmListJob, needReply));
        }
        #endregion
    }
}
