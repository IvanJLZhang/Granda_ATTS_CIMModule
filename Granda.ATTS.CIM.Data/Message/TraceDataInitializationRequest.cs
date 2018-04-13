#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: TraceDataInitializationRequest
// Author: Ivan JL Zhang    Date: 2018/4/13 17:26:04    Version: 1.0.0
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

namespace Granda.ATTS.CIM.Data.Message
{
    /// <summary>
    /// Host requests Trace Data Initialization
    /// </summary>
    public struct TraceDataInitializationRequest : IMessage
    {
        /// <summary>
        /// Trace Id
        /// </summary>
        public string TRID { get; private set; }
        /// <summary>
        /// Data Sample Period
        /// Time Format: hhmmss
        /// </summary>
        public string DSPER { get; private set; }
        /// <summary>
        /// Total Samples to be made
        /// The maximum number of samples that this Trace Report will perform.
        ///-1 means infinite count.
        /// </summary>
        public string TOTSMP { get; private set; }
        /// <summary>
        /// Reporting Group Size.
        /// ex:
        /// <para>DSPER = 3 Seconds, REPGSZ = 1: Report S6F1 (1 group) every 3 seconds.</para>
        /// <para>DSPER = 3 Seconds, REPGSZ = 2: Report S6F1(2 group) every 6 seconds.</para>
        /// </summary>
        public string REPGSZ { get; private set; }
        /// <summary>
        /// Status Variable ID
        /// <para>Status variables may include any parameter that can be sampled in time such as temperature or quantity of a consumable.</para>
        /// </summary>
        public IEnumerable<string> SVIDList;
        public void Parse(Item item)
        {
            if (item.Items.Count == 5)
            {
                TRID = item.Items[0].Format == SecsFormat.ASCII ? item.Items[0].GetString() : String.Empty;
                DSPER = item.Items[1].Format == SecsFormat.ASCII ? item.Items[0].GetString() : String.Empty;
                TOTSMP = item.Items[2].Format == SecsFormat.ASCII ? item.Items[0].GetString() : String.Empty;
                REPGSZ = item.Items[3].Format == SecsFormat.ASCII ? item.Items[0].GetString() : String.Empty;

                List<string> svidList = new List<string>();
                if (item.Items[4].Format == SecsFormat.List)
                {
                    for (int index = 0; index < item.Items[4].Items.Count; index++)
                    {
                        svidList.Add(item.Items[4].Items[index].Format == SecsFormat.ASCII ? item.Items[4].Items[index].GetString() : String.Empty);
                    }
                }

                SVIDList = svidList;
            }
        }
    }
}
