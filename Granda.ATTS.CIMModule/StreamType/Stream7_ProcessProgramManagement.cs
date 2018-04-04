using Secs4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Granda.ATTS.CIMModule.CimModuleBase;
using static Granda.ATTS.CIMModule.Extension.SmlExtension;
using static Secs4Net.Item;

namespace Granda.ATTS.CIMModule.StreamType
{
    internal class Stream7_ProcessProgramManagement
    {
        /// <summary>
        /// Current EPPD Request
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S7F19()
        {
            return SendMessage(7, 19, true, null);
        }
        /// <summary>
        /// Current EPPD Data
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S7F20()
        {
            return SendMessage(7, 20, false, null);
        }

        /// <summary>
        /// Formatted Process Program Send
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S7F23()
        {
            return SendMessage(7, 23, true, null);
        }
        /// <summary>
        /// Formatted Process Program Acknowledge
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S7F24()
        {
            return SendMessage(7, 24, false, null);
        }

        /// <summary>
        /// Formatted Process Program Request
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S7F25()
        {
            return SendMessage(7, 25, true, null);
        }
        /// <summary>
        /// Formatted Process Program Data
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S7F26()
        {
            return SendMessage(7, 26, false, null);
        }

        /// <summary>
        /// Recipe ID Check
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S7F73()
        {
            return SendMessage(7, 25, true, null);
        }
        /// <summary>
        /// Recipe ID Check Acknowledge
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S7F74()
        {
            return SendMessage(7, 26, false, null);
        }
    }
}
