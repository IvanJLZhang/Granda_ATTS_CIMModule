#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: CRST
// Author: Ivan JL Zhang    Date: 2018/4/11 12:33:50    Version: 1.0.0
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
    /// 描述Control State的三种状态枚举
    /// </summary>
    public enum CRST
    {
        /// <summary>
        /// offline
        /// </summary>
        O = 111,
        /// <summary>
        /// online local
        /// </summary>
        L = 112,
        /// <summary>
        /// online remote
        /// </summary>
        R = 113,
        /// <summary>
        /// equipment status change
        /// </summary>
        EQT_STATUS_CHANGE = 114
    }
}
