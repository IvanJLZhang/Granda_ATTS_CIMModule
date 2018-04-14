#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: CommonStatus
// Author: Ivan JL Zhang    Date: 2018/4/14 11:24:32    Version: 1.0.0
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
    /// Common Status enum: EQST/UNITST/SUNITST/SSUNITST
    /// </summary>
    public enum CommonStatus
    {
        /// <summary>
        /// IDLE
        /// </summary>
        I,
        /// <summary>
        /// RUN
        /// </summary>
        R,
        /// <summary>
        /// DOWN
        /// </summary>
        D,
        /// <summary>
        /// MAINT
        /// </summary>
        M,
        /// <summary>
        /// PAUSE
        /// </summary>
        P,
    }
}
