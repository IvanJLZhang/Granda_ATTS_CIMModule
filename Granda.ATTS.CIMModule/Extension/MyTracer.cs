using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Secs4Net;

namespace Granda.ATTS.CIMModule.Extension
{
    public class MyTracer : Secs4Net.SecsTracer
    {
        public override void TraceError(string msg)
        {
            base.TraceError(msg);
            Debug.WriteLine(msg);

        }

        public override void TraceInfo(string msg)
        {
            base.TraceInfo(msg);
            Debug.WriteLine(msg);
        }

        public override void TraceMessageIn(SecsMessage msg, int systembyte)
        {
            base.TraceMessageIn(msg, systembyte);
            Debug.WriteLine($"systemByte:{systembyte}\r\n" + msg.ToSML());

        }

        public override void TraceMessageOut(SecsMessage msg, int systembyte)
        {
            base.TraceMessageOut(msg, systembyte);
            Debug.WriteLine($"systemByte:{systembyte}\r\n" + msg.ToSML());

        }

        public override void TraceWarning(string msg)
        {
            base.TraceWarning(msg);
            Debug.WriteLine(msg);

        }
    }
}
