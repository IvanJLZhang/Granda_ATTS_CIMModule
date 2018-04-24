#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: CurrentEPPDRequestReport
// Author: Ivan JL Zhang    Date: 2018/4/13 11:48:30    Version: 1.0.0
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

namespace Granda.ATTS.CIM.Data.Message
{
    /// <summary>
    /// Current Equipment process program data request
    /// </summary>
    public struct CurrentEPPDRequest : IMessage
    {
        /// <summary>
        /// Unit Id
        /// </summary>
        public string UNITID { get; private set; }
        /// <summary>
        /// Process Program Type
        /// </summary>
        public PPTYPE PPTYPE { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Parse(Item item)
        {
            if (item.Items.Count == 2)
            {
                UNITID = item.Items[0].Format == SecsFormat.ASCII ? item.Items[0].GetString() : String.Empty;
                Enum.TryParse(item.Items[1].Format == SecsFormat.ASCII ? item.Items[1].GetString() : String.Empty, out PPTYPE pPTYPE);
                PPTYPE = pPTYPE;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(UNITID)}:{UNITID}, {nameof(this.PPTYPE)}: {PPTYPE.ToString()}";
        }
    }
    /// <summary>
    /// Current Equipment process program data request
    /// </summary>
    public struct CurrentEPPDReport : IReport
    {
        /// <summary>
        /// Unit Id
        /// </summary>
        public string UNITID { get; set; }
        /// <summary>
        /// Process Program Type
        /// </summary>
        public PPTYPE PPTYPE { get; set; }
        /// <summary>
        /// Process Program ID List
        /// </summary>
        public IList<string> PPIDLIST;
        /// <summary>
        /// 
        /// </summary>
        public Item SecsItem
        {
            get
            {
                var itemList = new List<Item>();
                itemList.Add(A(UNITID ?? String.Empty));
                itemList.Add(A(PPTYPE.ToString()));
                itemList.Add(L(
                    new Func<IList<string>, Item>((ppidList) =>
                    {
                        var itemList1 = new List<Item>();
                        for (int index = 0; index < ppidList.Count; index++)
                        {
                            itemList1.Add(A(ppidList[index]));
                        }
                        return L(itemList1);
                    })(PPIDLIST)
                    ));
                return L(itemList);
            }

        }
    }
}
