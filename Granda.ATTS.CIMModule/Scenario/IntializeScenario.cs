using Secs4Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Granda.ATTS.CIMModule.Extension;
using Granda.ATTS.CIMModule.Model;
using static Granda.ATTS.CIMModule.Extension.ExtensionHelper;
using static Granda.ATTS.CIMModule.Extension.SmlExtension;
using static Granda.ATTS.CIMModule.StreamType.Stream1_EquipmentStatus;
using static Granda.ATTS.CIMModule.StreamType.Stream2_EquipmentControl;
using static Granda.ATTS.CIMModule.StreamType.Stream6_DataCollection;
using static Secs4Net.Item;
using Granda.ATTS.CIMModule.Data;

namespace Granda.ATTS.CIMModule.Scenario
{
    /// <summary>
    /// 初始化场景处理类
    /// </summary>
    internal class InitializeScenario : BaseScenario, IScenario
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public InitializeScenario(IItializeScenario callBack) : base()
        {
            ScenarioType = Scenarios.Intialize_Scenario;
            itializeScenario = callBack;
        }
        public ControlState EQT_ControlState { get; private set; } = ControlState.OFFLINE;

        private IItializeScenario itializeScenario = new DefaultIItializeScenario();

        #region Initialize Scenario: 
        /// <summary>
        /// handle online/offline request by host
        /// </summary>
        public void HandleSecsMessage(SecsMessage secsMessage)
        {
            primaryMessage = secsMessage;
            switch (primaryMessage.GetSFString())
            {
                case "S1F1":// are you there request
                    SubScenarioName = Resource.Intialize_Scenario_1;
                    secsMessage.S1F2("MDLN", "SOFTREV");// 作为unit端， 只考虑online的选项
                    break;
                case "S1F13":// estublish communication request
                    SubScenarioName = Resource.Intialize_Scenario_3;
                    secsMessage.S1F14("MDLN", "SOFTREV", "0");
                    break;
                case "S1F17":// request online by host
                    SubScenarioName = Resource.Intialize_Scenario_3;
                    secsMessage.S1F18("0");
                    if (LaunchControlStateProcess((int)ControlState.ONLINE_REMOTE))
                    {
                        if (LaunchDateTimeUpdateProcess())
                        {
                            LaunchControlStateProcess((int)ControlState.EQT_STATUS_CHANGE);
                        }
                    }
                    break;
                case "S1F15":// request offline by host
                    SubScenarioName = Resource.Intialize_Scenario_4;
                    switch (EQT_ControlState)
                    {
                        case ControlState.OFFLINE:
                            //send equipment denies requests
                            S1F0();
                            break;
                        case ControlState.ONLINE_LOCAL:
                        case ControlState.ONLINE_REMOTE:
                            // send OFF_LINE Acknowledge
                            secsMessage.S1F16("1");
                            // send Control State Change(OFF_LINE)
                            LaunchControlStateProcess((int)ControlState.OFFLINE);
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
        /// 启动建立连接进程 online by Unit
        /// </summary>
        /// <returns></returns>
        public bool LaunchOnlineProcess()
        {
            SubScenarioName = Resource.Intialize_Scenario_1;
            // send estublish communication request
            var replyMsg = S1F13("MDLN", "SOFTREV");

            if (!(replyMsg != null && replyMsg.GetSFString() == "S1F14"))
            {
                return false;
            }

            replyMsg = S1F1();
            if (replyMsg == null || replyMsg.F == 0)
            {// host denies online request
                return false;
            }
            if (LaunchControlStateProcess((int)ControlState.ONLINE_REMOTE))
            {
                if (LaunchDateTimeUpdateProcess())
                {
                    return LaunchControlStateProcess((int)ControlState.EQT_STATUS_CHANGE);
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
            var result = LaunchControlStateProcess((int)ControlState.OFFLINE);
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
            var replyMsg = S1F13("MDLN", "SOFTREV");
            if (replyMsg != null && replyMsg.GetSFString() == "S1F14")
            {
                var ack = replyMsg.GetCommandValue();
                if (ack == 0)
                {
                    // send ON_LINE request
                    replyMsg = S1F17("CRST");
                    if (replyMsg != null && replyMsg.GetSFString() == "S1F18")
                    {
                        ack = replyMsg.GetCommandValue();
                        if (ack == 0)
                        {
                            EQT_ControlState = ControlState.ONLINE_REMOTE;
                            itializeScenario?.UpdateControlState(ControlState.ONLINE_REMOTE);
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
                if (ack == 1)
                {
                    EQT_ControlState = ControlState.OFFLINE;
                    itializeScenario?.UpdateControlState(ControlState.OFFLINE);
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
        /// <returns></returns>
        public bool LaunchControlStateProcess(int ceid)
        {
            if (ceid >= 111 && ceid <= 114)
            {// control state change
                var stack = new Stack<List<Item>>();
                stack.Push(new List<Item>());
                stack.Peek().Add(A("DATAID"));
                stack.Peek().Add(A($"{ceid}"));
                stack.Push(new List<Item>());
                stack.Push(new List<Item>());
                stack.Peek().Add(A("RPTID"));
                stack.Push(new List<Item>());
                stack.Peek().Add(A("CRST"));
                stack.Peek().Add(A("EQST"));
                stack.Peek().Add(A("EQSTCODE"));

                var item = ParseItem(stack);
                var replyMsg = S6F11(item, (int)ceid);
                if (replyMsg != null && replyMsg.GetSFString() == "S6F12")
                {
                    try
                    {
                        int ack = replyMsg.GetCommandValue();
                        if (ack == 0)
                        {
                            EQT_ControlState = (ControlState)ceid;
                            itializeScenario?.UpdateControlState((ControlState)ceid);
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

        public interface IItializeScenario
        {
            void UpdateControlState(ControlState controlState);
            void UpdateDateTime(string dateTimeStr);
        }

        private class DefaultIItializeScenario : IItializeScenario
        {
            public void UpdateControlState(ControlState controlState)
            {
                Debug.WriteLine("Control State Changed: " + controlState.ToString());
            }

            public void UpdateDateTime(string dateTimeStr)
            {
                Debug.WriteLine("date and time update: " + dateTimeStr);
            }
        }
    }
}
