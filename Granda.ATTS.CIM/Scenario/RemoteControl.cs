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
using Granda.ATTS.CIM.Extension;
using Granda.ATTS.CIM.Model;
using Secs4Net;
using static Granda.ATTS.CIM.StreamType.Stream2_EquipmentControl;
using static Granda.ATTS.CIM.StreamType.Stream6_DataCollection;
using static Secs4Net.Item;
using static Granda.ATTS.CIM.Extension.ExtensionHelper;
using static Granda.ATTS.CIM.Extension.SmlExtension;
using System.Diagnostics;
using Granda.ATTS.CIM.Data;
using Granda.ATTS.CIM.Data.ENUM;
using Granda.ATTS.CIM.Data.Message;
using Granda.ATTS.CIM.Data.Report;

namespace Granda.ATTS.CIM.Scenario
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
            PrimaryMessage = secsMessage;
            switch (PrimaryMessage.GetSFString())
            {
                case "S2F41":// host command
                    int rcmd = PrimaryMessage.GetCommandValue();
                    handleRCMDMessage(rcmd);
                    break;
                default:
                    break;
            }
        }

        private void handleRCMDMessage(int rcmd)
        {
            PrimaryMessage.S2F42(rcmd, 0);// 立即回复S2F42
            switch ((RCMD)rcmd)
            {
                case RCMD.START:
                    SubScenarioName = Resource.RCS_Host_Command_Start;
                    break;
                case RCMD.CANCEL:
                    SubScenarioName = Resource.RCS_Host_Command_Cancel;
                    break;
                case RCMD.ABORT:
                    SubScenarioName = Resource.RCS_Host_Command_Abort;
                    break;
                case RCMD.PAUSE:
                    SubScenarioName = Resource.RCS_Host_Command_Pause;
                    break;
                case RCMD.RESUME:
                    SubScenarioName = Resource.RCS_Host_Command_Resume;
                    break;
                case RCMD.OPERATOR_CALL:
                    SubScenarioName = Resource.RCS_Host_Command_Operator_Call;
                    break;
                default:
                    break;
            }
            RemoteControlCommandJob remoteControlCommandJob = new RemoteControlCommandJob();
            remoteControlCommandJob.Parse(PrimaryMessage.SecsItem);
            remoteControlScenario.UpdateProcessReportState(remoteControlCommandJob);
        }
        /// <summary>
        /// Process Report
        /// CEID: 301=>start, 304=>cancel, 305=>abort, 306=>pause, 307=>resume
        /// </summary>
        /// <returns></returns>
        public bool LaunchProcessReport(RCMD rcmd, ProcessLaunchReport processLaunchReport, EquipmentBaseInfo equipmentBaseInfo)
        {
            int ceid = 0;
            switch ((RCMD)rcmd)
            {
                case RCMD.START:
                    ceid = 301;
                    break;
                case RCMD.CANCEL:
                    ceid = 304;
                    break;
                case RCMD.ABORT:
                    ceid = 305;
                    break;
                case RCMD.PAUSE:
                    ceid = 306;
                    break;
                case RCMD.RESUME:
                    ceid = 307;
                    break;
                case RCMD.OPERATOR_CALL:
                default:
                    break;
            }
            ProcessLaunchReport newReport = new ProcessLaunchReport(0, ceid, 100, equipmentBaseInfo, 301)
            {
                LOTID = processLaunchReport.LOTID,
                PTID = processLaunchReport.PTID,
                PTTYPE = processLaunchReport.PTTYPE,
                PTUSETYPE = processLaunchReport.PTUSETYPE,
                CSTID = processLaunchReport.CSTID,
                PPID = processLaunchReport.PPID,
            };
            var replyMsg = S6F11(newReport.SecsItem, (int)ceid);
            if (replyMsg != null && replyMsg.GetSFString() == "S6F12")
            {
                try
                {
                    int ack = replyMsg.GetCommandValue();
                    if (ack == 0)
                    {
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
        /// <summary>
        /// Host端测试接口： 发送Host Command
        /// </summary>
        /// <param name="hostCommand"></param>
        /// <returns></returns>
        public bool LaunchHostCommand(RCMD hostCommand)
        {
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>());
            stack.Peek().Add(A($"{hostCommand}"));
            stack.Push(new List<Item>());
            stack.Push(new List<Item>());
            stack.Peek().Add(A("PTID"));
            stack.Peek().Add(A("PTID"));
            stack.Push(new List<Item>());
            stack.Peek().Add(A("CSTID"));
            stack.Peek().Add(A("CSTID"));
            stack.Push(new List<Item>());
            stack.Peek().Add(A("LOTID"));
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
            void UpdateProcessReportState(RemoteControlCommandJob remoteControlCommandJob);
        }

        private class DefaultRemoteControlScenario : IRCSCallBack
        {
            public void UpdateProcessReportState(RemoteControlCommandJob remoteControlCommandJob)
            {
                Debug.WriteLine("Update Process Report State: " + remoteControlCommandJob.RCMD.ToString());
            }
        }
    }
}
