#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: EQTBaseInfo
// Author: Ivan JL Zhang    Date: 2018/4/11 12:13:13    Version: 1.0.0
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
using Granda.HSMS;
using static Granda.HSMS.Item;
namespace Granda.ATTS.CIM.Data.Report
{
    /// <summary>
    /// 在初始化场景中用到的，提供Equipment Model Type和Software revision code的结构
    /// </summary>
    public struct EquipmentBaseInfo : IReport, IMessage
    {
        /// <summary>
        /// Equipment Model Type
        /// Format: ASCII
        /// </summary>
        public string MDLN { get; set; }
        /// <summary>
        /// Software revision code
        /// Format: ASCII
        /// </summary>
        public string SOFTREV { get; set; }
        /// <summary>
        /// 转换为Item
        /// </summary>
        public Item SecsItem
        {
            get
            {
                var itemList = new List<Item>();
                itemList.Add(A(MDLN ?? String.Empty));
                itemList.Add(A(SOFTREV ?? String.Empty));
                return L(itemList);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Parse(Item item)
        {
            if (item.Items.Count == 2)
            {
                MDLN = item.Items[0].Format == SecsFormat.ASCII ? item.Items[0].GetString() : String.Empty;
                SOFTREV = item.Items[1].Format == SecsFormat.ASCII ? item.Items[1].GetString() : String.Empty;
            }
        }
    }
}
