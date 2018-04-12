#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: EquipmentInfo
// Author: Ivan JL Zhang    Date: 2018/4/12 9:59:34    Version: 1.0.0
// Description: 
//   
// 
// Revision History: 
// <Author>  		<Date>     	 	<Revision>  		<Modification>
// 	
//----------------------------------------------------------------------------*/
#endregion
using Granda.ATTS.CIM.Data.ENUM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granda.ATTS.CIM.Data.Report
{
    /// <summary>
    /// 保存描述设备当前基本信息以及状态信息的结构体
    /// </summary>
    public struct EquipmentInfo
    {
        public EquipmentBaseInfo EquipmentBase
        {
            get
            {
                return new EquipmentBaseInfo()
                {
                    MDLN = MDLN,
                    SOFTREV = SOFTREV
                };
            }
        }
        public EquipmentStatus EquipmentStatus
        {
            get
            {
                return new EquipmentStatus()
                {
                    CRST = CRST,
                    EQST = EQST,
                    EQSTCODE = EQSTCODE
                };
            }
        }
        /// <summary>
        /// Equipment Model Type
        /// Format: ASCII
        /// </summary>
        public string MDLN
        {
            get; set;
        }
        /// <summary>
        /// Software revision code
        /// Format: ASCII
        /// </summary>
        public string SOFTREV
        {
            get; set;
        }

        /// <summary>
        /// Control State
        /// </summary>
        public CRST CRST
        {
            get; set;
        }
        /// <summary>
        /// Equipment Status
        /// </summary>
        public EQST EQST
        {
            get; set;
        }
        /// <summary>
        /// equipment status reason code
        /// </summary>
        public Int32 EQSTCODE
        {
            get; set;
        }
    }
}
