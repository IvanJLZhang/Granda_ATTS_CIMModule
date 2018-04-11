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
using System.Linq;
using System.Text;
using Secs4Net;
using static Secs4Net.Item;
using static Granda.ATTS.CIM.Data.Helper;
namespace Granda.ATTS.CIM.Data.Report
{
    /// <summary>
    /// 在初始化场景中用到的，提供Equipment Model Type和Software revision code的结构
    /// </summary>
    public struct EquipmentBaseInfo : IDataItem, IMessage
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
                var stack = new Stack<List<Item>>();
                stack.Push(new List<Item>()
                {
                    A(MDLN ?? String.Empty),
                    A(SOFTREV ?? String.Empty),
                });

                return ParseItem(stack);
            }
        }

        public void Parse(Item item)
        {
            try
            {
                MDLN = item.Items[0].GetString();
                SOFTREV = item.Items[1].GetString();
            }
            catch (Exception)
            {

                //throw;
            }
        }
    }
}
