using Secs4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granda.ATTS.CIM.Data
{
    /// <summary>
    /// 转换消息接口
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// 将SecsItem转换为业务数据类型
        /// </summary>
        /// <param name="item"></param>
        void Parse(Item item);
    }
}
