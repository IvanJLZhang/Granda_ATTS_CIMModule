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
using Granda.ATTS.CIM.Extension;
using Granda.ATTS.CIM.Model;
using Granda.ATTS.CIM.StreamType;
using Granda.HSMS;
using static Granda.ATTS.CIM.StreamType.Stream10_TerminalServices;
using static Granda.HSMS.Item;
namespace Granda.ATTS.CIM.Scenario
{
    internal class EqtTerminalService : BaseScenario, IScenario
    {
        #region 构造方法和变量
        public EqtTerminalService()
        {
            ScenarioType = Scenarios.Equipment_Terminal_Service;
        }
        public EqtTerminalService(IEqtTerminalService callback)
        {
            eqtTerminalService = callback;
        }
        IEqtTerminalService eqtTerminalService = new DefaultEqtTS();
        #endregion

        #region message handle methods
        public bool HandleSecsMessage(SecsMessage secsMessage)
        {
            bool ret = false;
            PrimaryMessage = secsMessage;
            switch (PrimaryMessage.GetSFString())
            {
                case "S10F1":// operator sends textual
                    var strs = GetTestMsg();
                    ret = strs != null;
                    if (ret)
                    {
                        eqtTerminalService.ReceiveTestMessage(strs);
                        PrimaryMessage.S10F2(0);
                    }
                    break;
                case "S10F5":// host sends muiti-block message
                    strs = GetTestMsg();
                    ret = strs != null;
                    if (ret)
                    {
                        eqtTerminalService.ReceiveTestMessage(strs);
                        PrimaryMessage.S10F6(0);
                    }
                    break;
                default:
                    break;
            }
            return ret;
        }
        #endregion

        #region Equipment Terminal Service mthods
        /// <summary>
        /// 用于host/eqt向eqt/host端发送消息
        /// </summary>
        /// <param name="messages">消息内容</param>
        /// <param name="byOP">是否是从Operator端发送，true为是， false为从host端发送</param>
        /// <returns></returns>
        public bool SendMessages(string[] messages, bool byOP = true)
        {
            if (messages.Length <= 0)
                return false;
            var itemList = new List<Item>();
            // Terminal number
            // 0: Single or main terminal,
            // >0: Additional terminals at the same equipment.
            itemList.Add(A("0"));
            itemList.Add(
                new Func<string[], Item>((messageArr) =>
                {
                    var itemList1 = new List<Item>();
                    foreach (var ite in messageArr)
                    {
                        itemList1.Add(A(ite ?? String.Empty));
                    }
                    return itemList1.Count > 1 ? L(itemList1) : itemList1[0];
                })(messages)
                );
            var item = L(itemList);
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
            CIMBASE.WriteLog(AATS.Log.LogLevel.ERROR, "something wrong was happened when send display message.");
            return false;
        }

        string[] GetTestMsg()
        {
            if (PrimaryMessage.SecsItem.Count == 2)
            {
                var itemList = PrimaryMessage.SecsItem.Items[1];
                List<string> strList = new List<string>();
                foreach (var item in itemList.Items)
                {
                    strList.Add(item.GetString());
                }
                return strList.ToArray();
            }
            return null;
        }
        #endregion

        #region 接口默认实例
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
        #endregion
    }

    #region 接口
    /// <summary>
    /// Equipment Terminal Service回调方法接口
    /// </summary>
    public interface IEqtTerminalService
    {
        /// <summary>
        /// 收到Display Message from host
        /// </summary>
        void ReceiveTestMessage(string[] messages);
        /// <summary>
        /// 向host发送display message结果回调方法
        /// </summary>
        void SendMessageDone(string[] messages);
    }
    #endregion
}
