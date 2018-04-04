#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: EqtTerminalServiceScenario
// Author: Ivan JL Zhang    Date: 2018/4/3 17:46:54    Version: 1.0.0
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
using Granda.ATTS.CIMModule.Extension;
using Granda.ATTS.CIMModule.Model;
using Granda.ATTS.CIMModule.StreamType;
using Secs4Net;
using static Secs4Net.Item;
using static Granda.ATTS.CIMModule.Extension.SmlExtension;
using static Granda.ATTS.CIMModule.StreamType.Stream10_TerminalServices;
namespace Granda.ATTS.CIMModule.Scenario
{
    internal class EqtTerminalService : BaseScenario, IScenario
    {
        public EqtTerminalService()
        {
            ScenarioType = Scenarios.Equipment_Terminal_Service;
        }
        public EqtTerminalService(IEqtTerminalService callback)
        {
            eqtTerminalService = callback;
        }
        IEqtTerminalService eqtTerminalService = new DefaultEqtTS();
        public void HandleSecsMessage(SecsMessage secsMessage)
        {
            primaryMessage = secsMessage;
            switch (primaryMessage.GetSFString())
            {
                case "S10F1":// operator sends textual
                    var strs = GetTestMsg();
                    if (strs != null)
                    {
                        eqtTerminalService.ReceiveTestMessage(strs);
                    }
                    primaryMessage.S10F2(0);
                    break;
                case "S10F5":// host sends muiti-block message
                    strs = GetTestMsg();
                    if (strs != null)
                    {
                        eqtTerminalService.ReceiveTestMessage(strs);
                    }
                    primaryMessage.S10F6(0);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 用于host/eqt向eqt/host端发送消息
        /// </summary>
        /// <param name="messages">消息内容</param>
        /// <param name="byOP">是否是从Operator端发送，true为是， false为从host端发送</param>
        /// <returns></returns>
        public bool SendMessages(string[] messages, bool byOP = true)
        {
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>() {
                A("TID"),
            });
            stack.Push(new List<Item>());
            foreach (var msg in messages)
            {
                stack.Peek().Add(A(msg));
            }
            var item = ParseItem(stack);
            if (byOP)
            {
                var replyMsg = S10F1(item);
                if (replyMsg != null && replyMsg.GetSFString() == "S10F2")
                {
                    var ack = replyMsg.GetCommandValue();
                    if (ack == 0)
                    {
                        eqtTerminalService.SendMessageDone(messages);
                        return true;
                    }
                }
            }
            else
            {
                var replyMsg = S10F5(item);
                if (replyMsg != null && replyMsg.GetSFString() == "S10F6")
                {
                    var ack = replyMsg.GetCommandValue();
                    if (ack == 0)
                    {
                        eqtTerminalService.SendMessageDone(messages);
                        return true;
                    }
                }
            }
            return false;
        }

        string[] GetTestMsg()
        {
            if (primaryMessage.SecsItem.Count == 2)
            {
                var itemList = primaryMessage.SecsItem.Items[1];
                List<string> strList = new List<string>();
                foreach (var item in itemList.Items)
                {
                    strList.Add(item.GetString());
                }
                return strList.ToArray();
            }
            return null;
        }
        public interface IEqtTerminalService
        {
            void ReceiveTestMessage(string[] messages);
            void SendMessageDone(string[] messages);
        }

        private class DefaultEqtTS : IEqtTerminalService
        {
            public void ReceiveTestMessage(string[] messages)
            {
                Debug.WriteLine("receive message: " + messages);
            }
            public void SendMessageDone(string[] messages)
            {
                Debug.WriteLine("sent message: " + messages);
            }
        }
    }
}
