#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: SLOTINFO
// Author: Ivan JL Zhang    Date: 2018/4/14 10:46:32    Version: 1.0.0
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
using System.Linq;
using System.Text;

namespace Granda.ATTS.CIM.Data.ENUM
{
    /// <summary>
    /// Slot Information. 
    /// <para>Remark:
    /// (EX) “44444888885555500000000000”,
    /// 01~05: Normal End,
    /// 06~10: Skip,
    /// 11 ~ 15: Abort,
    /// 16~26: Empty,
    /// </para>
    /// </summary>
    public enum SLOTINFO
    {
        /// <summary>
        /// 0: Empty
        /// </summary>
        Empty = 0,
        /// <summary>
        /// 1: Wait for Command (Recipe)
        /// </summary>
        WaitForCommand,
        /// <summary>
        /// 2: Wait for Process(Start)
        /// </summary>
        WaitForProcess,
        /// <summary>
        /// 3: Processing
        /// </summary>
        Processing,
        /// <summary>
        /// 4: Process Normal End 
        /// </summary>
        NormalEnd,
        /// <summary>
        /// 5: Process Abort End
        /// </summary>
        AbortEnd,
        /// <summary>
        /// 6: Process Alarm End (Glass Unit IN ->Alarm Set -> Alarm Reset -> Normal Process End -> index IN)
        /// </summary>
        AlarmEnd,
        /// <summary>
        /// 7: Process Fail End (Glass Unit IN ->Alarm Set ->Index IN)
        /// </summary>
        FailEnd,
        /// <summary>
        /// 8: Skip (Does not selected by host when received S2F103 message)
        /// </summary>
        Skip,
    }
}
