#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: CimModuleForEQT
// Author: Ivan JL Zhang    Date: 2018/4/4 10:36:38    Version: 1.0.0
// Description: 
//   用于Host端的CIM模块
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
    /// Host端CIM模块运行主入口
    /// 实现在：
    /// <para>Initialize Scenario,</para>
    /// <para>Remote Control,</para>
    /// <para>Alarm Management,</para>
    /// <para>Clock,</para>
    /// <para>Equipment Terminal Service,</para>
    /// <para>Process Program(Recipe) Management,</para>
    /// <para>Data Collection</para>
    /// 等场景下primary message的处理，以及由Host端发起的消息进程
    /// </summary>
    public class CimModuleForHST : CimModuleBase
    {
        /// <summary>
        /// 构造方法， 提供SecsGem参数
        /// </summary>
        /// <param name="secsGem"></param>
        public CimModuleForHST(SecsGem secsGem) : base(secsGem)
        {
        }
        /// <summary>
        /// 构造方法，提供创建SecsGem所需参数
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="isActive"></param>
        public CimModuleForHST(string ipAddress, int port, bool isActive) : base(ipAddress, port, isActive)
        {
        }

        #region host端测试用例
        /// <summary>
        /// host端测试Remote Control场景相关功能
        /// </summary>
        /// <returns></returns>
        public bool LaunchHostCommandProcess(HostCommand hostCommand)
        {
            //var hostCommand = HostCommand.START;
            var rcs = scenarioControllers[Scenarios.Recipe_Management] as RemoteControl;
            return rcs.LaunchHostCommand(hostCommand);
        }
        /// <summary>
        /// host端测试Alarm开关功能
        /// </summary>
        /// <param name="isEnable">是否关闭Alarm</param>
        /// <returns></returns>
        public bool LaunchAlarmControlProcess(bool isEnable)
        {
            var ams = scenarioControllers[Scenarios.Alarm_Management] as AlarmManagement;
            return ams.LaunchUpdateAlarmProcess(isEnable);
        }
        /// <summary>
        /// host端测试List Alarm Request功能
        /// </summary>
        /// <returns></returns>
        public bool LaunchListAlarmRequestProcess()
        {
            var ams = scenarioControllers[Scenarios.Alarm_Management] as AlarmManagement;
            return ams.LaunchListAlarmRequestProcess();
        }
        /// <summary>
        /// host端向eqt端发送更新时间命令
        /// </summary>
        /// <returns></returns>
        public bool LaunchSetTimeCommandProcess()
        {
            var clock = scenarioControllers[Scenarios.Clock] as Clock;
            return clock.LaunchInstructTimeProcess();
        }

        /// <summary>
        /// 向EQT发送display message
        /// </summary>
        /// <param name="messages">消息内容，最多十条</param>
        /// <returns></returns>
        public bool LaunchSendDisplayMessageProcess(string[] messages)
        {
            var eqt = scenarioControllers[Scenarios.Equipment_Terminal_Service] as EqtTerminalService;
            return eqt.SendMessages(messages, false);
        }
        #endregion
    }
}
