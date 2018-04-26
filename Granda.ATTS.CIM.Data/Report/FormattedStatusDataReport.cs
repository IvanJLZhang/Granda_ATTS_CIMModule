#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: FormattedStatusDataReport
// Author: Ivan JL Zhang    Date: 2018/4/14 10:22:36    Version: 1.0.0
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
    /// Formatted Status Data Report
    /// </summary>
    public struct FormattedStatusDataReport : IReport
    {
        /// <summary>
        /// Status Formatted Code
        /// </summary>
        public SFCD SFCD { get; set; }
        /// <summary>
        /// Equipment Status
        /// </summary>
        public EquipmentStatus EquipmentStatus { get; set; }
        /// <summary>
        /// Port Status Data List
        /// </summary>
        public PortStatusDatas PortStatusDataList { get; set; }
        /// <summary>
        /// Unit Status Data List
        /// </summary>
        public UnitStatusDatas UnitStatusDataList { get; set; }
        /// <summary>
        /// Sub Unit Status Data List
        /// </summary>
        public UnitStatusDatas SubUnitStatusDataList { get; set; }
        /// <summary>
        /// Sub-Sub Unit Status Data List
        /// </summary>
        public UnitStatusDatas SSubUnitStatusDataList { get; set; }
        /// <summary>
        /// Mask Status Data List
        /// </summary>
        public MaskStatusDatas MaskStatusDataList { get; set; }
        /// <summary>
        /// Operation Mode
        /// <para/>
        /// Some EQP has variable operation mode code as assigned in constant.
        ///Whenever EQP change operation mode, it should report to Host.
        ///Ex) 01 or 02 or 03 or 04 …
        /// </summary>
        public string OPERMODE { get; set; }
        /// <summary>
        /// Operation Mode Description<para/>
        /// Description of Operation Mode
        /// (Ex)
        /// <para/>01: All processing mode
        /// <para/>02: CVD only
        /// <para/>03: Docking cleaner only
        /// </summary>
        public string OPERMODEDESC { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Item SecsItem
        {
            get
            {
                var itemlist = new List<Item>();
                itemlist.Add(A($"{(Int32)SFCD}"));
                switch (SFCD)
                {
                    case SFCD.EquipmentStatus:
                        itemlist.Add(EquipmentStatus.SecsItem);
                        break;
                    case SFCD.PortStatus:
                        if (PortStatusDataList != null) itemlist.Add(PortStatusDataList.SecsItem);
                        else itemlist.Add(L());
                        break;
                    case SFCD.UnitStatus:
                        if (UnitStatusDataList != null) itemlist.Add(UnitStatusDataList.SecsItem);
                        else itemlist.Add(L());
                        break;
                    case SFCD.MaskStatus:
                        if (MaskStatusDataList != null) itemlist.Add(MaskStatusDataList.SecsItem);
                        else itemlist.Add(L());
                        break;
                    case SFCD.OperationMode:
                        itemlist.Add(L(
                            A(OPERMODE ?? String.Empty),
                            A(OPERMODEDESC ?? String.Empty)
                            ));
                        break;
                    case SFCD.SubUnitStatus:
                        break;
                    case SFCD.MaterialStatus:
                        break;
                    case SFCD.SorterJobList:
                        break;
                    case SFCD.CratePortStatus:
                        break;
                    case SFCD.PortLoad:
                        break;
                    case SFCD.EquipmentRecycleStatus:
                        break;
                    default:
                        break;
                }
                return L(itemlist);
            }
        }
    }

    #region formatted status data class
    /// <summary>
    /// Port Status Data
    /// </summary>
    public class PortStatusDatas : IReport
    {
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
        /// Transfer Mode
        /// </summary>
        public TRSMODE TRSMODE { get; set; }
        /// <summary>
        /// Port Status
        /// </summary>
        public PTST PTST { get; set; }
        /// <summary>
        /// Cassette ID
        /// </summary>
        public string CSTID { get; set; }

        /// <summary>
        /// Lot ID
        /// </summary>
        public string LOTID { get; set; }
        /// <summary>
        /// Process Program ID or Recipe ID
        /// </summary>
        public string PPID { get; set; }
        /// <summary>
        /// Slot Information
        /// </summary>
        public SLOTINFO SLOTINFO { get; set; }
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
                    var data = _items[index];
                    itemList.Add(L(
                        A(data.PTID),
                        A(data.PTTYPE),
                        A(data.PTUSETYPE),
                        A($"{(Int32)data.TRSMODE}"),
                        A($"{(Int32)data.PTST}"),
                        A(data.CSTID),
                        A(data.LOTID),
                        A(data.PPID),
                        A($"{(Int32)data.SLOTINFO}")
                        ));
                }
                return L(itemList);
            }
        }

        #region IList相关
        private PortStatusDatas[] _items;
        private static readonly PortStatusDatas[] emptyArray = new PortStatusDatas[0];
        private int _size = 0;
        /// <summary>
        /// ctor
        /// </summary>
        public PortStatusDatas()
        {
            _items = emptyArray;
        }
        /// <summary>
        /// 获取或设置指定索引处的元素。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public PortStatusDatas this[int index]
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
        public void Add(PortStatusDatas item)
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
                        PortStatusDatas[] mewItems = new PortStatusDatas[value];
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
    /// Operation Status
    /// </summary>
    public struct OperationStatus
    {
        /// <summary>
        /// Operation Mode
        /// <para>Some EQP has variable operation mode code as assigned in constant.
        ///        Whenever EQP change operation mode, it should report to Host.
        ///        Ex) 01 or 02 or 03 or 04 …
        ///</para>
        /// </summary>
        public int OPERMODE { get; set; }
        /// <summary>
        /// Operation Mode Description
        /// <para>Description of Operation Mode
        ///        (Ex)
        ///01: All processing mode
        ///02: CVD only
        ///03: Docking cleaner only
        /// </para>
        /// </summary>
        public int OPERMODEDESC { get; set; }
    }

    /// <summary>
    /// Unit Status
    /// </summary>
    public class UnitStatusDatas : IReport
    {
        /// <summary>
        /// Unit Id
        /// </summary>
        public string UNITID { get; set; }
        /// <summary>
        /// Unit Status
        /// </summary>
        public CommonStatus UNITST { get; set; }
        /// <summary>
        /// Unit Status reason code
        /// </summary>
        public string UNITSTCODE { get; set; }
        /// <summary>
        /// Unit Slot Number List
        /// </summary>
        public USLOTNOS USLOTNOLIST { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Item SecsItem
        {
            get
            {
                if (_size <= 0)
                    return null;
                var itemList = new List<Item>();
                for (int index = 0; index < _size; index++)
                {
                    var item = _items[index];
                    itemList.Add(L(
                        A(item.UNITID ?? String.Empty),
                        A(item.UNITST.ToString()),
                        A(item.UNITSTCODE ?? String.Empty),
                        USLOTNOLIST == null ? L() : USLOTNOLIST.SecsItem
                        ));
                }
                return L(itemList);
            }
        }

        #region IList相关
        private UnitStatusDatas[] _items;
        private static readonly UnitStatusDatas[] emptyArray = new UnitStatusDatas[0];
        private int _size = 0;
        /// <summary>
        /// ctor
        /// </summary>
        public UnitStatusDatas()
        {
            _items = emptyArray;
        }
        /// <summary>
        /// 获取或设置指定索引处的元素。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public UnitStatusDatas this[int index]
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
        public void Add(UnitStatusDatas item)
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
                        UnitStatusDatas[] mewItems = new UnitStatusDatas[value];
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
    /// Unit slot number
    /// </summary>
    public class USLOTNOS : IReport
    {
        /// <summary>
        /// Unit Slot Number
        /// </summary>
        public string USLOTNO { get; set; }
        /// <summary>
        /// Glass ID
        /// </summary>
        public string GLSID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Item SecsItem
        {
            get
            {
                if (_size <= 0)
                    return null;
                var itemList = new List<Item>();
                for (int index = 0; index < _size; index++)
                {
                    var uslotno = _items[index];
                    itemList.Add(L(
                        A(uslotno.USLOTNO ?? String.Empty),
                        A(uslotno.GLSID ?? String.Empty)
                        ));
                }
                return L(itemList);
            }
        }

        #region IList相关
        private USLOTNOS[] _items;
        private static readonly USLOTNOS[] emptyArray = new USLOTNOS[0];
        private int _size = 0;
        /// <summary>
        /// ctor
        /// </summary>
        public USLOTNOS()
        {
            _items = emptyArray;
        }
        /// <summary>
        /// 获取或设置指定索引处的元素。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public USLOTNOS this[int index]
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
        public void Add(USLOTNOS item)
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
                        USLOTNOS[] mewItems = new USLOTNOS[value];
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
    /// Mask Status data
    /// </summary>
    public class MaskStatusDatas : IReport
    {
        /// <summary>
        /// Unit ID
        /// </summary>
        public string UNITID { get; set; }
        /// <summary>
        /// Mask Status List
        /// </summary>
        public MaskStatuss MaskStatusList { get; set; }
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
                    if (item.MaskStatusList == null)
                    {
                        itemList.Add(L(
                                    A(item.UNITID ?? String.Empty)));
                    }
                    else
                    {
                        itemList.Add(L(
                                    A(item.UNITID ?? String.Empty),
                                    item.MaskStatusList.SecsItem
                                    ));
                    }
                }
                return L(itemList);

            }
        }

        #region IList相关
        private MaskStatusDatas[] _items;
        private static readonly MaskStatusDatas[] emptyArray = new MaskStatusDatas[0];
        private int _size = 0;
        /// <summary>
        /// ctor
        /// </summary>
        public MaskStatusDatas()
        {
            _items = emptyArray;
        }
        /// <summary>
        /// 获取或设置指定索引处的元素。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public MaskStatusDatas this[int index]
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
        public void Add(MaskStatusDatas item)
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
                        MaskStatusDatas[] mewItems = new MaskStatusDatas[value];
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
    /// Mask Status
    /// </summary>
    public class MaskStatuss : IReport
    {
        /// <summary>
        /// Mask Id
        /// </summary>
        public string MASKID { get; set; }
        /// <summary>
        /// Mask Status
        /// </summary>
        public string MASKST { get; set; }
        /// <summary>
        /// Mask Used Count
        /// </summary>
        public string MASKUSECNT { get; set; }
        /// <summary>
        /// Sub Unit mask status list
        /// </summary>
        public MaskStatusDatas SUNITMaskStatusList { get; set; }
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

                var stack = new Stack<List<Item>>();
                for (int index = 0; index < _size; index++)
                {
                    var item = _items[index];
                    if (item.SUNITMaskStatusList == null)
                    {
                        itemList.Add(L(
                             A(item.MASKID),
                             A(item.MASKST),
                             A(item.MASKUSECNT)));
                    }
                    else
                    {
                        itemList.Add(L(
                              A(item.MASKID),
                              A(item.MASKST),
                              A(item.MASKUSECNT),
                              item.SUNITMaskStatusList.SecsItem));
                    }
                }
                return L(itemList);
            }
        }

        #region IList相关
        private MaskStatuss[] _items;
        private static readonly MaskStatuss[] emptyArray = new MaskStatuss[0];
        private int _size = 0;
        /// <summary>
        /// ctor
        /// </summary>
        public MaskStatuss()
        {
            _items = emptyArray;
        }
        /// <summary>
        /// 获取或设置指定索引处的元素。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public MaskStatuss this[int index]
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
        public void Add(MaskStatuss item)
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
                        MaskStatuss[] mewItems = new MaskStatuss[value];
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
    #endregion

}
