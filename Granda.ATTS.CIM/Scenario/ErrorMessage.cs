#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: ErrorMessageScenario
// Author: Ivan JL Zhang    Date: 2018/4/3 18:02:36    Version: 1.0.0
// Description: 
//   
// 
// Revision History: 
// <Author>  		<Date>     	 	<Revision>  		<Modification>
// 	
//----------------------------------------------------------------------------*/
#endregion
using System;
using Granda.HSMS;

namespace Granda.ATTS.CIM.Scenario
{
    internal class ErrorMessage : BaseScenario, IScenario
    {
        public ErrorMessage()
        {
            ScenarioType = Model.Scenarios.Error_Message;
        }

        public void HandleSecsMessage(SecsMessage secsMessage)
        {
            throw new NotImplementedException();
        }
    }
}
