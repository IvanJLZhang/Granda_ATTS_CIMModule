using Secs4Net;
using System.Collections.Generic;
using static Granda.ATTS.CIMModule.CimModuleProcess;
using static Granda.ATTS.CIMModule.Extension.SmlExtension;
using static Secs4Net.Item;

namespace Granda.ATTS.CIMModule.StreamType
{
    internal class Stream2_EquipmentControl
    {
        /// <summary>
        /// Equipment Constants Request
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F13(string ECID)
        {
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>() {
                A(ECID),
            });
            var item = ParseItem(stack);

            return SendMessage(2, 13, true, item);
        }
        /// <summary>
        /// Equipment Constant Data
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F14(string ECID)
        {
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>() {
                A(ECID),
            });
            var item = ParseItem(stack);

            return SendMessage(2, 14, false, item);
        }
        /// <summary>
        /// New Equipment Constants Send
        /// </summary>
        /// <param name="ECID"></param>
        /// <param name="ECV"></param>
        /// <returns></returns>
        public static SecsMessage S2F15(string ECID, string ECV)
        {
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>() {
                A(ECID),
                A(ECV),
            });
            var item = ParseItem(stack);

            return SendMessage(2, 15, true, item);
        }
        /// <summary>
        /// New Equipment Constant Ack.
        /// </summary>
        /// <param name="EAC"></param>
        /// <returns></returns>
        public static SecsMessage S2F16(string EAC)
        {
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>() {
                A(EAC),
            });
            var item = ParseItem(stack);

            return SendMessage(2, 15, false, item);
        }

        /// <summary>
        /// Date and Time Request
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F17()
        {
            return SendMessage(2, 17, true, null);
        }
        /// <summary>
        /// Date and Time Data
        /// </summary>
        /// <param name="TIME"></param>
        /// <returns></returns>
        public static SecsMessage S2F18(string TIME)
        {
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>() {
                A(TIME),
            });
            var item = ParseItem(stack);

            return SendMessage(2, 18, false, item);
        }
        //  <L[5]
        //	1.<A[2] ‘2 Bytes’ [TRID]>
        //	2.<A[6] ‘6 Bytes’ [DSPER]>
        //	3.<A[5] ‘5 Bytes’ [TOTSMP]>
        //	4.<A[3] ‘3 Bytes’ [REPGSZ]>
        //	5.<L[n]
        //		1.< A[5] ‘5 Bytes’ [SVID]>
        //※  n is requested SVID count.
        //※  TOTSMP = -1 means infinite count.
        /// <summary>
        /// Trace Initialize Send 待定
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F23()
        {
            return SendMessage(2, 23, true, null);
        }
        /// <summary>
        /// Trace Initialize Acknowledge
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F24(string TIAACK)
        {
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>() {
                A(TIAACK),
            });
            var item = ParseItem(stack);

            return SendMessage(2, 24, false, item);
        }

        /// <summary>
        /// Trace Initialize Acknowledge 待定
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F29(string ECID)
        {
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>() {
                A(ECID),
            });
            var item = ParseItem(stack);

            return SendMessage(2, 29, true, item);
        }
        //  <L[n]
        //	1.<L[5]
        //		1.<A[4] ‘4 Bytes’ [ECID]>
        //		2.<A[40] ’40 Bytes’ [ECNAME]>
        //		3.<A[10] ’10 Bytes’ [ECMIN]>
        //		4.<A[10] ’10 Bytes’ [ECMAX]>
        //		5.<A[10] ’10 Bytes’ [ECV]>

        //※  n is EC List count
        /// <summary>
        /// Equipment Constant Name List Reply
        /// </summary>
        /// <param name="ECID"></param>
        /// <returns></returns>
        public static SecsMessage S2F30(string ECID)
        {
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>() {
                A(ECID),
            });
            var item = ParseItem(stack);

            return SendMessage(2, 30, false, item);
        }
        /// <summary>
        /// Date and Time Set Request (DTS)
        /// </summary>
        /// <param name="TIME"></param>
        /// <returns></returns>
        public static SecsMessage S2F31(string TIME)
        {
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>() {
                A(TIME),
            });
            var item = ParseItem(stack);

            return SendMessage(2, 31, true, item);
        }
        /// <summary>
        /// Date and Time Set Acknowledge (DTA)
        /// </summary>
        /// <param name="TIACK"></param>
        /// <returns></returns>
        public static SecsMessage S2F32(string TIACK)
        {
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>() {
                A(TIACK),
            });
            var item = ParseItem(stack);

            return SendMessage(2, 32, false, item);
        }
        /// <summary>
        /// Enable or Disable Event Report
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F37()
        {
            return SendMessage(2, 37, true, null);
        }
        /// <summary>
        /// Enable or Disable Event Report Acknowledge
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F38()
        {
            return SendMessage(2, 38, false, null);
        }

        /// <summary>
        /// Current Enable/Disable Event List
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F39()
        {
            return SendMessage(2, 39, true, null);
        }
        /// <summary>
        /// Current Enable/Disable Event List
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F40()
        {
            return SendMessage(2, 40, false, null);
        }

        /// <summary>
        /// Host Command Send
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F41()
        {
            return SendMessage(2, 41, true, null);
        }
        /// <summary>
        /// Host Command Acknowledge
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F42()
        {
            return SendMessage(2, 42, false, null);
        }

        /// <summary>
        /// Crate glass QTY download
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F53()
        {
            return SendMessage(2, 53, true, null);
        }
        /// <summary>
        /// Crate glass QTY download Ack.
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F54()
        {
            return SendMessage(2, 54, false, null);
        }

        /// <summary>
        /// Cassette Information Download
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F103()
        {
            return SendMessage(2, 103, true, null);
        }
        /// <summary>
        /// Cassette Information Download Ack.
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F104()
        {
            return SendMessage(2, 104, false, null);
        }

        /// <summary>
        /// Empty CST Permission
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F105()
        {
            return SendMessage(2, 105, true, null);
        }
        /// <summary>
        /// Empty CST Permission Reply
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F106()
        {
            return SendMessage(2, 106, false, null);
        }

        /// <summary>
        /// Sorter Job Command
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F107()
        {
            return SendMessage(2, 107, true, null);
        }
        /// <summary>
        /// Sorter Job Command Ack
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F108()
        {
            return SendMessage(2, 108, false, null);
        }

        /// <summary>
        /// Mask cassette information Download
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F109()
        {
            return SendMessage(2, 109, true, null);
        }
        /// <summary>
        /// Mask cassette information Download ack
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F110()
        {
            return SendMessage(2, 110, false, null);
        }

        /// <summary>
        /// Mask cassette information Download EVA
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F111()
        {
            return SendMessage(2, 111, true, null);
        }
        /// <summary>
        /// Mask cassette information Download EVA ack
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F112()
        {
            return SendMessage(2, 112, false, null);
        }

        /// <summary>
        /// Mask offset information Download
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F119()
        {
            return SendMessage(2, 119, true, null);
        }
        /// <summary>
        /// Mask offset information Download Ack
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F120()
        {
            return SendMessage(2, 120, false, null);
        }

        /// <summary>
        /// Job Reservation Command
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F121()
        {
            return SendMessage(2, 121, true, null);
        }
        /// <summary>
        /// Remind Job Start Signal Reply
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F122()
        {
            return SendMessage(2, 122, false, null);
        }

        /// <summary>
        /// Remind Job Start Signal
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F123()
        {
            return SendMessage(2, 123, true, null);
        }
        /// <summary>
        /// Remind Job Start Signal Ack
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F124()
        {
            return SendMessage(2, 124, false, null);
        }

        /// <summary>
        /// Port PPID Send
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F131()
        {
            return SendMessage(2, 131, true, null);
        }
        /// <summary>
        /// Port PPID Send Ack
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F132()
        {
            return SendMessage(2, 132, false, null);
        }

        /// <summary>
        /// Send Packing boxID Download
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F203()
        {
            return SendMessage(2, 203, true, null);
        }
        /// <summary>
        /// Send Packing box ID Download ack
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F204()
        {
            return SendMessage(2, 204, false, null);
        }

        /// <summary>
        /// Mask Eject Request
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F211()
        {
            return SendMessage(2, 211, true, null);
        }
        /// <summary>
        /// Mask Eject Request reply
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F212()
        {
            return SendMessage(2, 212, false, null);
        }

        /// <summary>
        /// Loading Stop
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F221()
        {
            return SendMessage(2, 221, true, null);
        }
        /// <summary>
        /// Loading Stop Acknowledge
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F222()
        {
            return SendMessage(2, 222, false, null);
        }

        /// <summary>
        /// Work Order Request
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F231()
        {
            return SendMessage(2, 231, true, null);
        }
        /// <summary>
        /// Work Order Reply
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S2F232()
        {
            return SendMessage(2, 232, false, null);
        }
    }
}
