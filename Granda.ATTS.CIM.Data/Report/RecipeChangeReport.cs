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
using System.Collections.Generic;
using Granda.ATTS.CIM.Data.ENUM;
using Secs4Net;
using static Granda.ATTS.CIM.Data.Helper;
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

                var stack = new Stack<List<Item>>();
                stack.Push(new List<Item>() {
                    A(DATAID.ToString()),
                    A(CEID.ToString()),
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
                stack.Push(new List<Item>() {
                    A(PPID),
                    A(PPTYPE.ToString()),
                    A($"{PPCINFO}"),
                    A(LCTIME),

                });
                stack.Push(new List<Item>() { });
                for (int index = 0; index < ProcessCommandList.Count; index++)
                {
                    var processCommand = ProcessCommandList[index];
                    stack.Push(new List<Item>() {
                        A(processCommand.CCODE),
                        A(processCommand.RCPSTEP),
                        A(processCommand.UNITID),
                        A(processCommand.SUNITID),
                    });
                    stack.Push(new List<Item>() { });
                    for (int indey = 0; indey < processCommand.ParameterList.Count; indey++)
                    {
                        var parmaeter = processCommand.ParameterList[indey];
                        stack.Push(new List<Item>() {
                            A(parmaeter.PPARMNAME),
                            A(parmaeter.PPARMVALUE),
                        });
                    }
                }
                return ParseItem(stack);
            }
        }
    }
    /// <summary>
    /// process command list
    /// </summary>
    public class ProcessCommands
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
        public void Add(ProcessCommands item)
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
    /// Process parameter list
    /// </summary>
    public class Parameters
    {
        /// <summary>
        /// process parameter name
        /// </summary>
        public string PPARMNAME { get; set; }
        /// <summary>
        /// process parameter value
        /// </summary>
        public string PPARMVALUE { get; set; }

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
        public void Add(Parameters item)
        {
            this._items[this._size++] = item;
        }

        /// <summary>
        /// 将新的item添加至列表末尾处
        /// </summary>
        public void Add(string paramName, string paramValue)
        {
            var item = new Parameters() { PPARMNAME = paramName, PPARMVALUE = paramValue };
            this._items[this._size++] = item;
        }
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
