#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: Helper
// Author: Ivan JL Zhang    Date: 2018/4/11 12:22:31    Version: 1.0.0
// Description: 
//   
// 
// Revision History: 
// <Author>  		<Date>     	 	<Revision>  		<Modification>
// 	
//----------------------------------------------------------------------------*/
#endregion
using Secs4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Secs4Net.Item;
namespace Granda.ATTS.CIM.Data
{
    internal abstract class Helper
    {
        public static Item ParseItem(Stack<List<Item>> stack)
        {
            Item rootItem = null;
            do
            {
                var itemList = stack.Pop();
                var item = itemList.Count > 0 ? L(itemList) : L();
                if (stack.Count > 0)
                {
                    stack.Peek().Add(item);
                }
                else
                {
                    rootItem = item;
                }
            } while (stack.Count > 0);

            return rootItem;
        }
    }
}
