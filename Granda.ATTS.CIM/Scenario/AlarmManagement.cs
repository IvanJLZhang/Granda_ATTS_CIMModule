#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: AlarmManagementScenario
// Author: Ivan JL Zhang    Date: 2018/4/3 15:57:55    Version: 1.0.0
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
using Granda.ATTS.CIM.Extension;
using Granda.ATTS.CIM.Model;
using Granda.ATTS.CIM.StreamType;
using Secs4Net;
using static Granda.ATTS.CIM.Extension.ExtensionHelper;
using static Granda.ATTS.CIM.StreamType.Stream5_ExceptionReporting;
using static Secs4Net.Item;
using static Granda.ATTS.CIM.Extension.SmlExtension;
using Granda.ATTS.CIM.Data;

namespace Granda.ATTS.CIM.Scenario
{
    internal class AlarmManagement : BaseScenario, IScenario
    {
        private IAMSCallBack AMSCallBack = new DefaultAMSCallBack();
        public AlarmManagement(IAMSCallBack callback)
        {
            this.ScenarioType = Scenarios.Alarm_Management;
            this.AMSCallBack = callback;
        }
        public void HandleSecsMessage(SecsMessage secsMessage)
        {
            PrimaryMessage = secsMessage;
            switch (secsMessage.GetSFString())
            {
                case "S5F3":// enable or disable alarm
                    SubScenarioName = Resource.AMS_Enable_Disable_Alarm;
                    int command = secsMessage.GetCommandValue();
                    AMSCallBack.UpdateAlarmStatus(command == 0);
                    secsMessage.S5F4(0);
                    break;
                case "S5F103":// current alarm set list request
                    SubScenarioName = Resource.AMS_Alarm_List_Request;
                    secsMessage.S5F104(null);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Host端测试接口：Enable/Disable Alarm
        /// </summary>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        public bool LaunchUpdateAlarmProcess(bool isEnable)
        {
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>());
            stack.Peek().Add(A(isEnable ? "0" : "1"));
            stack.Peek().Add(A("UNITID"));
            stack.Push(new List<Item>());
            stack.Peek().Add(A("ALID"));

            var replyMsg = S5F3(ParseItem(stack));
            if (replyMsg != null && replyMsg.GetSFString() == "S5F4")
            {
                AMSCallBack.UpdateAlarmStatus(isEnable);
                return true;
            }
            return false;
        }
        /// <summary>
        /// host端测试List Alarm Request功能
        /// </summary>
        /// <returns></returns>
        public bool LaunchListAlarmRequestProcess()
        {
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>());
            stack.Peek().Add(A("UNITID"));
            var replyMsg = S5F103(ParseItem(stack));
            if (replyMsg != null && replyMsg.GetSFString() == "S5F104")
            {
                return true;
            }
            return false;
        }
        public interface IAMSCallBack
        {
            void UpdateAlarmStatus(bool isEnable);
        }
        private class DefaultAMSCallBack : IAMSCallBack
        {
            public void UpdateAlarmStatus(bool isEnable)
            {
                Debug.WriteLine("is enable alarm: " + isEnable);
            }
        }
    }
}
