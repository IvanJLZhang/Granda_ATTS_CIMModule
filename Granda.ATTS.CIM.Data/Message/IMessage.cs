using Secs4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granda.ATTS.CIM.Data
{
    public interface IMessage
    {
        void Parse(Item item);
    }
}
