#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: PTTYPE
// Author: Ivan JL Zhang    Date: 2018/4/12 15:42:58    Version: 1.0.0
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
    /// Port Type，
    /// 需与同目录下PTTYPE文件共同使用
    /// </summary>
    public abstract class PTTYPE
    {
        private static Dictionary<string, string> _pttypelist = null;
        /// <summary>
        /// 系统内置的Port type列表
        /// 作为参考以及校验用
        /// </summary>
        public static Dictionary<string, string> PTTYPELIST
        {
            get
            {
                if (_pttypelist != null)
                    return _pttypelist;
                _pttypelist = new Dictionary<string, string>();
                using (var filestream = File.OpenRead(@".\Configs\PTTYPE"))
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
                                _pttypelist.Add(keyValue[1], keyValue[0]);
                            }
                        }
                    }
                }
                return _pttypelist;
            }
        }
    }
}
