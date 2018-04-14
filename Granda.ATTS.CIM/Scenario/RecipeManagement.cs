#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: RecipeManagementScenario
// Author: Ivan JL Zhang    Date: 2018/4/3 18:06:59    Version: 1.0.0
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
using Granda.ATTS.CIM.Extension;
using Secs4Net;
using static Secs4Net.Item;
using static Granda.ATTS.CIM.Extension.SmlExtension;

using static Granda.ATTS.CIM.StreamType.Stream7_ProcessProgramManagement;
using static Granda.ATTS.CIM.StreamType.Stream6_DataCollection;
using Granda.ATTS.CIM.Model;
using Granda.ATTS.CIM.Data;
using Granda.ATTS.CIM.Data.Message;
using Granda.ATTS.CIM.Data.Report;

namespace Granda.ATTS.CIM.Scenario
{
    internal class RecipeManagement : BaseScenario, IScenario
    {
        public RecipeManagement()
        {
            ScenarioType = Model.Scenarios.Recipe_Management;
        }
        IRecipeManagement recipeManagement = new DefaultRecipeManagement();
        public RecipeManagement(IRecipeManagement callback) : this()
        {
            recipeManagement = callback;
        }
        public void HandleSecsMessage(SecsMessage secsMessage)
        {
            PrimaryMessage = secsMessage;
            switch (secsMessage.GetSFString())
            {
                case "S7F19":
                    SubScenarioName = Resource.RMS_Host_Attempts_To_Recipe_Mnt;
                    handleS7F19Message();
                    break;
                case "S725":
                    SubScenarioName = Resource.RMS_Host_Attempts_To_Recipe_Mnt;
                    handleS7F25Message();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// local端Process Program or Recipe发生变化时向host发送通知
        /// </summary>
        /// <returns></returns>
        public bool LaunchRecipeChangeReportProcess(IReport report)
        {
            var replyMsg = S6F11(report.SecsItem, 401);
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
        /// 对CurrentEPPDRequest的回复
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public bool LaunchCurrentEPPDReportProcess(IReport report)
        {
            PrimaryMessage.S7F20(report.SecsItem);
            return true;
        }
        /// <summary>
        /// 对 Formatted Process Program Request 的回复
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public bool LaunchFormattedProcessProgramReport(IReport report)
        {
            PrimaryMessage.S7F26(report.SecsItem);
            return true;
        }
        /// <summary>
        /// Host端尝试直接进行Recipe管理
        /// </summary>
        /// <returns></returns>
        public bool LaunchCurrentEPPDRequestProcess()
        {
            //var stack = new Stack<List<Item>>();
            //stack.Push(new List<Item>()
            //    {
            //        A(UnitId),
            //        A(pptype.ToString()),
            //    });

            //var replyMsg = S7F19(ParseItem(stack));
            //if (replyMsg != null && replyMsg.GetSFString() == "S19F20")
            //{
            //    // 需要相应实现
            //    return true;
            //}
            return false;
        }
        /// <summary>
        /// Host端发送Formatted Process Program Request
        /// </summary>
        /// <returns></returns>
        public bool LaunchFormattedRecipeRequestProcess()
        {
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>()
                {
                    A("PPID"),
                    A("UNITID"),
                    A("SUNITID"),
                    A("SSUNITID"),
                    A("PPTYPE"),
                });

            var replyMsg = S7F25(ParseItem(stack));
            if (replyMsg != null && replyMsg.GetSFString() == "S19F26")
            {
                // 需要相应实现
                return true;
            }
            return false;
        }


        void handleS7F19Message()
        {
            CurrentEPPDRequest currentEPPDRequest = new CurrentEPPDRequest();
            currentEPPDRequest.Parse(PrimaryMessage.SecsItem);
            recipeManagement.CurrentEPPDRequestEvent(currentEPPDRequest);
        }

        void handleS7F25Message()
        {
            FormattedProcessProgramRequest formattedProcessProgramRequest = new FormattedProcessProgramRequest();
            formattedProcessProgramRequest.Parse(PrimaryMessage.SecsItem);
            recipeManagement.FormattedProcessProgramRequestEvent(formattedProcessProgramRequest);
        }



        private class DefaultRecipeManagement : IRecipeManagement
        {
            public void CurrentEPPDRequestEvent(CurrentEPPDRequest currentEPPDRequest)
            {
                throw new NotImplementedException();
            }

            public void FormattedProcessProgramRequestEvent(FormattedProcessProgramRequest formattedProcessProgramRequest)
            {
                throw new NotImplementedException();
            }
        }
    }
    /// <summary>
    /// Process Program (Recipe) Management 回调方法接口
    /// </summary>
    public interface IRecipeManagement
    {
        /// <summary>
        /// Current EPPD Request回调方法
        /// </summary>
        /// <param name="currentEPPDRequest"></param>
        void CurrentEPPDRequestEvent(CurrentEPPDRequest currentEPPDRequest);
        /// <summary>
        /// Formatted Process Program Request回调方法
        /// </summary>
        /// <param name="formattedProcessProgramRequest"></param>
        void FormattedProcessProgramRequestEvent(FormattedProcessProgramRequest formattedProcessProgramRequest);
    }
}
