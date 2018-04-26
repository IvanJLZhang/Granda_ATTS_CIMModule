using System;
using System.Diagnostics;
using Granda.AATS.Log;
using Granda.ATTS.CIM.Data;
using Granda.ATTS.CIM.Data.ENUM;
using Granda.ATTS.CIM.Data.Report;
using Granda.ATTS.CIM.Extension;
using Granda.ATTS.CIM.Model;
using Granda.HSMS;
using static Granda.ATTS.CIM.Extension.ExtensionHelper;
using static Granda.ATTS.CIM.StreamType.Stream1_EquipmentStatus;
using static Granda.ATTS.CIM.StreamType.Stream2_EquipmentControl;
using static Granda.ATTS.CIM.StreamType.Stream6_DataCollection;

namespace Granda.ATTS.CIM.Scenario
{
    /// <summary>
    /// 初始化场景处理类
    /// </summary>
    internal class InitializeScenario : BaseScenario, IScenario
    {
        #region 构造方法及变量定义
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public InitializeScenario(IInitializeScenario callBack) : base()
        {
            ScenarioType = Scenarios.Intialize_Scenario;
            itializeScenario = callBack;
        }
        /// <summary>
        /// 保存Equipment当前基本信息
        /// </summary>
        private EquipmentBaseInfo _equipmentBaseInfo = new EquipmentBaseInfo();
        /// <summary>
        /// 保存设备当前的状态信息
        /// </summary>
        private EquipmentStatus _equipmentStatusInfo = new EquipmentStatus();

        private IInitializeScenario itializeScenario = new DefaultIItializeScenario();
        ///// <summary>
        ///// 保存Equipment当前基本信息
        ///// </summary>
        //public EquipmentBaseInfo EquipmentBaseInfo { get => _equipmentBaseInfo; set => _equipmentBaseInfo = value; }
        ///// <summary>
        ///// 保存设备当前的状态信息
        ///// </summary>
        //public EquipmentStatus EquipmentStatusInfo { get => _equipmentStatusInfo; set => _equipmentStatusInfo = value; }

        #endregion

        #region message handler methods
        /// <summary>
        /// handle online/offline request by host
        /// </summary>
        public void HandleSecsMessage(SecsMessage secsMessage)
        {
            PrimaryMessage = secsMessage;
            switch (PrimaryMessage.GetSFString())
            {
                case "S1F1":// are you there request
                    SubScenarioName = Resource.Intialize_Scenario_1;
                    secsMessage.S1F2(this._equipmentBaseInfo.MDLN ?? String.Empty, this._equipmentBaseInfo.SOFTREV ?? String.Empty);// 作为unit端， 只考虑online的选项
                    break;
                case "S1F13":// estublish communication request
                    handleS1F13();
                    break;
                case "S1F17":// request online by host
                    handleS1F17();
                    break;
                case "S1F15":// request offline by host
                    SubScenarioName = Resource.Intialize_Scenario_4;
                    switch (this._equipmentStatusInfo.CRST)
                    {
                        case CRST.O:
                            //send equipment denies requests
                            S1F0();
                            break;
                        case CRST.L:
                        case CRST.R:
                            // send OFF_LINE Acknowledge
                            // 0: Accepted, 1: Not Accepted
                            secsMessage.S1F16("0");
                            // send Control State Change(OFF_LINE)
                            launchControlStateProcess((int)CRST.O);
                            break;
                        default:
                            break;
                    }
                    break;
                case "S2F17":// Date and Time Request
                    string dataTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                    secsMessage.S2F18(dataTime);
                    break;
                case "S6F11":// Event Report Send (ERS)
                    secsMessage.S6F12("0");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// establish communication request by host
        /// </summary>
        void handleS1F13()
        {
            SubScenarioName = Resource.Intialize_Scenario_3;
            EquipmentBaseInfo message = new EquipmentBaseInfo();
            message.Parse(PrimaryMessage.SecsItem);
            //this._equipmentBaseInfo = message;
            PrimaryMessage.S1F14(_equipmentBaseInfo.MDLN ?? String.Empty, _equipmentBaseInfo.SOFTREV ?? String.Empty, "0");
        }
        /// <summary>
        /// request online by host
        /// 0: Accepted,
        /// 1: Not Accepted,
        /// 2: Already ON-LINE LOCAL,
        /// 3: Already ON-LINE REMOTE.
        /// </summary>
        void handleS1F17()
        {
            SubScenarioName = Resource.Intialize_Scenario_3;
            string ONLACK = String.Empty;
            switch (this._equipmentStatusInfo.CRST)
            {
                case CRST.O:
                    ONLACK = "0";
                    break;
                case CRST.L:
                    ONLACK = "2";
                    break;
                case CRST.R:
                    ONLACK = "3";
                    break;
                default:
                    ONLACK = "0";
                    break;
            }
            PrimaryMessage.S1F18(ONLACK);
            if (launchControlStateProcess((int)CRST.R))
            {
                if (LaunchDateTimeUpdateProcess())
                {
                    launchControlStateProcess(114);
                }
            }
        }
        #endregion

        #region Initialize Scenario methods
        /// <summary>
        /// 启动建立连接进程 online by Unit
        /// </summary>
        /// <returns></returns>
        public bool LaunchOnlineProcess(EquipmentInfo equipmentInfo)
        {
            SubScenarioName = Resource.Intialize_Scenario_1;
            this._equipmentBaseInfo = equipmentInfo.EquipmentBase;
            this._equipmentStatusInfo = equipmentInfo.EquipmentStatus;
            // send estublish communication request
            var replyMsg = S1F13(_equipmentBaseInfo.SecsItem);

            if ((replyMsg != null && replyMsg.GetSFString() == "S1F14"))
            {
                var ack = replyMsg.GetCommandValue();
                if (ack != 0)
                {
                    CIMBASE.WriteLog(LogLevel.INFO, "Host denies establish communication request.");
                    return false;
                }
            }
            if (replyMsg == null)
                return false;
            replyMsg = S1F1();
            if (replyMsg == null || replyMsg.F == 0)
            {// host denies online request
                CIMBASE.WriteLog(LogLevel.INFO, "Host denies online request.");
                return false;
            }
            CIMBASE.WriteLog(LogLevel.DEBUG, "Host grants online.");
            if (launchControlStateProcess((int)CRST.R))
            {
                if (LaunchDateTimeUpdateProcess())
                {
                    return launchControlStateProcess(114);
                }
            }
            CIMBASE.WriteLog(LogLevel.ERROR, "estublish communication with host failed.");
            return false;
        }

        /// <summary>
        /// 启动Offline进程 Offline by Unit
        /// <para>If Host reply ACK=0, Equipment turns control mode to Offline Mode,
        /// If Host reply ACK != 0 or host no response, Equipment still change to Offline</para>
        /// </summary>
        /// <returns></returns>
        public bool LaunchOfflineProcess()
        {
            SubScenarioName = Resource.Intialize_Scenario_2;
            var result = launchControlStateProcess((int)CRST.O);
            return result;
        }

        #region Host methods
        /// <summary>
        /// 启动online by host 进程
        /// </summary>
        /// <returns></returns>
        public bool LaunchOnlineByHostProcess()
        {
            SubScenarioName = Resource.Intialize_Scenario_3;
            // send estublish communication request
            var replyMsg = S1F13(this._equipmentBaseInfo.SecsItem);
            if (replyMsg != null && replyMsg.GetSFString() == "S1F14")
            {
                var ack = replyMsg.GetCommandValue();
                if (ack == 0)
                {
                    // send ON_LINE request
                    replyMsg = S1F17(CRST.R.ToString());
                    if (replyMsg != null && replyMsg.GetSFString() == "S1F18")
                    {
                        ack = replyMsg.GetCommandValue();
                        if (ack == 0)
                        {
                            this._equipmentStatusInfo.CRST = CRST.R;
                            itializeScenario?.UpdateControlState(this._equipmentStatusInfo.CRST);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 启动offline by host 进程
        /// </summary>
        /// <returns></returns>
        public bool LaunchOfflineByHostProcess()
        {
            SubScenarioName = Resource.Intialize_Scenario_4;
            // send Off-line request
            var replyMsg = S1F15();
            if (replyMsg != null && replyMsg.GetSFString() == "S1F0")
            {// equipment denies requests

            }
            else if (replyMsg.GetSFString() == "S1F16")
            {
                var ack = replyMsg.GetCommandValue();
                if (ack == 0)
                {
                    this._equipmentStatusInfo.CRST = CRST.O;
                    itializeScenario?.UpdateControlState(this._equipmentStatusInfo.CRST);
                    return true;
                }
            }
            return false;
        }
        #endregion

        #endregion

        #region 其他可公开的控制进程
        /// <summary>
        /// 设置Control State状态：
        /// CEID 113==>ONLINE REMOTE
        ///      112==>ONLINE LOCAL
        ///      111==>OFFLINE
        ///      114==>EQUIPMENT STATUS CHANGE
        /// </summary>
        /// <param name="ceid"></param>
        /// <returns></returns>
        private bool launchControlStateProcess(int ceid)
        {
            ControlStateChangeReport controlStateChangeReport = new ControlStateChangeReport()
            {
                CEID = ceid,
                EquipmentStatus = new EquipmentStatus()
                {
                    CRST = ceid == 114 ? CRST.R : (CRST)ceid,
                    EQST = this._equipmentStatusInfo.EQST,
                    EQSTCODE = this._equipmentStatusInfo.EQSTCODE,
                },
                RPTID = 100,
            };
            var replyMsg = S6F11(controlStateChangeReport.SecsItem, (int)ceid);
            switch (ceid)
            {
                case (int)CRST.R:
                case (int)CRST.L:
                    if (replyMsg != null && replyMsg.GetSFString() == "S6F12")
                    {
                        int ack = replyMsg.GetCommandValue();
                        if (ack == 0)
                        {
                            this._equipmentStatusInfo = controlStateChangeReport.EquipmentStatus;
                            itializeScenario?.UpdateControlState(this._equipmentStatusInfo.CRST);
                            return true;
                        }
                    }
                    break;
                case (int)CRST.O:// no matter what happened, send control state change event
                    this._equipmentStatusInfo = controlStateChangeReport.EquipmentStatus;
                    itializeScenario?.UpdateControlState(this._equipmentStatusInfo.CRST);
                    return true;
                case 114:
                    if (replyMsg != null && replyMsg.GetSFString() == "S6F12")
                    {
                        return replyMsg.GetCommandValue() == 0;
                    }
                    break;
                default:
                    break;
            }
            CIMBASE.WriteLog(LogLevel.ERROR, "something was wrong when sending S6F11 message.");
            return false;
        }
        /// <summary>
        /// 启动date and time更新请求
        /// </summary>
        /// <returns></returns>
        public bool LaunchDateTimeUpdateProcess()
        {
            CIMBASE.WriteLog(LogLevel.DEBUG, "send update date and time request to host.");
            var replyMsg = S2F17();
            if (replyMsg != null && replyMsg.GetSFString() == "S2F18")
            {
                var dateTimeStr = replyMsg.SecsItem.GetString();
                CIMBASE.WriteLog(LogLevel.DEBUG, "get response datetime string: " + dateTimeStr);
                itializeScenario?.UpdateDateTime(dateTimeStr);
                return true;
            }
            CIMBASE.WriteLog(LogLevel.ERROR, "something wrong happened in sending update date and time request to host");
            return false;
        }
        #endregion

        #region 接口默认实例类
        private class DefaultIItializeScenario : IInitializeScenario
        {
            public void UpdateControlState(CRST controlState)
            {
                Debug.WriteLine("Control State Changed: " + controlState.ToString());
            }
            public void UpdateDateTime(string dateTimeStr)
            {
                Debug.WriteLine("date and time update: " + dateTimeStr);
            }
        }
        #endregion
    }

    #region 接口
    /// <summary>
    /// Initialize Scenario interface
    /// </summary>
    public interface IInitializeScenario
    {
        /// <summary>
        /// 更新 Control State状态
        /// 是否需要回复：否
        /// </summary>
        /// <param name="controlState"></param>
        void UpdateControlState(CRST controlState);
        /// <summary>
        /// 更新系统时间
        /// 是否需要回复：否
        /// </summary>
        /// <param name="dateTimeStr"></param>
        void UpdateDateTime(string dateTimeStr);
    }
    #endregion
}
