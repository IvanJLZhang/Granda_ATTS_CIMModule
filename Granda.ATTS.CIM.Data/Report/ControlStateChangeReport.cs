﻿#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: ControlStateChangeReport
// Author: Ivan JL Zhang    Date: 2018/4/12 14:10:31    Version: 1.0.0
// Description: 
//   
// 
// Revision History: 
// <Author>  		<Date>     	 	<Revision>  		<Modification>
// 	
//----------------------------------------------------------------------------*/
#endregion
using System.Collections.Generic;
using Secs4Net;
using static Secs4Net.Item;
namespace Granda.ATTS.CIM.Data.Report
{
    /// <summary>
    /// Control State Change Report
    /// </summary>
    public struct ControlStateChangeReport : IReport
    {
        /// <summary>
        /// 数据ID
        /// </summary>
        public int DATAID { get; private set; }
        /// <summary>
        /// Collected event ID
        /// </summary>
        public int CEID { get; set; }
        /// <summary>
        /// Report ID
        /// </summary>
        public int RPTID { get; set; }
        /// <summary>
        /// 设备的基本状态
        /// </summary>
        public EquipmentStatus EquipmentStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Item SecsItem
        {
            get
            {
                DATAID = 0;
                var itemList = new List<Item>();
                itemList.Add(A(DATAID.ToString()));
                itemList.Add(A(CEID.ToString()));
                itemList.Add(L(
                    L(
                        A(RPTID == 0 ? "100" : RPTID.ToString()),
                        this.EquipmentStatus.SecsItem
                        )
                    ));
                return L(itemList);
            }
        }
    }
}
