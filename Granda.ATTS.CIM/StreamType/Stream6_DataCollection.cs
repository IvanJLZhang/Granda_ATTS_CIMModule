using System;
using Secs4Net;
using static Granda.ATTS.CIM.CIMBASE;
using static Secs4Net.Item;
namespace Granda.ATTS.CIM.StreamType
{
    internal static class Stream6_DataCollection
    {
        /// <summary>
        /// Trace Data Send
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S6F1(Item item)
        {
            return SendMessage(6, 1, false, item);
        }

        /// <summary>
        /// Discrete Variable Data Send
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S6F3(Item item, int ceid)
        {
            return SendMessage(6, 3, true, item, ceid, "CEID");
        }
        /// <summary>
        /// Discrete Variable Data Acknowledge
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S6F4(this SecsMessage secsMessage, int ack)
        {
            return SendMessage(6, 4, secsMessage.SystenBytes, A(ack.ToString()));
        }

        /// <summary>
        /// Event Report Send
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S6F11(Item item, int ceid)
        {
            return SendMessage(6, 11, true, item, ceid, "CEID");
        }
        /// <summary>
        /// Event Report Acknowledge
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S6F12(this SecsMessage secsMessage, string ack)
        {
            return SendMessage(6, 12, secsMessage.SystenBytes, A(ack ?? String.Empty));
        }

        /// <summary>
        /// Cassette Information Upload
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S6F103()
        {
            return SendMessage(6, 103, true, null);
        }
        /// <summary>
        /// Cassette Information Upload Ack.
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S6F104()
        {
            return SendMessage(6, 104, false, null);
        }

        /// <summary>
        /// Mask cassette information Upload
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S6F109()
        {
            return SendMessage(6, 109, true, null);
        }
        /// <summary>
        /// Mask cassette information Upload Ack
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S6F110()
        {
            return SendMessage(6, 110, false, null);
        }

        /// <summary>
        /// Mask offset information Upload
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S6F119()
        {
            return SendMessage(6, 119, true, null);
        }
        /// <summary>
        /// Mask offset information Upload Ack
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S6F120()
        {
            return SendMessage(6, 120, false, null);
        }

        /// <summary>
        /// Job Reservation Reset Request
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S6F121()
        {
            return SendMessage(6, 121, true, null);
        }
        /// <summary>
        /// Job Reservation Reset Request Ack
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S6F122()
        {
            return SendMessage(6, 122, false, null);
        }

        /// <summary>
        /// Glass Call Data Request
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S6F131()
        {
            return SendMessage(6, 131, true, null);
        }
        /// <summary>
        /// Glass Call Data Request Ack
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S6F132()
        {
            return SendMessage(6, 132, false, null);
        }

        /// <summary>
        /// Packing box information upload
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S6F203()
        {
            return SendMessage(6, 203, true, null);
        }
        /// <summary>
        /// Packing box information upload reply
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S6F204()
        {
            return SendMessage(6, 204, false, null);
        }
    }
}
