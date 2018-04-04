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
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>() {
                A(ack.ToString()),
            });
            return SendMessage(10, 2, secsMessage.SystenBytes, ParseItem(stack));
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
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>() {
                A(ack.ToString()),
            });
            return SendMessage(10, 6, secsMessage.SystenBytes, ParseItem(stack));
        }
    }
}
