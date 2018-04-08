#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: RemoteControlScenario
// Author: Ivan JL Zhang    Date: 2018/4/3 12:14:05    Version: 1.0.0
// Description: 
//   用于实现Remote Control场景相关功能
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
using Granda.ATTS.CIMModule.Extension;
using Granda.ATTS.CIMModule.Model;
using Secs4Net;
using static Granda.ATTS.CIMModule.StreamType.Stream2_EquipmentControl;
using static Granda.ATTS.CIMModule.StreamType.Stream6_DataCollection;
using static Secs4Net.Item;
using static Granda.ATTS.CIMModule.Extension.ExtensionHelper;
using static Granda.ATTS.CIMModule.Extension.SmlExtension;
using System.Diagnostics;
using Granda.ATTS.CIMModule.Data;

namespace Granda.ATTS.CIMModule.Scenario
{
    internal class RemoteControl : BaseScenario, IScenario
    {
        private IRCSCallBack remoteControlScenario = new DefaultRemoteControlScenario();

        public RemoteControl(IRCSCallBack callback)
        {
            this.ScenarioType = Scenarios.Remote_Control;
            this.remoteControlScenario = callback;
        }

        public void HandleSecsMessage(SecsMessage secsMessage)
        {
            primaryMessage = secsMessage;
            switch (primaryMessage.GetSFString())
            {
                case "S2F41":// host command
                    int rcmd = primaryMessage.GetCommandValue();
                    handleRCMDMessage(rcmd);
                    break;
                default:
                    break;
            }
        }

        private bool handleRCMDMessage(int rcmd)
        {
            primaryMessage.S2F42(rcmd, 0);// 立即回复S2F42
            int ceid = 0;
            switch ((HostCommand)rcmd)
            {
                case HostCommand.START:
                    ceid = 301;
                    SubScenarioName = Resource.RCS_Host_Command_Start;
                    break;
                case HostCommand.CANCEL:
                    ceid = 304;
                    SubScenarioName = Resource.RCS_Host_Command_Cancel;
                    break;
                case HostCommand.ABORT:
                    SubScenarioName = Resource.RCS_Host_Command_Abort;
                    ceid = 305;
                    break;
                case HostCommand.PAUSE:
                    SubScenarioName = Resource.RCS_Host_Command_Pause;
                    ceid = 306;
                    break;
                case HostCommand.RESUME:
                    SubScenarioName = Resource.RCS_Host_Command_Resume;
                    ceid = 307;
                    break;
                case HostCommand.OPERATOR_CALL:
                    SubScenarioName = Resource.RCS_Host_Command_Operator_Call;
                    break;
                default:
                    break;
            }
            return LaunchProcessReport(ceid, (HostCommand)rcmd);
        }
        /// <summary>
        /// Process Report
        /// CEID: 301=>start, 304=>cancel, 305=>abort, 306=>pause, 307=>resume
        /// </summary>
        /// <param name="ceid"></param>
        /// <param name="hostCommand"></param>
        /// <returns></returns>
        private bool LaunchProcessReport(int ceid, HostCommand hostCommand)
        {
            if (ceid == 301 ||
                ceid == 304 ||
                ceid == 305 ||
                ceid == 306 ||
                ceid == 307)
            {
                var stack = new Stack<List<Item>>();
                stack.Push(new List<Item>());
                stack.Peek().Add(A("DATAID"));
                stack.Peek().Add(A($"{ceid}"));
                stack.Push(new List<Item>());
                stack.Push(new List<Item>());
                stack.Peek().Add(A("fix RPTID"));
                stack.Push(new List<Item>());
                stack.Peek().Add(A("CRST"));
                stack.Peek().Add(A("EQST"));
                stack.Peek().Add(A("EQSTCODE"));
                stack.Push(new List<Item>());
                stack.Peek().Add(A("fix RPTID"));
                stack.Push(new List<Item>());
                stack.Peek().Add(A("LOTID"));
                stack.Peek().Add(A("P01"));
                stack.Peek().Add(A("PB"));
                stack.Peek().Add(A("OO"));
                stack.Peek().Add(A("CST001"));
                stack.Peek().Add(A("TestRecipe"));
                var item = ParseItem(stack);
                var replyMsg = S6F11(item, (int)ceid);
                if (replyMsg != null && replyMsg.GetSFString() == "S6F12")
                {
                    try
                    {
                        int ack = replyMsg.GetCommandValue();
                        if (ack == 0)
                        {
                            remoteControlScenario.UpdateProcessReportState(hostCommand);
                            return true;
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        return false;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Host端测试接口： 发送Host Command
        /// </summary>
        /// <param name="hostCommand"></param>
        /// <returns></returns>
        public bool LaunchHostCommand(HostCommand hostCommand)
        {
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>());
            stack.Peek().Add(A($"{hostCommand}"));
            stack.Push(new List<Item>());
            stack.Push(new List<Item>());
            stack.Peek().Add(A("fixed PTID"));
            stack.Peek().Add(A("PTID"));
            stack.Push(new List<Item>());
            stack.Peek().Add(A("fixed CSTID"));
            stack.Peek().Add(A("CSTID"));
            stack.Push(new List<Item>());
            stack.Peek().Add(A("fixed LOTID"));
            stack.Peek().Add(A("CSTID"));
            var item = ParseItem(stack);

            var replyMsg = S2F41(item, (int)hostCommand);
            if (replyMsg != null && replyMsg.GetSFString() == "S4F42")
            {
                try
                {
                    int ack = replyMsg.GetCommandValue();
                    if (ack == (int)hostCommand)
                    {
                        remoteControlScenario.UpdateProcessReportState(hostCommand);
                        return true;
                    }
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            }
            return false;
        }

        public interface IRCSCallBack
        {
            void UpdateProcessReportState(HostCommand hostCommand);
        }

        private class DefaultRemoteControlScenario : IRCSCallBack
        {
            public void UpdateProcessReportState(HostCommand hostCommand)
            {
                Debug.WriteLine("Update Process Report State: " + hostCommand.ToString());
            }
        }
    }
}
