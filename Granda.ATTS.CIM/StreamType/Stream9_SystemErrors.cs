using Secs4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Granda.ATTS.CIM.CimModuleBase;
using static Granda.ATTS.CIM.Extension.SmlExtension;
using static Secs4Net.Item;
namespace Granda.ATTS.CIM.StreamType
{
    internal static class Stream9_SystemErrors
    {
        /// <summary>
        /// Unrecognized Device ID
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S9F1(this SecsMessage secsMessage)
        {
            return SendMessage(9, 1, false, null);
        }

        /// <summary>
        /// Unrecognized Stream Type
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S9F3(this SecsMessage secsMessage)
        {
            return SendMessage(9, 3, false, null);
        }

        /// <summary>
        /// Unrecognized Function Type
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S9F5(this SecsMessage secsMessage)
        {
            return SendMessage(9, 5, false, null);
        }

        /// <summary>
        /// Illegal Data
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S9F7(this SecsMessage secsMessage)
        {
            return SendMessage(9, 7, false, null);
        }

        /// <summary>
        /// Transaction Timer Timeout
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S9F9(this SecsMessage secsMessage)
        {
            return SendMessage(9, 9, false, null);
        }

        /// <summary>
        /// Conversation Timeout
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S9F13(this SecsMessage secsMessage)
        {
            return SendMessage(9, 13, false, null);
        }
    }
}
