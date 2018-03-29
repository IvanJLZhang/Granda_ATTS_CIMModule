using Secs4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Granda.ATTS.CIMModule.CimModuleProcess;
using static Granda.ATTS.CIMModule.Extension.SmlExtension;
using static Secs4Net.Item;
namespace Granda.ATTS.CIMModule.StreamType
{
    internal class Stream5_ExceptionReporting
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
        public static SecsMessage S5F3()
        {
            return SendMessage(5, 3, true, null);
        }
        /// <summary>
        /// Enable/Disable Alarm Acknowledge(EAA)
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S5F4()
        {
            return SendMessage(5, 4, false, null);
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
        public static SecsMessage S5F103()
        {
            return SendMessage(5, 103, true, null);
        }
        /// <summary>
        /// Current Alarm Set List Data
        /// </summary>
        /// <returns></returns>
        public static SecsMessage S5F104()
        {
            return SendMessage(5, 104, false, null);
        }
    }
}
