using System;
using Granda.HSMS;
using static Granda.HSMS.Item;
using static Granda.ATTS.CIM.CIMBASE;
namespace Granda.ATTS.CIM.StreamType
{
    internal static class Stream1_EquipmentStatus
    {
        /// <summary>
        /// Equipment denies requests
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S1F0()
        {
            return SendMessage(1, 0, false, null, 0);
        }
        /// <summary>
        /// Are you there request
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S1F1()
        {
            return SendMessage(1, 1, true, null, 0);
        }

        public static SecsMessage S1F2(this SecsMessage secsMessage, string MDLN, string SOFTREV)
        {
            return SendMessage(1, 2, secsMessage.SystenBytes, L(A(MDLN ?? String.Empty), A(SOFTREV ?? String.Empty)));
        }
        /// <summary>
        /// Selected Equipment Status Request
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S1F3(Item item)
        {
            return SendMessage(1, 3, true, item);
        }
        /// <summary>
        /// Selected Equipment Status Data
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S1F4(this SecsMessage secsMessage, Item item)
        {
            return SendMessage(1, 4, secsMessage.SystenBytes, item);
        }
        /// <summary>
        /// Formatted Status Request (FSR)
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S1F5(int SFCD)
        {
            return SendMessage(1, 5, true, A(SFCD.ToString()), SFCD, "SFCD");
        }
        /// <summary>
        /// Formatted Status Request (FSR) ack
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S1F6(this SecsMessage secsMessage, Item item)
        {
            return SendMessage(1, 6, secsMessage.SystenBytes, item);
        }
        ///// <summary>
        ///// 待定
        ///// </summary>
        ///// <param name="SVID"></param>
        ///// <returns></returns>
        //public static SecsMessage S1F11(string SVID)
        //{
        //    var stack = new Stack<List<Item>>();
        //    stack.Push(new List<Item>() {
        //        A(SVID),
        //    });
        //    var item = ParseItem(stack);
        //    return SendMessage(1, 11, true, item);
        //}
        ///// <summary>
        ///// 待定
        ///// </summary>
        ///// <param name="SVID"></param>
        ///// <returns></returns>
        //public static SecsMessage S1F12(string SVID)
        //{
        //    var stack = new Stack<List<Item>>();
        //    stack.Push(new List<Item>() {
        //        A(SVID),
        //    });
        //    var item = ParseItem(stack);
        //    return SendMessage(1, 12, false, item);
        //}
        /// <summary>
        /// Establish Communication Request
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S1F13(Item item)
        {
            return SendMessage(1, 13, true, item);
        }
        /// <summary>
        /// Establish Communications Acknowledge
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S1F14(this SecsMessage secsMessage, string MDLN, string SOFTREV, string ACK)
        {
            return SendMessage(1, 14, secsMessage.SystenBytes, L(
                A(ACK ?? String.Empty),
                L(
                    A(MDLN ?? String.Empty),
                    A(SOFTREV ?? String.Empty)
                    )
                ));
        }
        /// <summary>
        /// Request OFF-LINE
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S1F15()
        {
            return SendMessage(1, 15, true, null);

        }
        /// <summary>
        /// OFF-LINE Acknowledge
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S1F16(this SecsMessage secsMessage, string OFLACK)
        {
            var item = A(OFLACK);
            return SendMessage(1, 16, secsMessage.SystenBytes, item);
        }

        /// <summary>
        /// Request ON-LINE
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S1F17(string CRST)
        {
            return SendMessage(1, 17, true, A(CRST ?? String.Empty));
        }
        /// <summary>
        /// ON-LINE Acknowledge
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S1F18(this SecsMessage secsMessage, string ONLACK)
        {
            var item = A(ONLACK);
            return SendMessage(1, 18, secsMessage.SystenBytes, item);
        }
    }
}
