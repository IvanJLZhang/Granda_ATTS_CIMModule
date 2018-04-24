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
using System.Diagnostics.Contracts;
using Secs4Net;
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
                var itemList = new List<Item>();
                itemList.Add(A($"{DATAID}"));
                itemList.Add(A($"{CEID}"));
                itemList.Add(
                    L(
                        L(
                            A(RPTID.ToString()),
                            EquipmentStatus.SecsItem),
                        L(
                            A(RPTID1.ToString()),
                            ECIDLIST.SecsItem
                            )
                    ));
                return L(itemList);
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
                var itemList = new List<Item>();
                for (int index = 0; index < _size; index++)
                {
                    var item = _items[index];
                    itemList.Add(L(
                        A(item.ECID ?? String.Empty),
                        A(item.ECV ?? String.Empty)
                        ));
                }
                return L(itemList);
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
            if (_size == _items.Length) EnsureCapacity(_size + 1);
            this._items[this._size++] = item;
        }

        private const int _defaultCapacity = 4;
        private void EnsureCapacity(int min)
        {
            if (_items.Length < min)
            {
                int newCapacity = _items.Length == 0 ? _defaultCapacity : _items.Length * 2;
                if (newCapacity < min) newCapacity = min;
                Capacity = newCapacity;
            }
        }

        private int Capacity
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                return _items.Length;
            }
            set
            {
                if (value < _size)
                {
                    throw new ArgumentOutOfRangeException();
                }
                Contract.EndContractBlock();
                if (value != _items.Length)
                {
                    if (value > 0)
                    {
                        ECIDDatas[] mewItems = new ECIDDatas[value];
                        if (_size > 0)
                        {
                            Array.Copy(_items, 0, mewItems, 0, _size);
                        }
                        _items = mewItems;
                    }
                    else
                    {
                        _items = emptyArray;
                    }
                }
            }
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
