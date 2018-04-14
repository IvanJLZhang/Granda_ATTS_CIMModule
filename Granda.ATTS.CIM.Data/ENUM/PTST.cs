#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: PTST
// Author: Ivan JL Zhang    Date: 2018/4/14 10:40:01    Version: 1.0.0
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
    /// Port Status
    /// </summary>
    public enum PTST
    {
        /// <summary>
        /// 0: Load Request,
        /// </summary>
        LoadRequest = 0,
        /// <summary>
        /// 1: Pre-Load Complete,
        /// </summary>
        PreLoadComplete,
        /// <summary>
        /// 2: Load Complete,
        /// </summary>
        LoadComplete,
        /// <summary>
        /// 3: Unload Request,
        /// </summary>
        UnloadRequest,
        /// <summary>
        /// 4: Unload Complete,
        /// </summary>
        UnloadComplete,
        /// <summary>
        /// 5: Disable
        /// </summary>
        Disable,
    }
}
