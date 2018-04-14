#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: ClockScenario
// Author: Ivan JL Zhang    Date: 2018/4/3 18:05:49    Version: 1.0.0
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using Granda.ATTS.CIM.Data;
using Granda.ATTS.CIM.Extension;
using Secs4Net;
using static Granda.ATTS.CIM.StreamType.Stream2_EquipmentControl;

namespace Granda.ATTS.CIM.Scenario
{
    internal class Clock : BaseScenario, IScenario
    {
        IClock clock = new DefaultClock();
        public Clock()
        {
            ScenarioType = Model.Scenarios.Clock;
        }
        public Clock(IClock callback) : this()
        {
            clock = callback;
        }
        public void HandleSecsMessage(SecsMessage secsMessage)
        {
            switch (secsMessage.GetSFString())
            {
                case "S2F17":// equipment requests host time
                             // 在initialize scenario中有处理此消息， 所以这里不再重复
                    break;
                case "S2F31":// host instructs equipment to set time
                    SubScenarioName = Resource.CLKS_Host_Instructs_Time;
                    string dateTimeStr = secsMessage.SecsItem.GetString();
                    if (!String.IsNullOrEmpty(dateTimeStr))
                    {
                        clock.UpdateDateTime(dateTimeStr);
                        secsMessage.S2F32("0");
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// host 端用于测试向eqt发送更新时间指定功能
        /// </summary>
        /// <returns></returns>
        public bool LaunchInstructTimeProcess()
        {
            string dateTimeStr = DateTime.Now.ToString("yyyyMMddHHmmss");
            var replyMsg = S2F31(dateTimeStr);
            if (replyMsg != null && replyMsg.GetSFString() == "S2F18")
            {
                var result = replyMsg.GetCommandValue();
                if (result == 0)
                {
                    clock.UpdateDateTime(dateTimeStr);
                }
                return true;
            }
            return false;
        }


        public class DefaultClock : IClock
        {
            public void UpdateDateTime(string dateTimeStr)
            {
                Debug.WriteLine("update data time string: " + dateTimeStr);
            }
        }
    }
    /// <summary>
    /// Clock 回调方法接口
    /// </summary>
    public interface IClock
    {
        /// <summary>
        /// 更新系统时间
        /// </summary>
        /// <param name="dateTimeStr"></param>
        void UpdateDateTime(string dateTimeStr);
    }
}
