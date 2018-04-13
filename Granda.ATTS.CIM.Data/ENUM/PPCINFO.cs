#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: PPCINFO
// Author: Ivan JL Zhang    Date: 2018/4/13 14:42:48    Version: 1.0.0
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
    /// Process Program Change Information
    /// </summary>
    public enum PPCINFO
    {
        /// <summary>
        /// a new PPID is created and registered
        /// </summary>
        Created = 1,
        /// <summary>
        /// some parameters of a PPID are modified
        /// </summary>
        Modified,
        /// <summary>
        /// any PPID is deleted
        /// </summary>
        Deleted,
        /// <summary>
        /// equipment sets up any PPID which different from current PPID
        /// </summary>
        Changed,
    }
}
