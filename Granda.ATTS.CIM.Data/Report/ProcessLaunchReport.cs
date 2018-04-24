#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: ProcessLaunchReport
// Author: Ivan JL Zhang    Date: 2018/4/12 15:38:40    Version: 1.0.0
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
using Secs4Net;
using static Secs4Net.Item;
namespace Granda.ATTS.CIM.Data.Report
{
    /// <summary>
    /// Remote Control: Process Report
    /// </summary>
    public struct ProcessLaunchReport : IReport
    {
        /// <summary>
        /// 内置信息的初始化构造方法
        /// </summary>
        /// <param name="dATAID"></param>
        /// <param name="cEID"></param>
        /// <param name="rPTID"></param>
        /// <param name="equipmentBaseInfo"></param>
        /// <param name="rPTID1"></param>
        public ProcessLaunchReport(int dATAID, int cEID, int rPTID, EquipmentBaseInfo equipmentBaseInfo, int rPTID1) : this()
        {
            DATAID = dATAID;
            CEID = cEID;
            RPTID = rPTID;
            EquipmentBaseInfo = equipmentBaseInfo;
            RPTID1 = rPTID1;
        }

        /// <summary>
        /// 数据ID
        /// </summary>
        public int DATAID { get; private set; }
        /// <summary>
        /// Collected event ID
        /// </summary>
        public int CEID { get; private set; }
        /// <summary>
        /// Report ID
        /// </summary>
        public int RPTID { get; private set; }
        /// <summary>
        /// 设备基本信息
        /// </summary>
        public EquipmentBaseInfo EquipmentBaseInfo { get; private set; }
        /// <summary>
        /// report id
        /// </summary>
        public int RPTID1 { get; private set; }
        /// <summary>
        /// Lot ID
        /// </summary>
        public string LOTID { get; set; }
        /// <summary>
        /// Port ID
        /// </summary>
        public string PTID { get; set; }
        /// <summary>
        /// Port Type
        /// </summary>
        public string PTTYPE { get; set; }
        /// <summary>
        /// Port Use Type
        /// </summary>
        public string PTUSETYPE { get; set; }
        /// <summary>
        /// Cassette ID
        /// </summary>
        public string CSTID { get; set; }
        /// <summary>
        /// Process Program ID or Recipe ID
        /// </summary>
        public string PPID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Item SecsItem
        {
            get
            {
                var itemList = new List<Item>();
                itemList.Add(A(DATAID.ToString()));
                itemList.Add(A(CEID.ToString()));
                itemList.Add(L(
                    L(
                        A(RPTID.ToString()),
                        EquipmentBaseInfo.SecsItem
                    ),
                    L(
                        A(RPTID1.ToString()),
                        L(
                            A(LOTID ?? String.Empty),
                            A(PTID ?? String.Empty),
                            A(PTTYPE ?? String.Empty),
                            A(PTUSETYPE ?? String.Empty),
                            A(CSTID ?? String.Empty),
                            A(PPID ?? String.Empty)
                            )
                        )));
                return L(itemList);
            }
        }
    }
}
