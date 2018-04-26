#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: SFCD
// Author: Ivan JL Zhang    Date: 2018/4/14 10:00:44    Version: 1.0.0
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
    /// Status Formatted Code
    /// </summary>
    public enum SFCD
    {
        /// <summary>
        /// 01: Equipment Status Request,
        /// </summary>
        EquipmentStatus = 01,
        /// <summary>
        /// 02: Port Status Request
        /// </summary>
        PortStatus = 02,
        /// <summary>
        /// 03: Operation mode Request
        /// </summary>
        OperationMode = 03,
        /// <summary>
        /// 04: Unit Status Request
        /// </summary>
        UnitStatus = 04,
        /// <summary>
        /// 05: Sub-Unit Status Request
        /// </summary>
        SubUnitStatus = 05,
        /// <summary>
        /// 06: Sub-Sub-Unit Status Request
        /// </summary>
        SSubUnitStatus = 06,
        /// <summary>
        /// 07: Mask Status Request
        /// </summary>
        MaskStatus = 07,
        /// <summary>
        /// 08: Material Status Request
        /// </summary>
        MaterialStatus = 08,
        /// <summary>
        /// 09: Sorter Job List Request
        /// </summary>
        SorterJobList = 09,
        /// <summary>
        /// 10: Crate Port Status Request
        /// </summary>
        CratePortStatus = 10,
        /// <summary>
        /// 11: Port load-request and Unloader-request report start
        /// </summary>
        PortLoad = 11,
        /// <summary>
        /// 12: Equipment Recycle Status Request
        /// </summary>
        EquipmentRecycleStatus = 12,
    }
}
