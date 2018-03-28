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
namespace Granda.ATTS.CIMModule.Scenario
{
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
                    SubScenarioName = Resource.Intialize_Scenario_1;
                    Stack<List<Item>> stack = new Stack<List<Item>>();
                    stack.Push(new List<Item>());
                    var item = A("0");
                    stack.Peek().Add(item);
                    stack.Push(new List<Item>());
                    stack.Peek().Add(A("MDLN"));
                    stack.Peek().Add(A("SOFTREV"));
                    item = ParseItem(stack);
                    var msg = new SecsMessage(secsMessage.S, (byte)(secsMessage.F + 1)
                        , GetFunctionName(secsMessage.S, (byte)(secsMessage.F + 1)) ?? String.Empty
                        , false, item);
                    secsGemService.Send(msg);
                    break;
                case "S1F17":
                    SubScenarioName = Resource.Intialize_Scenario_1;
                    stack = new Stack<List<Item>>();
                    stack.Push(new List<Item>() { A("CRST") });
                    item = ParseItem(stack);
                    msg = new SecsMessage(secsMessage.S, (byte)(secsMessage.F + 1)
                        , GetFunctionName(secsMessage.S, (byte)(secsMessage.F + 1)) ?? String.Empty
                        , false, item);
                    secsGemService.Send(msg);
                    break;

                case "S1F15":
                    break;
                default:
                    break;
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
            SecsMessage secsMessage = new SecsMessage(1, 14, ExtensionHelper.GetFunctionName(1, 14), false, item);
            //        var msg = new SecsMessage(secsMessage.S, (byte)(secsMessage.F + 1)
            //, ExtensionHelper.GetFunctionName(secsMessage.S, (byte)(secsMessage.F + 1)) ?? String.Empty
            //, false, item);
            string sml = secsMessage.ToSML();
        }
    }
}
