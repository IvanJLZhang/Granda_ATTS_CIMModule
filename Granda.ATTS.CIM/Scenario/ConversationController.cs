#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: ConversationController
// Author: Ivan JL Zhang    Date: 2018/5/4 17:12:42    Version: 1.0.0
// Description: 
//   用于对Conversation Timeout行为的管控
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
using System.Threading;
using System.Threading.Tasks;

namespace Granda.ATTS.CIM.Scenario
{
    internal class ConversationController
    {
        /// <summary>
        /// 会话计时器
        /// </summary>
        protected Timer ConversationTimer;
        /// <summary>
        /// 会话控制器
        /// </summary>
        protected CancellationTokenSource CancellationTokenSource;
        /// <summary>
        /// 会话完成后结果保存变量
        /// </summary>
        protected TaskCompletionSource<bool> TaskCompletionSource;

        public ConversationController()
        {
            ConversationTimer = new Timer(obj =>
            {
                ConversationTimer.Change(Timeout.Infinite, Timeout.Infinite);
                CancellationTokenSource.Cancel();
            }, null, Timeout.Infinite, Timeout.Infinite);
        }
        /// <summary>
        /// 启动会话
        /// </summary>
        /// <param name="CntDown"></param>
        protected void StartConversation(int CntDown)
        {
            this.CancellationTokenSource = new CancellationTokenSource();
            this.TaskCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.None);
            ConversationTimer.Change(CntDown, Timeout.Infinite);
        }
        /// <summary>
        /// 完成会话
        /// </summary>
        protected void CompleteConversation()
        {
            ConversationTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}
