using Granda.ATTS.CIMModule.Model;
using Secs4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granda.ATTS.CIMModule.Scenario
{
    public interface IScenario
    {
        /// <summary>
        /// 枚举类型属性，Scenario类型
        /// </summary>
        Scenarios ScenarioType { get;}

        /// <summary>
        /// 
        /// </summary>
        String ScenarioName { get; }
        /// <summary>
        /// 
        /// </summary>
        String SubScenarioName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        void HandleSecsMessage(SecsMessage secsMessage);
        /// <summary>
        /// 
        /// </summary>
        void PackSecsMessage();

        SecsMessage primaryMessage { get; set; }
    }
}
