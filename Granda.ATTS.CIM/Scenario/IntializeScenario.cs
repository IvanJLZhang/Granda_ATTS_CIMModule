using Secs4Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Granda.ATTS.CIM.Extension;
using Granda.ATTS.CIM.Model;
using static Granda.ATTS.CIM.Extension.ExtensionHelper;
using static Granda.ATTS.CIM.Extension.SmlExtension;
using static Granda.ATTS.CIM.StreamType.Stream1_EquipmentStatus;
using static Granda.ATTS.CIM.StreamType.Stream2_EquipmentControl;
using static Granda.ATTS.CIM.StreamType.Stream6_DataCollection;
using static Secs4Net.Item;
using Granda.ATTS.CIM.Data;
using Granda.ATTS.CIM.Data.ENUM;
using Granda.ATTS.CIM.Data.Report;

namespace Granda.ATTS.CIM.Scenario
{
    /// <summary>
    /// 初始化场景处理类
    /// </summary>
    internal class InitializeScenario : BaseScenario, IScenario
    {
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
                    secsMessage.S1F2(this._equipmentBaseInfo.MDLN, this._equipmentBaseInfo.SOFTREV);// 作为unit端， 只考虑online的选项
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
                            secsMessage.S1F16("0");
                            // send Control State Change(OFF_LINE)
                            launchControlStateProcess((int)CRST.O, this._equipmentStatusInfo);
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
        #region Initialize Scenario: 
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

            if (!(replyMsg != null && replyMsg.GetSFString() == "S1F14"))
            {
                return false;
            }

            replyMsg = S1F1();
            if (replyMsg == null || replyMsg.F == 0)
            {// host denies online request
                return false;
            }
            if (launchControlStateProcess((int)CRST.R, this._equipmentStatusInfo))
            {
                if (LaunchDateTimeUpdateProcess())
                {
                    return launchControlStateProcess((int)CRST.EQT_STATUS_CHANGE, this._equipmentStatusInfo);
                }
            }
            return false;
        }

        /// <summary>
        /// 启动Offline进程 Offline by Unit
        /// </summary>
        /// <returns></returns>
        public bool LaunchOfflineProcess()
        {
            SubScenarioName = Resource.Intialize_Scenario_2;
            var result = launchControlStateProcess((int)CRST.O, this._equipmentStatusInfo);
            return result;
        }
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
                            itializeScenario?.UpdateControlState(this._equipmentStatusInfo);
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
                    itializeScenario?.UpdateControlState(this._equipmentStatusInfo);
                    return true;
                }
            }
            return false;
        }
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
        /// <param name="equipmentStatus"></param>
        /// <returns></returns>
        private bool launchControlStateProcess(int ceid, EquipmentStatus equipmentStatus)
        {
            if (ceid >= 111 && ceid <= 114)
            {// control state change
                EquipmentStatus newEquipmentStatus = new EquipmentStatus()
                {
                    CRST = equipmentStatus.CRST,
                    EQST = equipmentStatus.EQST,
                    EQSTCODE = equipmentStatus.EQSTCODE,
                };
                if (ceid == 114)
                {
                    newEquipmentStatus.CRST = CRST.R;
                }
                else
                {
                    newEquipmentStatus.CRST = (CRST)ceid;
                }
                ControlStateChangeReport controlStateChangeReport = new ControlStateChangeReport()
                {
                    CEID = ceid,
                    EquipmentStatus = newEquipmentStatus,
                    DATAID = 0,
                    RPTID = 100,
                };
                var replyMsg = S6F11(controlStateChangeReport.SecsItem, (int)ceid);
                if (replyMsg != null && replyMsg.GetSFString() == "S6F12")
                {
                    try
                    {
                        int ack = replyMsg.GetCommandValue();
                        if (ack == 0)
                        {
                            if (ceid != 114)
                            {
                                this._equipmentStatusInfo = newEquipmentStatus;
                                itializeScenario?.UpdateControlState(this._equipmentStatusInfo);
                            }

                            return true;
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        return false;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 启动date and time更新请求
        /// </summary>
        /// <returns></returns>
        public bool LaunchDateTimeUpdateProcess()
        {
            var replyMsg = S2F17();
            if (replyMsg != null && replyMsg.GetSFString() == "S2F18")
            {
                var dateTimeStr = replyMsg.SecsItem.GetString();
                itializeScenario?.UpdateDateTime(dateTimeStr);
                return true;
            }
            return false;
        }
        #endregion

        #region message handler methods
        void handleS1F13()
        {
            SubScenarioName = Resource.Intialize_Scenario_3;
            EquipmentBaseInfo message = new EquipmentBaseInfo();
            message.Parse(PrimaryMessage.SecsItem);
            this._equipmentBaseInfo = message;
            PrimaryMessage.S1F14(message.MDLN, message.SOFTREV, "0");
        }

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
                case CRST.EQT_STATUS_CHANGE:
                default:
                    break;
            }
            PrimaryMessage.S1F18(ONLACK);
            if (launchControlStateProcess((int)CRST.R, this._equipmentStatusInfo))
            {
                if (LaunchDateTimeUpdateProcess())
                {
                    launchControlStateProcess((int)CRST.EQT_STATUS_CHANGE, this._equipmentStatusInfo);
                }
            }
        }
        #endregion



        private class DefaultIItializeScenario : IInitializeScenario
        {
            public void UpdateControlState(EquipmentStatus controlState)
            {
                Debug.WriteLine("Control State Changed: " + controlState.CRST.ToString());
            }
            public void UpdateDateTime(string dateTimeStr)
            {
                Debug.WriteLine("date and time update: " + dateTimeStr);
            }
        }

    }
    #region 接口
    /// <summary>
    /// Initialize Scenario interface
    /// </summary>
    public interface IInitializeScenario
    {
        /// <summary>
        /// 更新 Control State状态
        /// </summary>
        /// <param name="controlState"></param>
        void UpdateControlState(EquipmentStatus controlState);
        /// <summary>
        /// 更新系统时间
        /// </summary>
        /// <param name="dateTimeStr"></param>
        void UpdateDateTime(string dateTimeStr);
    }
    #endregion
}
