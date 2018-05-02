using Granda.HSMS;
using static Granda.ATTS.CIM.CIMBASE;
namespace Granda.ATTS.CIM.StreamType
{
    internal static class Stream9_SystemErrors
    {
        /// <summary>
        /// Unrecognized Device ID
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S9F1(this SecsMessage secsMessage, Item item)
        {
            return SendMessage(9, 1, false, item);
        }

        /// <summary>
        /// Unrecognized Stream Type
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S9F3(this SecsMessage secsMessage, Item item)
        {
            return SendMessage(9, 3, false, item);
        }

        /// <summary>
        /// Unrecognized Function Type
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S9F5(this SecsMessage secsMessage, Item item)
        {

            return SendMessage(9, 5, false, item);
        }

        /// <summary>
        /// Illegal Data
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S9F7(this SecsMessage secsMessage, Item item)
        {
            return SendMessage(9, 7, false, item);
        }

        /// <summary>
        /// Transaction Timer Timeout
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S9F9(this SecsMessage secsMessage, Item item)
        {
            return SendMessage(9, 9, false, item);
        }

        /// <summary>
        /// Conversation Timeout
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S9F13(this SecsMessage secsMessage, Item item)
        {
            return SendMessage(9, 13, false, item);
        }
    }
}
