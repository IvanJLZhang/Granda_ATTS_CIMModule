#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: OperationNormalSequenceScen
// Author: Ivan JL Zhang    Date: 2018/4/4 9:53:50    Version: 1.0.0
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
    internal class OperationNormalSequence : BaseScenario, IScenario
    {
        public OperationNormalSequence()
        {
            ScenarioType = Model.Scenarios.Operation_Normal_Sequence;
        }

        public bool HandleSecsMessage(SecsMessage secsMessage)
        {
            return false;
        }
    }
}
