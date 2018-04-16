#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: EquipmentConstantChangeReport
// Author: Ivan JL Zhang    Date: 2018/4/14 13:59:15    Version: 1.0.0
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
using static Granda.ATTS.CIM.Data.Helper;
using static Secs4Net.Item;
namespace Granda.ATTS.CIM.Data.Report
{
    /// <summary>
    /// Equipment Constants Change by Operator
    /// </summary>
    public struct EquipmentConstantChangeReport : IReport
    {
        /// <summary>
        /// DataID
        /// </summary>
        public int DATAID { get; private set; }
        /// <summary>
        /// CEID
        /// </summary>
        public int CEID { get; private set; }
        /// <summary>
        /// Report ID
        /// </summary>
        public int RPTID { get; private set; }
        /// <summary>
        /// Equipment Status
        /// </summary>
        public EquipmentStatus EquipmentStatus { get; set; }

        /// <summary>
        /// Report ID
        /// </summary>
        public int RPTID1 { get; private set; }
        /// <summary>
        /// Equipment Constant ID
        /// </summary>
        public ECIDDatas ECIDLIST { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Item SecsItem
        {
            get
            {
                DATAID = 0;
                CEID = 109;
                RPTID = 100;
                RPTID1 = 109;
                var stack = new Stack<List<Item>>();
                stack.Push(new List<Item>() {
                    A($"{DATAID}"),
                    A($"{CEID}"),
                });
                stack.Push(new List<Item>());
                stack.Push(new List<Item>()
                {
                    A(RPTID.ToString()),
                });
                stack.Push(new List<Item>(EquipmentStatus.SecsItem.Items));
                stack.Push(new List<Item>() {
                    A(RPTID1.ToString()),
                });
                stack.Push(new List<Item>());
                stack.Push(new List<Item>(ECIDLIST.SecsItem.Items));

                return ParseItem(stack);
            }
        }
    }
    /// <summary>
    /// ECID data
    /// </summary>
    public class ECIDDatas : IReport
    {
        /// <summary>
        /// Equipment Constant ID
        /// </summary>
        public string ECID { get; set; }
        /// <summary>
        /// Equipment Constant Value
        /// </summary>
        public string ECV { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Item SecsItem
        {
            get
            {
                if (_size == 0)
                    return null;
                var stack = new Stack<List<Item>>();
                for (int index = 0; index < _size; index++)
                {
                    var item = _items[index];
                    stack.Push(new List<Item>()
                    {
                        A(item.ECID ?? String.Empty),
                        A(item.ECV ?? String.Empty),
                    });
                }
                return ParseItem(stack);
            }
        }

        #region IList相关
        private ECIDDatas[] _items;
        private static readonly ECIDDatas[] emptyArray = new ECIDDatas[0];
        private int _size = 0;
        /// <summary>
        /// ctor
        /// </summary>
        public ECIDDatas()
        {
            _items = emptyArray;
        }
        /// <summary>
        /// 获取或设置指定索引处的元素。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ECIDDatas this[int index]
        {
            get
            {
                return _items[index];
            }
            set
            {
                _items[index] = value;
            }
        }


        /// <summary>
        /// List的长度
        /// </summary>
        public int Count { get => _size; }

        /// <summary>
        /// 将新的item添加至列表末尾处
        /// </summary>
        /// <param name="item"></param>
        public void Add(ECIDDatas item)
        {
            this._items[this._size++] = item;
        }

        ///// <summary>
        ///// 将新的item添加至列表末尾处
        ///// </summary>
        //public void Add(string paramName, string paramValue)
        //{
        //    var item = new Parameters() { PPARMNAME = paramName, PPARMVALUE = paramValue };
        //    this._items[this._size++] = item;
        //}
        /// <summary>
        /// 清空列表
        /// </summary>
        public void Clear()
        {
            this._items = emptyArray;
            this._size = 0;
        }
        #endregion
    }
}
