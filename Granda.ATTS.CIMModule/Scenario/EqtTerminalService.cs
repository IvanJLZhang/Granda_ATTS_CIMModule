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
using System.Linq;
using System.Text;
using Granda.ATTS.CIMModule.Model;
using Secs4Net;

namespace Granda.ATTS.CIMModule.Scenario
{
    internal class EqtTerminalService : BaseScenario, IScenario
    {
        public EqtTerminalService()
        {
            ScenarioType = Scenarios.Equipment_Terminal_Service;
        }
        public void HandleSecsMessage(SecsMessage secsMessage)
        {
            throw new NotImplementedException();
        }
    }
}
