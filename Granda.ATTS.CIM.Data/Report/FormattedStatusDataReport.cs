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
using System.Collections.Generic;
using Granda.ATTS.CIM.Data.ENUM;
using Secs4Net;
using static Granda.ATTS.CIM.Data.Helper;
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
        /// 
        /// </summary>
        public Item SecsItem
        {
            get
            {
                var stack = new Stack<List<Item>>();
                stack.Push(new List<Item>()
                {
                    A($"{SFCD}"),
                });
                switch (SFCD)
                {
                    case SFCD.EquipmentStatus:
                        stack.Push(new List<Item>(EquipmentStatus.SecsItem.Items));
                        break;
                    case SFCD.PortStatus:
                        stack.Push(new List<Item>(PortStatusDataList.SecsItem.Items));
                        break;
                    case SFCD.UnitStatus:
                        stack.Push(new List<Item>(UnitStatusDataList.SecsItem.Items));
                        break;
                    case SFCD.MaskStatus:
                        stack.Push(new List<Item>(MaskStatusDataList.SecsItem.Items));
                        break;
                    case SFCD.OperationMode:
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

                return ParseItem(stack);
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
                var stack = new Stack<List<Item>>();
                //stack.Push(new List<Item>());
                for (int index = 0; index < _size; index++)
                {
                    var data = _items[index];
                    stack.Push(new List<Item>() {
                        A(data.PTID),
                        A(data.PTTYPE),
                        A(data.PTUSETYPE),
                        A($"{data.TRSMODE}"),
                        A($"{data.PTST}"),
                        A(data.CSTID),
                        A(data.LOTID),
                        A(data.PPID),
                        A($"{data.SLOTINFO}"),
                    });
                }

                return ParseItem(stack);
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
        public void Add(PortStatusDatas item)
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
                var stack = new Stack<List<Item>>();
                for (int index = 0; index < _size; index++)
                {
                    var item = _items[index];
                    stack.Push(new List<Item>()
                    {
                        A(item.UNITID),
                        A(item.UNITST.ToString()),
                        A(item.UNITSTCODE),
                    });
                    stack.Push(new List<Item>(USLOTNOLIST.SecsItem.Items));
                }

                return ParseItem(stack);
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
        public void Add(UnitStatusDatas item)
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
                var stack = new Stack<List<Item>>();
                for (int index = 0; index < _size; index++)
                {
                    var uslotno = _items[index];
                    stack.Push(new List<Item>()
                    {
                        A(uslotno.USLOTNO),
                        A(uslotno.GLSID),
                    });
                }
                return ParseItem(stack);
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
        public void Add(USLOTNOS item)
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
                var stack = new Stack<List<Item>>();
                for (int index = 0; index < _size; index++)
                {
                    var item = _items[index];
                    stack.Push(new List<Item>()
                    {
                        A(UNITID),
                    });
                    if (MaskStatusList != null && MaskStatusList.Count > 0)
                    {
                        stack.Push(new List<Item>(MaskStatusList.SecsItem.Items));
                    }
                }
                return ParseItem(stack);

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
        public void Add(MaskStatusDatas item)
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
                var stack = new Stack<List<Item>>();
                for (int index = 0; index < _size; index++)
                {
                    var item = _items[index];
                    stack.Push(new List<Item>()
                    {
                        A(item.MASKID),
                        A (item.MASKST),
                        A(item.MASKUSECNT),
                    });
                    if (SUNITMaskStatusList != null && SUNITMaskStatusList.Count > 0)
                    {
                        stack.Push(new List<Item>(SUNITMaskStatusList.SecsItem.Items));
                    }
                }
                return ParseItem(stack);
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
        public void Add(MaskStatuss item)
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
    #endregion

}
