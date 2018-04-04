#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: DataCollectionScenario
// Author: Ivan JL Zhang    Date: 2018/4/3 12:02:59    Version: 1.0.0
// Description: 
//   用于收集Equipment Information场景
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
    internal class DataCollection : BaseScenario, IScenario
    {
        public DataCollection()
        {
            ScenarioType = Scenarios.Data_Collection;
        }

        public void HandleSecsMessage(SecsMessage secsMessage)
        {

        }
    }
}
