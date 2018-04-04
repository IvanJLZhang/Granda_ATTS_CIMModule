#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: HostCommand
// Author: Ivan JL Zhang    Date: 2018/4/3 12:29:56    Version: 1.0.0
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

namespace Granda.ATTS.CIMModule.Model
{
    /// <summary>
    /// Remote Control Host Command枚举
    /// </summary>
    public enum HostCommand
    {
        /// <summary>
        /// Remote Control Host Command-START
        /// </summary>
        START = 1,
        /// <summary>
        /// Remote Control Host Command-CANCEL
        /// </summary>
        CANCEL,
        /// <summary>
        /// Remote Control Host Command-ABORT
        /// </summary>
        ABORT,
        /// <summary>
        /// Remote Control Host Command-PAUSE
        /// </summary>
        PAUSE,
        /// <summary>
        /// Remote Control Host Command-RESUME
        /// </summary>
        RESUME,
        /// <summary>
        /// Remote Control Host Command-OPERATOR_CALL
        /// </summary>
        OPERATOR_CALL,

    }
}
