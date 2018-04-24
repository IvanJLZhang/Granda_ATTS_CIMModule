#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: FormattedProcessProgramRequest
// Author: Ivan JL Zhang    Date: 2018/4/13 15:17:00    Version: 1.0.0
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
using Granda.ATTS.CIM.Data.Report;
using Secs4Net;
using static Secs4Net.Item;

namespace Granda.ATTS.CIM.Data.Message
{
    /// <summary>
    /// Formatted Process Program Request
    /// </summary>
    public struct FormattedProcessProgramRequest : IMessage
    {
        /// <summary>
        /// Process Program ID
        /// </summary>
        public string PPID { get; private set; }
        /// <summary>
        /// Unit ID
        /// </summary>
        public string UNITID { get; private set; }
        /// <summary>
        /// Sub Unit ID
        /// </summary>
        public string SUNITID { get; private set; }
        /// <summary>
        /// Sub-Sub Unit ID
        /// </summary>
        public string SSUNITID { get; private set; }
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
            if (item.Items.Count == 5)
            {
                PPID = item.Items[0].Format == SecsFormat.ASCII ? item.Items[0].GetString() : String.Empty;
                UNITID = item.Items[1].Format == SecsFormat.ASCII ? item.Items[1].GetString() : String.Empty;
                SUNITID = item.Items[2].Format == SecsFormat.ASCII ? item.Items[2].GetString() : String.Empty;
                SSUNITID = item.Items[3].Format == SecsFormat.ASCII ? item.Items[3].GetString() : String.Empty;

                Enum.TryParse(item.Items[4].Format == SecsFormat.ASCII ? item.Items[4].GetString() : String.Empty, out PPTYPE pPTYPE);
                this.PPTYPE = pPTYPE;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $@"{nameof(PPID)}:{PPID}, 
{nameof(PPID)}:{PPID}, 
{nameof(UNITID)}:{UNITID},
{nameof(SUNITID)}:{SUNITID},
{nameof(SSUNITID)}:{SSUNITID},
{nameof(PPTYPE)}:{PPTYPE.ToString()},
";
        }
    }
    /// <summary>
    /// Formatted Process Program Data
    /// </summary>
    public struct FormattedProcessProgramReport : IReport
    {
        /// <summary>
        /// Process Program ID
        /// </summary>
        public string PPID { get; set; }
        /// <summary>
        /// Process Program Type
        /// </summary>
        public PPTYPE PPTYPE { get; set; }
        /// <summary>
        /// Equipment base info
        /// </summary>
        public EquipmentBaseInfo EquipmentBaseInfo { get; set; }

        /// <summary>
        /// Local Time
        /// </summary>
        public string LCTIME { get; set; }

        /// <summary>
        /// Process Command list
        /// </summary>
        public ProcessCommands ProcessCommandList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Item SecsItem
        {
            get
            {
                var itemList = new List<Item>();
                itemList.Add(A(PPID ?? String.Empty));
                itemList.Add(A(PPTYPE.ToString()));
                itemList.Add(A(EquipmentBaseInfo.MDLN ?? String.Empty));
                itemList.Add(A(EquipmentBaseInfo.SOFTREV ?? String.Empty));
                itemList.Add(A(LCTIME ?? String.Empty));
                itemList.Add(ProcessCommandList.SecsItem);
                return L(itemList);
            }
        }
    }
}
