#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: ConnectionStatus
// Author: Ivan JL Zhang    Date: 2018/4/18 10:41:05    Version: 1.0.0
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

namespace Granda.ATTS.CIM.Data.ENUM
{
    /// <summary>
    /// Connection status
    /// </summary>
    public enum ConnectionStatus
    {
        /// <summary>
        /// 
        /// </summary>
        Disconnected,
        /// <summary>
        /// connecting
        /// </summary>
        Connecting,

        /// <summary>
        /// connected
        /// </summary>
        Connected,

        /// <summary>
        /// selected
        /// </summary>
        Selected,

        /// <summary>
        /// retry
        /// </summary>
        Retry
    }
}
