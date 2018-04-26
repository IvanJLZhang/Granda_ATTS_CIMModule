using Granda.HSMS;
using static Granda.ATTS.CIM.CIMBASE;
namespace Granda.ATTS.CIM.StreamType
{
    internal static class Stream7_ProcessProgramManagement
    {
        /// <summary>
        /// Current EPPD Request
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S7F19(Item item)
        {
            return SendMessage(7, 19, true, item);
        }
        /// <summary>
        /// Current EPPD Data
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S7F20(this SecsMessage secsMessage, Item item)
        {
            return SendMessage(7, 20, secsMessage.SystenBytes, item);
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
        public static SecsMessage S7F25(Item item)
        {
            return SendMessage(7, 25, true, item);
        }
        /// <summary>
        /// Formatted Process Program Data
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S7F26(this SecsMessage secsMessage, Item item)
        {
            return SendMessage(7, 26, secsMessage.SystenBytes, item);
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
