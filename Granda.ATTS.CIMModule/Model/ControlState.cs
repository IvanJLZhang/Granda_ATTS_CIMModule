using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Granda.ATTS.CIMModule.Model
{
    /// <summary>
    /// 描述Control State的三种状态枚举
    /// </summary>
    public enum ControlState
    {
        /// <summary>
        /// offline
        /// </summary>
        OFFLINE = 111,
        /// <summary>
        /// online local
        /// </summary>
        ONLINE_LOCAL = 112,
        /// <summary>
        /// online remote
        /// </summary>
        ONLINE_REMOTE = 113,
        /// <summary>
        /// equipment status change
        /// </summary>
        EQT_STATUS_CHANGE = 114
    }
}
