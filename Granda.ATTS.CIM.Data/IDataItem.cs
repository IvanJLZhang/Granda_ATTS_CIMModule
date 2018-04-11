using Secs4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granda.ATTS.CIM.Data
{
    public interface IDataItem
    {
        /// <summary>
        /// 将数据结构体转换成Item
        /// </summary>
        Item SecsItem { get; }
    }
}
