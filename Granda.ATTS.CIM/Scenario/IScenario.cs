using Granda.HSMS;

namespace Granda.ATTS.CIM.Scenario
{
    /// <summary>
    /// 场景接口
    /// </summary>
    public interface IScenario
    {
        /// <summary>
        /// 处理primary message
        /// </summary>
        bool HandleSecsMessage(SecsMessage secsMessage);
    }
}
