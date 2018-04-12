#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: EquipmentStatus
// Author: Ivan JL Zhang    Date: 2018/4/11 13:59:49    Version: 1.0.0
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
    /// EQST,Equipment Status
    /// </summary>
    public enum EQST
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
