#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: CIMEventArgs
// Author: Ivan JL Zhang    Date: 2018/4/18 10:44:04    Version: 1.0.0
// Description: 
//   
// 
// Revision History: 
// <Author>  		<Date>     	 	<Revision>  		<Modification>
// 	
//----------------------------------------------------------------------------*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granda.ATTS.CIM
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CIMEventArgs<T> : EventArgs
    {
        /// <summary>
        /// /
        /// </summary>
        /// <param name="data"></param>
        /// <param name="needReply"></param>
        public CIMEventArgs(T data, bool needReply = false)
        {
            Data = data;
            this.NeedReply = needReply;
        }
        /// <summary>
        /// 
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool NeedReply { get; set; }
    }
}
