#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: EQSTCODE
// Author: Ivan JL Zhang    Date: 2018/4/11 14:53:50    Version: 1.0.0
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
using Granda.ATTS.CIM.Data.ENUM;
using Secs4Net;
using static Secs4Net.Item;
namespace Granda.ATTS.CIM.Data.Report
{
    /// <summary>
    /// Equipment status reason code（EQSTCODE）
    /// </summary>
    public struct EquipmentStatus : IReport
    {
        /// <summary>
        /// Control State
        /// </summary>
        public CRST CRST { get; set; }
        /// <summary>
        /// Equipment Status
        /// </summary>
        public CommonStatus EQST { get; set; }
        /// <summary>
        /// equipment status reason code
        /// </summary>
        public Int32 EQSTCODE { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Item SecsItem
        {
            get
            {
                var itemList = new List<Item>();
                itemList.Add(A(CRST.ToString()));
                itemList.Add(A(EQST.ToString()));
                itemList.Add(A(EQSTCODE.ToString()));
                return L(itemList);
            }
        }
        /// <summary>
        /// 重写toString方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {

            return base.ToString();
        }
    }
}
