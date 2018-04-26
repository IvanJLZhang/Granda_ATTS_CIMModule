#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: CurrentAlarmListJob
// Author: Ivan JL Zhang    Date: 2018/4/13 10:44:01    Version: 1.0.0
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
using Granda.HSMS;
using static Granda.HSMS.Item;
namespace Granda.ATTS.CIM.Data.Message
{
    /// <summary>
    /// Current Alarm Set List Request
    /// </summary>
    public struct CurrentAlarmListRequest : IMessage
    {
        /// <summary>
        /// UNITID List
        /// </summary>
        public IEnumerable<string> UNITIDLIST;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Parse(Item item)
        {
            List<string> list = new List<string>();
            foreach (var ite in item.Items)
            {
                if (ite.Format == SecsFormat.List)
                {
                    foreach (var it in ite.Items)
                    {
                        if (it.Format == SecsFormat.ASCII)
                            list.Add(it.GetString());
                    }
                }
                else if (ite.Format == SecsFormat.ASCII)
                    list.Add(ite.GetString());
            }
            UNITIDLIST = list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string str = string.Empty;
            foreach (var item in UNITIDLIST)
            {
                str += "UNITID: " + item + "\r\n";
            }
            return str;
        }
    }
    /// <summary>
    /// Current Alarm Set List Data
    /// </summary>
    public class CurrentAlarmListReport : IReport
    {
        /// <summary>
        /// Unit ID
        /// </summary>
        public string UNITID { get; set; }
        /// <summary>
        /// ALID List
        /// </summary>
        public IEnumerable<string> ALIDLIST;
        /// <summary>
        /// 
        /// </summary>
        public Item SecsItem
        {
            get
            {
                var itemList = new List<Item>();
                for (int index = 0; index < _size; index++)
                {
                    var item = _items[index];
                    itemList.Add(L(
                        A(item.UNITID ?? String.Empty),
                        new Func<Item>(() =>
                        {
                            var itemList1 = new List<Item>();
                            foreach (var ALID in item.ALIDLIST)
                            {
                                itemList1.Add(A(ALID));
                            }
                            return L(itemList1);
                        })()
                        ));
                }
                return L(itemList);
            }
        }

        #region IList相关
        private CurrentAlarmListReport[] _items;
        private static readonly CurrentAlarmListReport[] emptyArray = new CurrentAlarmListReport[0];
        private int _size = 0;
        /// <summary>
        /// ctor
        /// </summary>
        public CurrentAlarmListReport()
        {
            _items = emptyArray;
        }
        /// <summary>
        /// 获取或设置指定索引处的元素。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public CurrentAlarmListReport this[int index]
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
        public void Add(CurrentAlarmListReport item)
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
                        CurrentAlarmListReport[] mewItems = new CurrentAlarmListReport[value];
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
