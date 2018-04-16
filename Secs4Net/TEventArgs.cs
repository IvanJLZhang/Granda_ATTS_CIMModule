using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Secs4Net
{
    public class TEventArgs<T> : EventArgs
    {
        public TEventArgs(T data, bool needReply = false)
        {
            Data = data;
            this.NeedReply = needReply;
        }

        public T Data { get; set; }

        public bool NeedReply { get; set; }
    }
}
