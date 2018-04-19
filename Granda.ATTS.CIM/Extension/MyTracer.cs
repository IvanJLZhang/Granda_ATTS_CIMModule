//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using Secs4Net;

//namespace Granda.ATTS.CIM.Extension
//{
//    /// <summary>
//    /// 默认的Tracer类，用于Debug在控制台输出内容
//    /// </summary>
//    public class MyTracer : SecsTracer
//    {
//        /// <summary>
//        /// 错误
//        /// </summary>
//        /// <param name="msg"></param>
//        public override void TraceError(string msg)
//        {
//            base.TraceError(msg);
//            Debug.WriteLine(msg);

//        }
//        /// <summary>
//        /// 信息
//        /// </summary>
//        /// <param name="msg"></param>
//        public override void TraceInfo(string msg)
//        {
//            base.TraceInfo(msg);
//            Debug.WriteLine(msg);
//        }
//        /// <summary>
//        /// message in
//        /// </summary>
//        /// <param name="msg"></param>
//        /// <param name="systembyte"></param>
//        public override void TraceMessageIn(SecsMessage msg, int systembyte)
//        {
//            base.TraceMessageIn(msg, systembyte);
//            Debug.WriteLine($"systemByte:{systembyte}\r\n" + msg.ToSML());

//        }
//        /// <summary>
//        /// message out
//        /// </summary>
//        /// <param name="msg"></param>
//        /// <param name="systembyte"></param>
//        public override void TraceMessageOut(SecsMessage msg, int systembyte)
//        {
//            base.TraceMessageOut(msg, systembyte);
//            Debug.WriteLine($"systemByte:{systembyte}\r\n" + msg.ToSML());

//        }
//        /// <summary>
//        /// warning
//        /// </summary>
//        /// <param name="msg"></param>
//        public override void TraceWarning(string msg)
//        {
//            base.TraceWarning(msg);
//            Debug.WriteLine(msg);

//        }
//    }
//}
