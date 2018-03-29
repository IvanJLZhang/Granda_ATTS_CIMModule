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
    public class InitializeScenario : IScenario
    {
        public InitializeScenario(SecsMessage message, MessageHeader header, SecsGem secsGem, bool isFromHst = false)
        {
            secsMessage = message;
            messageHeader = header;
            secsGemService = secsGem;
            this.isFromHst = isFromHst;
            if (isFromHst)
            {
                ParseSecsMessage();
            }
            else
            {
                PackSecsMessage();
            }
        }
        public InitializeScenario(String smlStr, MessageHeader header, SecsGem secsGem, bool isFromHst = false)
            : this(smlStr.ToSecsMessage(), header, secsGem, isFromHst)
        {
        }
        public InitializeScenario()
        {

        }
        SecsMessage secsMessage;
        MessageHeader messageHeader;

        public Scenarios ScenarioType { get; set; } = Scenarios.Intialize_Scenario;
        public string ScenarioName { get => ScenarioType.ToString().Replace("_", " "); }
        public string SubScenarioName { get; set; }
        public bool isFromHst { get; set; } = false;
        public bool isNeedSend { get; set; } = false;

        public event EventHandler<ControlState> ControlStateChanged;
        public event EventHandler<string> DateTimeUpdated;
        readonly SecsGem secsGemService;

        public void PackSecsMessage()
        {
            //secsGemService.Send()
        }

        public void ParseSecsMessage()
        {
            switch (secsMessage.GetSFString())
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
                        var replyMsg = S2F17();
                        if (replyMsg != null && replyMsg.S == 2 && replyMsg.F == 18)
                        {
                            var dateTimeStr = replyMsg.SecsItem.GetValue<string>();
                            DateTimeUpdated?.Invoke(this, dateTimeStr);
                        }
                    }
                    break;
                case "S1F15":
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 启动建立连接进程
        /// </summary>
        /// <returns></returns>
        public bool LaunchOnlineProcess()
        {
            SubScenarioName = Resource.Intialize_Scenario_1;
            // send estublish communication request
            var replyMsg = S1F13("MDLN", "SOFTREV");
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
                    DateTimeUpdated?.Invoke(this, dateTimeStr);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 启动Offline进程
        /// </summary>
        /// <returns></returns>
        public bool LaunchOfflineProcess()
        {
            SubScenarioName = Resource.Intialize_Scenario_2;
            var result = LaunchControlStateProcess(ControlState.OFFLINE);
            return result;
        }
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
            if (replyMsg != null)
            {
                ControlStateChanged?.Invoke(this, controlState);
                return true;
            }
            return false;
        }
        private SecsMessage SendMessage(byte s, byte f, bool replyExpected, Item item = null, int ceid = 0)
        {
            if (secsGemService.State == ConnectionState.Selected)
            {
                return secsGemService.Send(new SecsMessage(s, f, GetFunctionName(s, f, ceid), replyExpected, item));
            }
            return null;
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
            SecsMessage secsMessage = new SecsMessage(1, 14, ExtensionHelper.GetFunctionName(1, 14), false, item);
            string sml = secsMessage.ToSML();
        }
    }
}
