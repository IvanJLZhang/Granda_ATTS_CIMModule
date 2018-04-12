#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: EQSTCODE
// Author: Ivan JL Zhang    Date: 2018/4/12 15:55:33    Version: 1.0.0
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
using System.IO;
using System.Linq;
using System.Text;

namespace Granda.ATTS.CIM.Data.ENUM
{
    /// <summary>
    /// 需与EQSTCODE文件在同一目录下共同使用
    /// </summary>
    public abstract class EQSTCODE
    {
        private static Dictionary<EQST, Dictionary<string, int>> _eqstcodelist = null;
        /// <summary>
        /// 系统内置的equipment status reason code列表
        /// 作为参考以及校验用
        /// </summary>
        public static Dictionary<EQST, Dictionary<string, int>> EQSTCODELIST
        {
            get
            {
                if (_eqstcodelist != null)
                    return _eqstcodelist;
                _eqstcodelist = new Dictionary<EQST, Dictionary<string, int>>();
                using (var filestream = File.OpenRead("EQSTCODE"))
                {
                    EQST key = EQST.I;
                    StreamReader reader = new StreamReader(filestream);
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (!string.IsNullOrEmpty(line) && line.Contains("#EQST"))
                        {
                            if (line.Contains("MAINT"))
                            {
                                key = EQST.M;
                                _eqstcodelist.Add(key, new Dictionary<string, int>());
                            }
                            else if (line.Contains("DOWN"))
                            {
                                key = EQST.D;
                                _eqstcodelist.Add(key, new Dictionary<string, int>());
                            }
                            else if (line.Contains("PAUSE"))
                            {
                                key = EQST.P;
                                _eqstcodelist.Add(key, new Dictionary<string, int>());
                            }
                            else if (line.Contains("IDLE"))
                            {
                                key = EQST.I;
                                _eqstcodelist.Add(key, new Dictionary<string, int>());
                            }
                            else if (line.Contains("RUN"))
                            {
                                key = EQST.R;
                                _eqstcodelist.Add(key, new Dictionary<string, int>());
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
                                    _eqstcodelist[key].Add(keyValue[1].Trim(), result);
                                }
                            }
                        }
                    }
                }
                return _eqstcodelist;
            }
        }
    }
}
