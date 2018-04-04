#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: CimModuleForEQT
// Author: Ivan JL Zhang    Date: 2018/4/4 10:36:38    Version: 1.0.0
// Description: 
//   用于Local端的CIM模块
// 
// Revision History: 
// <Author>  		<Date>     	 	<Revision>  		<Modification>
// 	
//----------------------------------------------------------------------------*/
#endregion
using Granda.ATTS.CIMModule.Model;
using Granda.ATTS.CIMModule.Scenario;
using Secs4Net;

namespace Granda.ATTS.CIMModule
{
    /// <summary>
    /// Equipment端CIM模块运行主入口
    /// 实现在：
    /// <para>Initialize Scenario,</para>
    /// <para>Remote Control,</para>
    /// <para>Alarm Management,</para>
    /// <para>Clock,</para>
    /// <para>Equipment Terminal Service,</para>
    /// <para>Process Program(Recipe) Management,</para>
    /// <para>Data Collection</para>
    /// 等场景下primary message的处理，以及由Equipment端发起的消息进程
    /// </summary>
    public class CimModuleForEQT : CimModuleBase
    {
        /// <summary>
        /// 构造方法， 提供SecsGem参数
        /// </summary>
        /// <param name="secsGem"></param>
        public CimModuleForEQT(SecsGem secsGem) : base(secsGem)
        {
        }
        /// <summary>
        /// 构造方法，提供创建SecsGem所需参数
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="isActive"></param>
        public CimModuleForEQT(string ipAddress, int port, bool isActive) : base(ipAddress, port, isActive)
        {
        }

        #region 公开的方法
        /// <summary>
        /// local端设置online/offline状态
        /// </summary>
        /// <param name="onLine"></param>
        /// <returns></returns>
        public bool LaunchOnOffLineProcess(bool onLine)
        {
            var initi = scenarioControllers[Scenarios.Intialize_Scenario] as InitializeScenario;
            if (onLine)
            {
                return initi.LaunchOnlineProcess();
            }
            else
            {
                return initi.LaunchOfflineProcess();
            }
        }
        /// <summary>
        /// 向Host端发送更新时间请求
        /// </summary>
        /// <returns></returns>
        public bool LaunchRequestDateTimeProcess()
        {
            var initi = scenarioControllers[Scenarios.Intialize_Scenario] as InitializeScenario;
            return initi.LaunchDateTimeUpdateProcess();
        }
        /// <summary>
        /// 向Host发送display message
        /// </summary>
        /// <param name="messages">消息内容，最多十条</param>
        /// <returns></returns>
        public bool LaunchSendDisplayMessageProcess(string[] messages)
        {
            var eqt = scenarioControllers[Scenarios.Equipment_Terminal_Service] as EqtTerminalService;
            return eqt.SendMessages(messages);
        }
        #endregion
    }
}
