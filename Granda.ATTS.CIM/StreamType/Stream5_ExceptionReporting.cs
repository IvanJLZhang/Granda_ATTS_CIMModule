using Secs4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Granda.ATTS.CIM.CIMBASE;
using static Granda.ATTS.CIM.Extension.SmlExtension;
using static Secs4Net.Item;
namespace Granda.ATTS.CIM.StreamType
{
    internal static class Stream5_ExceptionReporting
    {
        /// <summary>
        /// Alarm Report Send
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S5F1()
        {
            return SendMessage(5, 1, true, null);
        }
        /// <summary>
        /// Alarm Report Acknowledge
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S5F2()
        {
            return SendMessage(5, 2, false, null);
        }

        /// <summary>
        /// Enable/Disable Alarm Send(EAS)
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S5F3(Item item)
        {
            return SendMessage(5, 3, true, item);
        }
        /// <summary>
        /// Enable/Disable Alarm Acknowledge(EAA)
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S5F4(this SecsMessage secsMessage, int ack)
        {
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>()
            {
                A(ack.ToString()),
            });
            return SendMessage(5, 4, secsMessage.SystenBytes, ParseItem(stack));
        }

        /// <summary>
        /// List Alarms Data Request(LAR)
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S5F5()
        {
            return SendMessage(5, 5, true, null);
        }
        /// <summary>
        /// List Alarm Data(LAD)
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S5F6()
        {
            return SendMessage(5, 6, false, null);
        }

        /// <summary>
        /// Current Alarm Set List Request
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S5F103(Item item)
        {
            return SendMessage(5, 103, true, item);
        }
        /// <summary>
        /// Current Alarm Set List Data
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S5F104(this SecsMessage secsMessage, Item item)
        {
            return SendMessage(5, 104, secsMessage.SystenBytes, item);
        }
    }
}
