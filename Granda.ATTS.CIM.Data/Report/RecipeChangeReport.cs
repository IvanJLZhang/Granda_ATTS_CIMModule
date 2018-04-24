#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: RecipeChangeReport
// Author: Ivan JL Zhang    Date: 2018/4/13 12:17:10    Version: 1.0.0
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
using Granda.ATTS.CIM.Data.ENUM;
using Secs4Net;
using static Secs4Net.Item;
namespace Granda.ATTS.CIM.Data.Report
{
    /// <summary>
    /// Process Program is changed(created/edited/deleted) by Operator
    /// </summary>
    public struct RecipeChangeReport : IReport
    {
        /// <summary>
        /// 数据ID
        /// </summary>
        public int DATAID { get; private set; }
        /// <summary>
        /// Collected event ID
        /// </summary>
        public int CEID { get; private set; }
        /// <summary>
        /// Report ID， 固定为100
        /// </summary>
        public int RPTID { get; private set; }
        /// <summary>
        /// 设备的基本状态
        /// </summary>
        public EquipmentStatus EquipmentStatus { get; set; }
        /// <summary>
        /// Report ID, 固定为401
        /// </summary>
        public int RPTID1 { get; private set; }
        /// <summary>
        /// Process Program ID
        /// </summary>
        public string PPID { get; set; }
        /// <summary>
        /// Process Program Type
        /// </summary>
        public PPTYPE PPTYPE { get; set; }
        /// <summary>
        /// Process Program Change Information
        /// </summary>
        public PPCINFO PPCINFO { get; set; }
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
                DATAID = 0;
                CEID = 401;
                RPTID = 100;
                RPTID1 = 401;
                var itemList = new List<Item>();
                itemList.Add(A(DATAID.ToString()));
                itemList.Add(A(CEID.ToString()));
                itemList.Add(L(
                    L(
                        A(RPTID.ToString()),
                        EquipmentStatus.SecsItem
                        ),
                    L(
                        A(RPTID1.ToString()),
                        L(
                             A(PPID ?? String.Empty),
                             A(PPTYPE.ToString()),
                             A($"{(Int32)PPCINFO}"),
                             A(LCTIME ?? String.Empty),
                             ProcessCommandList.SecsItem
                            )
                        )
                    ));
                return L(itemList);
            }
        }
    }
    /// <summary>
    /// process command list
    /// </summary>
    public class ProcessCommands : IReport
    {
        /// <summary>
        /// Command Code
        /// </summary>
        public string CCODE { get; set; }
        /// <summary>
        /// Recipe Step
        /// </summary>
        public string RCPSTEP { get; set; }
        /// <summary>
        /// Unit ID
        /// </summary>
        public string UNITID { get; set; }
        /// <summary>
        /// Sub Unit ID
        /// </summary>
        public string SUNITID { get; set; }
        /// <summary>
        /// Process paramter list
        /// </summary>
        public Parameters ParameterList { get; set; }
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
                         A(item.CCODE ?? String.Empty),
                         A(item.RCPSTEP ?? String.Empty),
                         A(item.UNITID ?? String.Empty),
                         A(item.SUNITID ?? String.Empty),
                         item.ParameterList.SecsItem
                         ));
                }
                return L(itemList);
            }

        }


        #region IList相关
        private ProcessCommands[] _items;
        private static readonly ProcessCommands[] emptyArray = new ProcessCommands[0];
        private int _size = 0;
        /// <summary>
        /// ctor
        /// </summary>
        public ProcessCommands()
        {
            _items = emptyArray;
        }
        /// <summary>
        /// 获取或设置指定索引处的元素。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ProcessCommands this[int index]
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
        public void Add(ProcessCommands item)
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
                        ProcessCommands[] mewItems = new ProcessCommands[value];
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
    /// <summary>
    /// Process parameter list
    /// </summary>
    public class Parameters : IReport
    {
        /// <summary>
        /// process parameter name
        /// </summary>
        public string PPARMNAME { get; set; }
        /// <summary>
        /// process parameter value
        /// </summary>
        public string PPARMVALUE { get; set; }
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
                            A(item.PPARMNAME ?? String.Empty),
                            A(item.PPARMVALUE ?? String.Empty)
                            ));
                }
                return L(itemList);
            }
        }


        #region IList相关
        private Parameters[] _items;
        private static readonly Parameters[] emptyArray = new Parameters[0];
        private int _size = 0;
        /// <summary>
        /// ctor
        /// </summary>
        public Parameters()
        {
            _items = emptyArray;
        }
        /// <summary>
        /// 获取或设置指定索引处的元素。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Parameters this[int index]
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
        public void Add(Parameters item)
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
                        Parameters[] mewItems = new Parameters[value];
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
