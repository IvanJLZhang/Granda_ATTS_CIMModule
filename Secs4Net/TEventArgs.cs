using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Secs4Net
{
    public class TEventArgs<T> : EventArgs
    {
        public TEventArgs(T data)
        {
            Data = data;
        }

        public T Data { get; set; }
    }
}
