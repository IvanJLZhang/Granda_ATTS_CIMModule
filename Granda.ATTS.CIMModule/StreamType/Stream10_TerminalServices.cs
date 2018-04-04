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
    internal class Stream10_TerminalServices
    {
        /// <summary>
        /// Terminal Request
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S10F1()
        {
            return SendMessage(10, 1, true, null);
        }
        /// <summary>
        /// Terminal Request Acknowledge
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S10F2()
        {
            return SendMessage(10, 2, false, null);
        }

        /// <summary>
        /// Terminal Display, Multi-block
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S10F5()
        {
            return SendMessage(10, 5, true, null);
        }
        /// <summary>
        /// Terminal Display, Multi-block Ack.
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S10F6()
        {
            return SendMessage(10, 6, false, null);
        }
    }
}
