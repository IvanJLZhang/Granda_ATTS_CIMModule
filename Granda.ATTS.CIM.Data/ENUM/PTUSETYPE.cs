#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: PTUSETYPE
// Author: Ivan JL Zhang    Date: 2018/4/12 15:45:34    Version: 1.0.0
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
    /// Port Use Type，
    /// 需与同目录下PTUSETYPE文件共同使用
    /// </summary>
    public abstract class PTUSETYPE
    {
        private static Dictionary<string, string> _ptusetypelist = null;
        /// <summary>
        /// 系统内置的Port use type列表
        /// 作为参考以及校验用
        /// </summary>
        public static Dictionary<string, string> PTUSETYPELIST
        {
            get
            {
                if (_ptusetypelist != null)
                    return _ptusetypelist;
                _ptusetypelist = new Dictionary<string, string>();
                using (var filestream = File.OpenRead(@".\Configs\PTUSETYPE"))
                {
                    StreamReader reader = new StreamReader(filestream);
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (!string.IsNullOrEmpty(line) && line.Contains(":"))
                        {
                            var keyValue = line.Split(':');
                            if (keyValue.Length == 2)
                            {
                                _ptusetypelist.Add(keyValue[1], keyValue[0]);
                            }
                        }
                    }
                }
                return _ptusetypelist;
            }
        }
    }
}
