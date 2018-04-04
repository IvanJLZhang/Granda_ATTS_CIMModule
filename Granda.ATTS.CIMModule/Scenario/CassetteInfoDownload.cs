#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: CassetteInfoDownload
// Author: Ivan JL Zhang    Date: 2018/4/3 18:08:22    Version: 1.0.0
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
    internal class CassetteInfoDownload : BaseScenario, IScenario
    {
        public CassetteInfoDownload()
        {
            ScenarioType = Model.Scenarios.Cassette_Information_Download;
        }

        public void HandleSecsMessage(SecsMessage secsMessage)
        {
            throw new NotImplementedException();
        }
    }
}
