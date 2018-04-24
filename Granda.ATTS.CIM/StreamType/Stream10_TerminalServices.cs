using Secs4Net;
using static Granda.ATTS.CIM.CIMBASE;
using static Secs4Net.Item;

namespace Granda.ATTS.CIM.StreamType
{
    internal static class Stream10_TerminalServices
    {
        /// <summary>
        /// Terminal Request
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S10F1(Item item)
        {
            return SendMessage(10, 1, true, item);
        }
        /// <summary>
        /// Terminal Request Acknowledge
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S10F2(this SecsMessage secsMessage, int ack)
        {
            return SendMessage(10, 2, secsMessage.SystenBytes, A(ack.ToString()));
        }

        /// <summary>
        /// Terminal Display, Multi-block
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S10F5(Item item)
        {
            return SendMessage(10, 5, true, item);
        }
        /// <summary>
        /// Terminal Display, Multi-block Ack.
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S10F6(this SecsMessage secsMessage, int ack)
        {
            return SendMessage(10, 6, secsMessage.SystenBytes, A(ack.ToString()));
        }
    }
}
