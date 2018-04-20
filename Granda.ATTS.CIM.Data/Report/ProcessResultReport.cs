#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: ProcessResultReport
// Author: Ivan JL Zhang    Date: 2018/4/13 16:25:18    Version: 1.0.0
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
    /// Process Result Data Report(Glass/Lot/Mask)
    /// </summary>
    public struct ProcessResultReport : IReport
    {
        /// <summary>
        /// CEID
        /// </summary>
        public PTYPE CEID { get; set; }
        /// <summary>
        /// Unit Id
        /// </summary>
        public string UNITID { get; set; }
        /// <summary>
        /// Sub Unit Id
        /// </summary>
        public string SUNITID { get; set; }
        /// <summary>
        /// Lot Id
        /// </summary>
        public string LOTID { get; set; }
        /// <summary>
        /// Cassette Id
        /// </summary>
        public string CSTID { get; set; }
        /// <summary>
        /// Glass Id
        /// </summary>
        public string GLSID { get; set; }
        /// <summary>
        /// Mask Id
        /// </summary>
        public string MASKID { get; set; }
        /// <summary>
        /// Operator Id
        /// </summary>
        public string OPERID { get; set; }
        /// <summary>
        /// Process Spec ID
        /// </summary>
        public string PRODID { get; set; }
        /// <summary>
        /// Process Program Id
        /// </summary>
        public string PPID { get; set; }
        /// <summary>
        /// Data Value List，
        /// 用于Glass/Mask is ended时需要对其赋值
        /// </summary>
        public DVNAMES DVNAMELIST { get; set; }
        /// <summary>
        /// Lot Process Time List, 
        /// 用于Lot Process is ended时需对其赋值
        /// </summary>
        public LOTPROCESSTIMES LOTPROCESSTIMELIST { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Item SecsItem
        {

            get
            {
                var itemList = new List<Item>();
                itemList.Add(A($"{(Int32)CEID}"));
                switch (CEID)
                {
                    case PTYPE.Glass:// Glass is ended
                        #region
                        itemList.Add(L(
                            A(UNITID ?? String.Empty),
                            A(SUNITID ?? String.Empty),
                            A(LOTID ?? String.Empty),
                            A(CSTID ?? String.Empty),
                            A(GLSID ?? String.Empty),
                            A(OPERID ?? String.Empty),
                            A(PRODID ?? String.Empty),
                            A(PPID ?? String.Empty),
                            DVNAMELIST.SecsItem
                            ));
                        #endregion
                        break;
                    case PTYPE.Lot:// lot is ended
                        #region
                        itemList.Add(L(
                            A(""),
                            A(""),
                            A(LOTID ?? String.Empty),
                            A(CSTID ?? String.Empty),
                            A(""),
                            A(OPERID ?? String.Empty),
                            A(PRODID ?? String.Empty),
                            A(PPID ?? String.Empty),
                            LOTPROCESSTIMELIST.SecsItem
                            ));
                        #endregion
                        break;
                    case PTYPE.Mask:// mask is ended
                        #region
                        itemList.Add(L(
                            A(UNITID ?? String.Empty),
                            A(SUNITID ?? String.Empty),
                            A(LOTID ?? String.Empty),
                            A(CSTID ?? String.Empty),
                            A(MASKID ?? String.Empty),
                            A(OPERID ?? String.Empty),
                            A(PRODID ?? String.Empty),
                            A(PPID ?? String.Empty),
                            DVNAMELIST.SecsItem
                            ));
                        #endregion
                        break;
                    default:
                        break;
                }
                return L(itemList);
            }
        }
    }
    /// <summary>
    /// Data Value List
    /// </summary>
    public class DVNAMES : IReport
    {
        /// <summary>
        /// Data Value Name
        /// </summary>
        public string DVNAME { get; set; }
        /// <summary>
        /// Site Name List
        /// </summary>
        public SITENAMES SITENAMELIST { get; set; }
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
                        A(item.DVNAME ?? String.Empty),
                        item.SITENAMELIST?.SecsItem
                        ));
                }
                return L(itemList);
            }
        }


        #region IList相关
        private DVNAMES[] _items;
        private static readonly DVNAMES[] emptyArray = new DVNAMES[0];
        private int _size = 0;
        /// <summary>
        /// ctor
        /// </summary>
        public DVNAMES()
        {
            _items = emptyArray;
        }
        /// <summary>
        /// 获取或设置指定索引处的元素。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DVNAMES this[int index]
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
        public void Add(DVNAMES item)
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
                        DVNAMES[] mewItems = new DVNAMES[value];
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
    /// Site Name List
    /// </summary>
    public class SITENAMES : IReport
    {
        /// <summary>
        /// Site Name
        /// </summary>
        public string SITENAME { get; set; }
        /// <summary>
        /// Data Value
        /// </summary>
        public string DV { get; set; }
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
                        A(item.SITENAME ?? String.Empty),
                       A(item.DV ?? String.Empty)
                        ));
                }
                return L(itemList);

            }
        }


        #region IList相关
        private SITENAMES[] _items;
        private static readonly SITENAMES[] emptyArray = new SITENAMES[0];
        private int _size = 0;
        /// <summary>
        /// ctor
        /// </summary>
        public SITENAMES()
        {
            _items = emptyArray;
        }
        /// <summary>
        /// 获取或设置指定索引处的元素。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public SITENAMES this[int index]
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
        public void Add(SITENAMES item)
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
                        SITENAMES[] mewItems = new SITENAMES[value];
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
    /// Lot Process Time List
    /// </summary>
    public class LOTPROCESSTIMES : IReport
    {
        /// <summary>
        /// Lot Process Time
        /// </summary>
        public string LOTPROCESSTIME { get; set; }
        /// <summary>
        ///  Lot Start or End Time List
        /// </summary>
        public LOTSTARTENDTIMES LOTSTARTENDTIMELIST { get; set; }
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
                        A(item.LOTPROCESSTIME ?? String.Empty),
                       item.LOTSTARTENDTIMELIST?.SecsItem
                        ));
                }
                return L(itemList);

            }
        }
        #region IList相关
        private LOTPROCESSTIMES[] _items;
        private static readonly LOTPROCESSTIMES[] emptyArray = new LOTPROCESSTIMES[0];
        private int _size = 0;
        /// <summary>
        /// ctor
        /// </summary>
        public LOTPROCESSTIMES()
        {
            _items = emptyArray;
        }
        /// <summary>
        /// 获取或设置指定索引处的元素。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public LOTPROCESSTIMES this[int index]
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
        public void Add(LOTPROCESSTIMES item)
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
                        LOTPROCESSTIMES[] mewItems = new LOTPROCESSTIMES[value];
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
    /// Lot Start or End Time List
    /// </summary>
    public class LOTSTARTENDTIMES : IReport
    {
        /// <summary>
        /// Lot Start or end Time
        /// </summary>
        public string LOTSTARTENDTIME { get; set; }
        /// <summary>
        /// Time
        /// </summary>
        public string TIME { get; set; }
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
                        A(item.LOTSTARTENDTIME ?? String.Empty),
                       A(item.TIME ?? String.Empty)));
                }
                return L(itemList);
            }
        }


        #region IList相关
        private LOTSTARTENDTIMES[] _items;
        private static readonly LOTSTARTENDTIMES[] emptyArray = new LOTSTARTENDTIMES[0];
        private int _size = 0;
        /// <summary>
        /// ctor
        /// </summary>
        public LOTSTARTENDTIMES()
        {
            _items = emptyArray;
        }
        /// <summary>
        /// 获取或设置指定索引处的元素。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public LOTSTARTENDTIMES this[int index]
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
        public void Add(LOTSTARTENDTIMES item)
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
                        LOTSTARTENDTIMES[] mewItems = new LOTSTARTENDTIMES[value];
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
