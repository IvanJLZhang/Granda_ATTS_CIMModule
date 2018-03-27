using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granda.ATTS.CIMModule.Core;
using Granda.ATTS.CIMModule.Model;
using Secs4Net;

namespace Granda.ATTS.CIMModule.Scenario
{
    public class InitializeScenario : IScenario
    {
        public InitializeScenario(SecsMessage message, MessageHeader header)
        {
            secsMessage = message;
            messageHeader = header;
        }

        SecsMessage secsMessage;
        MessageHeader messageHeader;

        public Scenarios ScenarioType { get; set; } = Scenarios.Intialize_Scenario;
        public string ScenarioName { get => ScenarioType.ToString().Replace("_", " "); }
        public string SubScenarioName { get; set; }

        public void PackSecsMessage()
        {
            throw new NotImplementedException();
        }

        public void ParseSecsMessage()
        {
            throw new NotImplementedException();
        }
    }
}
