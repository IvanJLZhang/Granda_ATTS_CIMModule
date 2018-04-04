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
using Secs4Net;

namespace Granda.ATTS.CIMModule.Scenario
{
    internal class RecipeManagement : BaseScenario, IScenario
    {
        public RecipeManagement()
        {
            ScenarioType = Model.Scenarios.Recipe_Management;
        }

        public void HandleSecsMessage(SecsMessage secsMessage)
        {
            throw new NotImplementedException();
        }
    }
}
