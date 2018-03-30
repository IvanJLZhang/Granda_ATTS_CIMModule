using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granda.ATTS.CIMModule.Core;
using Granda.ATTS.CIMModule.Extension;
using static Granda.ATTS.CIMModule.Extension.SmlExtension;
using static Granda.ATTS.CIMModule.Extension.ExtensionHelper;
using Granda.ATTS.CIMModule.Model;
using Secs4Net;
using static Secs4Net.Item;
using System.Diagnostics;
using static Granda.ATTS.CIMModule.StreamType.Stream1_EquipmentStatus;
using static Granda.ATTS.CIMModule.StreamType.Stream6_DataCollection;
using static Granda.ATTS.CIMModule.StreamType.Stream2_EquipmentControl;
namespace Granda.ATTS.CIMModule.Scenario
{
    /// <summary>
    /// 初始化场景处理类
    /// </summary>
    internal class InitializeScenario : IScenario
    {
        /// <summary>
        /// 构造函数： 用于处理接收消息时进行初始化
        /// </summary>
        /// <param name="message"></param>
        public InitializeScenario(SecsMessage message)
        {
            primaryMessage = message;
        }
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public InitializeScenario(IItializeScenario callBack)
        {
            itializeScenario = callBack;
        }
        /// <summary>
        /// primary message cache
        /// </summary>
        public SecsMessage primaryMessage { get; set; } = null;

        /// <summary>
        /// 场景类型
        /// </summary>
        public Scenarios ScenarioType { get; } = Scenarios.Intialize_Scenario;
        /// <summary>
        /// 场景类型名称
        /// </summary>
        public string ScenarioName { get => ScenarioType.ToString().Replace("_", " "); }
        /// <summary>
        /// 场景下功能名称
        /// </summary>
        public string SubScenarioName { get; set; } = String.Empty;
        /// <summary>
        /// 设备的控制状态
        /// </summary>
        public ControlState EQT_ControlState { get; private set; } = ControlState.OFFLINE;

        private IItializeScenario itializeScenario = new DefaultIItializeScenario();

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public void PackSecsMessage()
        {
            //secsGemService.Send()
        }

        #region Initialize Scenario: 
        /// <summary>
        /// handle online/offline request by host
        /// </summary>
        public void HandleSecsMessage(SecsMessage secsMessage)
        {
            primaryMessage = secsMessage;
            switch (primaryMessage.GetSFString())
            {
                case "S1F13":
                    SubScenarioName = Resource.Intialize_Scenario_3;
                    S1F14("MDLN", "SOFTREV", "0");
                    break;
                case "S1F17":
                    SubScenarioName = Resource.Intialize_Scenario_3;
                    S1F18("0");
                    if (LaunchControlStateProcess(ControlState.ONLINE_REMOTE))
                    {
                        LaunchDateTimeUpdateProcess();
                    }
                    break;
                case "S1F15":
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
                            S1F16("1");
                            // send Control State Change(OFF_LINE)
                            LaunchControlStateProcess(ControlState.OFFLINE);
                            break;
                        default:
                            break;
                    }
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
            //var replyMsg = S1F14("MDLN", "SOFTREV", "0");

            if (!(replyMsg != null && replyMsg.S == 1 && replyMsg.F == 14))
            {
                return false;
            }
            replyMsg = S1F1();
            if (replyMsg == null || replyMsg.F == 0)
            {// host denies online request
                return false;
            }

            if (LaunchControlStateProcess(ControlState.ONLINE_REMOTE))
            {
                replyMsg = S2F17();
                if (replyMsg != null && replyMsg.S == 2 && replyMsg.F == 18)
                {
                    var dateTimeStr = replyMsg.SecsItem.GetValue<string>();
                    itializeScenario?.UpdateDateTime(dateTimeStr);
                    return true;
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
            var result = LaunchControlStateProcess(ControlState.OFFLINE);
            return result;
        }
        #endregion

        #region 其他可公开的控制进程
        /// <summary>
        /// 设置Control State状态：
        /// CEID 113==>ONLINE REMOTE
        ///      112==>ONLINE LOCAL
        ///      111==>OFFLINE
        /// </summary>
        /// <param name="controlState"></param>
        /// <returns></returns>
        public bool LaunchControlStateProcess(ControlState controlState)
        {
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>());
            stack.Peek().Add(A("DATAID"));
            stack.Peek().Add(A($"{controlState}"));
            stack.Push(new List<Item>());
            stack.Push(new List<Item>());
            stack.Peek().Add(A("RPTID"));
            stack.Push(new List<Item>());
            stack.Peek().Add(A("CRST"));
            stack.Peek().Add(A("EQST"));
            stack.Peek().Add(A("EQSTCODE"));

            var item = ParseItem(stack);
            var replyMsg = S6F11(item, (int)controlState);
            if (replyMsg != null && replyMsg.GetSFString() == "S6F12")
            {
                try
                {
                    string ack = replyMsg.SecsItem.GetString();
                    if (ack == "0")
                    {
                        EQT_ControlState = controlState;
                        itializeScenario?.UpdateControlState(controlState);
                        return true;
                    }
                }
                catch (InvalidOperationException)
                {
                    return false;
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

        public void CustomMethod()
        {
            Stack<List<Item>> stack = new Stack<List<Item>>();
            stack.Push(new List<Item>());
            var item = A("0");
            stack.Peek().Add(item);
            stack.Push(new List<Item>());
            stack.Peek().Add(A("MDLN"));
            stack.Peek().Add(A("SOFTREV"));
            item = ParseItem(stack);

            string str = item.ToString();
            GetItemStr(item);
            SecsMessage secsMessage = new SecsMessage(1, 14, ExtensionHelper.GetFunctionName(1, 14), false, item);
            string sml = secsMessage.ToSML();
        }

        public void GetItemStr(Item item)
        {
            for (int index = 0; index < item.Count; index++)
            {
                var ite = item.Items[index];
                if (ite.Count > 1)
                    GetItemStr(ite);
                else if (ite.Count == 1)
                {
                    var str = ite.GetString();
                    Debug.WriteLine(str);
                }
            }
        }
    }
}
