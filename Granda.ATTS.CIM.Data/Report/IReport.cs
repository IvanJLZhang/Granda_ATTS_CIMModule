using Granda.HSMS;
namespace Granda.ATTS.CIM.Data
{
    /// <summary>
    /// 业务逻辑数据转换为通信数据类型接口
    /// </summary>
    public interface IReport
    {
        /// <summary>
        /// 将数据结构体转换成Item
        /// </summary>
        Item SecsItem { get; }
    }
}
