#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: RemoteControlCommandJob
// Author: Ivan JL Zhang    Date: 2018/4/12 15:15:04    Version: 1.0.0
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
using Granda.ATTS.CIM.Data.ENUM;
using Secs4Net;

namespace Granda.ATTS.CIM.Data.Message
{
    public struct RemoteControlCommandJob : IMessage
    {
        /// <summary>
        /// Remote Control Command
        /// </summary>
        public RCMD RCMD { get; private set; }
        /// <summary>
        /// Port ID
        /// </summary>
        public string PTID { get; private set; }
        /// <summary>
        /// Cassette Number
        /// </summary>
        public string CSTID { get; private set; }
        /// <summary>
        /// Lot ID
        /// </summary>
        public string LOTID { get; private set; }
        /// <summary>
        /// Operator Call
        /// </summary>
        public string OPCALL { get; private set; }

        public void Parse(Item item)
        {
            if (item == null)
                return;
            if (item.Items.Count == 2)
            {
                Enum.TryParse(item.Items[0].GetString(), out RCMD rcmd);
                RCMD = rcmd;
                foreach (var it in item.Items[1].Items)
                {
                    if (it.Items.Count == 2)
                    {
                        if (it.Items[0].GetString().Equals(nameof(this.PTID)))
                        {
                            this.PTID = it.Items[1].GetString();
                        }
                        else if (it.Items[0].GetString().Equals(nameof(this.CSTID)))
                        {
                            this.CSTID = it.Items[1].GetString();
                        }
                        else if (it.Items[0].GetString().Equals(nameof(this.LOTID)))
                        {
                            this.LOTID = it.Items[1].GetString();
                        }
                        else if (it.Items[0].GetString().Equals(nameof(this.LOTID)))
                        {
                            this.OPCALL = it.Items[1].GetString();
                        }
                    }
                }
            }
        }
    }
}
