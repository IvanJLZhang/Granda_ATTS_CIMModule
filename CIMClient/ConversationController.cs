#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: ConversationController
// Author: Ivan JL Zhang    Date: 2018/5/4 16:48:26    Version: 1.0.0
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
using System.Threading;
using System.Threading.Tasks;

namespace CIMClient
{
    class ConversationController : Task
    {
        public ConversationController(Action action) : base(action)
        {
        }

        public ConversationController(Action action, CancellationToken cancellationToken) : base(action, cancellationToken)
        {
        }

        public ConversationController(Action action, TaskCreationOptions creationOptions) : base(action, creationOptions)
        {
        }

        public ConversationController(Action<object> action, object state) : base(action, state)
        {
        }

        public ConversationController(Action action, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(action, cancellationToken, creationOptions)
        {
        }

        public ConversationController(Action<object> action, object state, CancellationToken cancellationToken) : base(action, state, cancellationToken)
        {
        }

        public ConversationController(Action<object> action, object state, TaskCreationOptions creationOptions) : base(action, state, creationOptions)
        {
        }

        public ConversationController(Action<object> action, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(action, state, cancellationToken, creationOptions)
        {
            
        }
    }

    
}
