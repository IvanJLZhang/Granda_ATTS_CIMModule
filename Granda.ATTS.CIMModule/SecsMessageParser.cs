using Granda.ATTS.CIMModule.Core;
using Granda.ATTS.CIMModule.Model;
using Granda.ATTS.CIMModule.Scenario;
using Secs4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granda.ATTS.CIMModule
{
    public static class SecsMessageParser
    {
        public static IScenario Parser(MessageHeader messageHeader, SecsMessage secsMessage) {
            IScenario scenario = null;
            switch ((StreamType)secsMessage.S)
            {
                case StreamType.Equipment_Status:
                    break;
                case StreamType.Equipment_Control:
                    break;
                case StreamType.Exception_Report:
                    break;
                case StreamType.Data_Collection:
                    break;
                case StreamType.Process_Program_Management:
                    break;
                case StreamType.System_Errors:
                    break;
                case StreamType.Terminal_Services:
                    break;
                default:
                    break;
            }
            var sf = secsMessage.GetSFString();
            switch (sf)
            {
                case "S1F13":
                case "S1F14":
                case "S1F1":
                case "S1F2":
                case "S1F0":
                case "S6F11":
                case "S6F12":
                case "S2F17":
                case "S2F18":
                    scenario = new InitializeScenario(secsMessage, messageHeader);
                    break;
                default:
                    break;
            }
            return scenario;
        }

        public static String GetSFString(this SecsMessage secsMessage)
        {
            return $"S{secsMessage.S}F{secsMessage.F}";
        }
    }
}
