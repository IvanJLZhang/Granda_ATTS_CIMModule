using Granda.HSMS;

namespace Granda.ATTS.CIM.Data
{
    /// <summary>
    /// 转换消息接口
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// 将SecsItem转换为业务数据类型
        /// </summary>
        /// <param name="item"></param>
        bool Parse(Item item);
    }
}
