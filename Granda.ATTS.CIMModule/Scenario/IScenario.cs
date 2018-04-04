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
        /// 处理primary message
        /// </summary>
        void HandleSecsMessage(SecsMessage secsMessage);
    }
}
