using Granda.ATTS.CIMModule.Model;
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
        Scenarios ScenarioType { get; set; }
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
        void ParseSecsMessage();
        /// <summary>
        /// 
        /// </summary>
        void PackSecsMessage();
    }
}
