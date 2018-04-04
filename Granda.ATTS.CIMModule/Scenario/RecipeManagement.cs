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
using Granda.ATTS.CIMModule.Extension;
using Secs4Net;
using static Secs4Net.Item;
using static Granda.ATTS.CIMModule.Extension.SmlExtension;

using static Granda.ATTS.CIMModule.StreamType.Stream7_ProcessProgramManagement;
using static Granda.ATTS.CIMModule.StreamType.Stream6_DataCollection;
using Granda.ATTS.CIMModule.Model;

namespace Granda.ATTS.CIMModule.Scenario
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
            primaryMessage = secsMessage;
            switch (secsMessage.GetSFString())
            {
                case "S7F19":
                    SubScenarioName = Resource.RMS_Host_Attempts_To_Recipe_Mnt;
                    handleS7F19Message();
                    break;
                case "S725":
                    SubScenarioName = Resource.RMS_Host_Attempts_To_Recipe_Mnt;
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// local端recipe发生变化时向host发送通知
        /// </summary>
        /// <returns></returns>
        public bool LaunchRecipeChangeProcess()
        {
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>());
            stack.Peek().Add(A("DATAID"));
            stack.Peek().Add(A("401"));
            //需要具体实现
            var item = ParseItem(stack);
            var replyMsg = S6F11(item, 401);
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
        /// Host端尝试直接进行Recipe管理
        /// </summary>
        /// <param name="pptype"></param>
        /// <param name="UnitId"></param>
        /// <returns></returns>
        public bool LaunchCurrentEPPDRequestProcess(PPTYPE pptype, string UnitId)
        {
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>()
                {
                    A(UnitId),
                    A(pptype.ToString()),
                });

            var replyMsg = S7F19(ParseItem(stack));
            if (replyMsg != null && replyMsg.GetSFString() == "S19F20")
            {
                // 需要相应实现
                return true;
            }
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
            if (primaryMessage.SecsItem.Count == 2)
            {
                string unitId = primaryMessage.SecsItem.Items[0].GetString();
                string pptype = primaryMessage.SecsItem.Items[1].GetString();

                var stack = new Stack<List<Item>>();
                stack.Push(new List<Item>()
                {
                    A(unitId),
                    A(pptype),
                });
                stack.Push(new List<Item>()
                {
                    A("PPID"),
                });
                primaryMessage.S7F20(ParseItem(stack));
            }
        }

        void handleS7F25Message()
        {
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>()
                {
                    A("TestRecipe"),
                });// 需要具体实现

            primaryMessage.S7F26(ParseItem(stack));
        }

        public interface IRecipeManagement
        {

        }

        private class DefaultRecipeManagement : IRecipeManagement
        {

        }
    }
}
