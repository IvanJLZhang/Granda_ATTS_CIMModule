#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: EQSTCODE
// Author: Ivan JL Zhang    Date: 2018/4/11 14:53:50    Version: 1.0.0
// Description: 
//   
// 
// Revision History: 
// <Author>  		<Date>     	 	<Revision>  		<Modification>
// 	
//----------------------------------------------------------------------------*/
#endregion
using Granda.ATTS.CIM.Data.ENUM;
using Secs4Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Secs4Net.Item;
using static Granda.ATTS.CIM.Data.Helper;
namespace Granda.ATTS.CIM.Data.Report
{
    /// <summary>
    /// Equipment status reason code（EQSTCODE），需与EQSTCODE文件在同一目录下共同使用
    /// </summary>
    public class EquipmentStatus : IDataItem
    {
        /// <summary>
        /// Control State
        /// </summary>
        public ControlState CRST { get; set; }
        /// <summary>
        /// Equipment Status
        /// </summary>
        public EEquipmentStatus EQST { get; set; }
        /// <summary>
        /// equipment status reason code
        /// </summary>
        public Int32 EQSTCODE { get; set; }

        private static Dictionary<EEquipmentStatus, Dictionary<int, string>> _eqstcodelist = null;
        /// <summary>
        /// 系统内置的equipment status reason code列表
        /// 作为参考以及校验用
        /// </summary>
        public static Dictionary<EEquipmentStatus, Dictionary<int, string>> EQSTCODELIST
        {
            get
            {
                if (_eqstcodelist != null)
                    return _eqstcodelist;
                _eqstcodelist = new Dictionary<EEquipmentStatus, Dictionary<int, string>>();
                using (var filestream = File.OpenRead("EQSTCODE"))
                {
                    EEquipmentStatus key = EEquipmentStatus.I;
                    StreamReader reader = new StreamReader(filestream);
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (!string.IsNullOrEmpty(line) && line.Contains("#EQST"))
                        {
                            if (line.Contains("MAINT"))
                            {
                                key = EEquipmentStatus.M;
                                _eqstcodelist.Add(key, new Dictionary<int, string>());
                            }
                            else if (line.Contains("DOWN"))
                            {
                                key = EEquipmentStatus.D;
                                _eqstcodelist.Add(key, new Dictionary<int, string>());
                            }
                            else if (line.Contains("PAUSE"))
                            {
                                key = EEquipmentStatus.P;
                                _eqstcodelist.Add(key, new Dictionary<int, string>());
                            }
                            else if (line.Contains("IDLE"))
                            {
                                key = EEquipmentStatus.I;
                                _eqstcodelist.Add(key, new Dictionary<int, string>());
                            }
                            else if (line.Contains("RUN"))
                            {
                                key = EEquipmentStatus.R;
                                _eqstcodelist.Add(key, new Dictionary<int, string>());
                            }
                        }
                        else if (!string.IsNullOrEmpty(line) && line.Contains(":"))
                        {
                            var keyValue = line.Split(':');
                            if (keyValue.Length == 2)
                            {
                                Int32.TryParse(keyValue[0].Trim(), out int result);
                                if (result > 0)
                                {
                                    _eqstcodelist[key].Add(result, keyValue[1].Trim());
                                }
                            }
                        }
                    }
                }
                return _eqstcodelist;
            }
        }

        public Item SecsItem
        {
            get
            {
                var stack = new Stack<List<Item>>();
                stack.Push(new List<Item>()
                {
                    A(CRST.ToString()),
                    A(EQST.ToString()),
                    A(EQSTCODE.ToString()),
                });
                return ParseItem(stack);
            }
        }
    }
}
