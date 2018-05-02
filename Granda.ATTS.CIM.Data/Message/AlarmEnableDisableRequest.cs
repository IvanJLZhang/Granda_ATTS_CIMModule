#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: AlarmEnableDisableJob
// Author: Ivan JL Zhang    Date: 2018/4/13 10:11:31    Version: 1.0.0
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

namespace Granda.ATTS.CIM.Data.Message
{
    /// <summary>
    /// Alarm Management Scenario: Enable/Disable alarm job
    /// </summary>
    public struct AlarmEnableDisableRequest : IMessage
    {
        /// <summary>
        /// Alarm Enable/Disable
        /// </summary>
        public bool ALED { get; private set; }
        /// <summary>
        /// Unit ID
        /// </summary>
        public string UNITID { get; private set; }
        /// <summary>
        /// ALID List
        /// </summary>
        public IEnumerable<string> ALIDLIST;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public bool Parse(Item item)
        {
            if (item.Items.Count == 3)
            {
                ALED = item.Items[0].Format == SecsFormat.ASCII ? item.Items[0].GetString() == "0" : false;
                UNITID = item.Items[1].Format == SecsFormat.ASCII ? item.Items[1].GetString() : String.Empty;

                List<string> idlist = new List<string>();
                foreach (var it in item.Items[2].Items)
                {
                    idlist.Add(it.Format == SecsFormat.ASCII ? it.GetString() : String.Empty);
                }
                ALIDLIST = idlist;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string str = $"{nameof(ALED)}:{ALED}, {nameof(UNITID)}:{UNITID} \r\n";
            foreach (var item in ALIDLIST)
            {
                str += "ALID: " + item + "\r\n";
            }
            return str;
        }
    }
}
