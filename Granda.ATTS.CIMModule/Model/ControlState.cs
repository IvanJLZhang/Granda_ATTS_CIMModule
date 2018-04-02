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
        OFFLINE = 111,
        ONLINE_LOCAL = 112,
        ONLINE_REMOTE = 113,
        EQT_STATUS_CHANGE = 114
    }
}
