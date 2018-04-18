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
using Granda.ATTS.CIM.Data.ENUM;
using Granda.ATTS.CIM.Data.Message;
using Granda.ATTS.CIM.Data.Report;
using Granda.ATTS.CIM.Model;
using Granda.ATTS.CIM.Scenario;
using Secs4Net;

namespace Granda.ATTS.CIM
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
    public class CIM4EQT : CIMBASE
    {
        #region 构造方法
        /// <summary>
        /// 构造方法， 提供SecsGem参数
        /// </summary>
        /// <param name="secsGem">HSMS通信模块</param>
        /// <param name="deviceId">设备Id号， 默认为1</param>
        public CIM4EQT(SecsGem secsGem, short deviceId = 1) : base(secsGem, deviceId)
        {
        }
        /// <summary>
        /// 构造方法，提供创建SecsGem所需参数
        /// </summary>
        /// <param name="ipAddress">Server端IP地址</param>
        /// <param name="port">Server端端口号</param>
        /// <param name="isActive">是否为主动模式</param>
        /// <param name="deviceId">设备Id号， 默认为1</param>
        public CIM4EQT(string ipAddress, int port, bool isActive, short deviceId = 1) : base(ipAddress, port, isActive, deviceId)
        {
        }
        #endregion

        #region 公开的方法

        #region Initialization
        /// <summary>
        /// local端设置online/offline状态
        /// </summary>
        /// <param name="onLine">true表示请求在线，反之离线</param>
        /// <param name="equipmentInfo">设备当前信息</param>
        /// <returns></returns>
        public bool LaunchOnOffLineProcess(bool onLine, EquipmentInfo equipmentInfo)
        {
            var initi = scenarioControllers[Scenarios.Intialize_Scenario] as InitializeScenario;
            if (onLine)
            {
                return initi.LaunchOnlineProcess(equipmentInfo);
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
        #endregion

        #region Equipment Terminal Service
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

        #region Data Collection
        /// <summary>
        /// report Glass Process data
        /// </summary>
        /// <returns></returns>
        public bool LaunchProcessResultReportProcess(ProcessResultReport report)
        {
            var dc = scenarioControllers[Scenarios.Data_Collection] as DataCollection;
            return dc.LaunchProcessResultReportProcess(report);
        }

        /// <summary>
        /// S6F1: Trace Data Send
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public bool LaunchTraceDataInitializationReportProcess(TraceDataInitializationReport report)
        {
            var dc = scenarioControllers[Scenarios.Data_Collection] as DataCollection;
            return dc.LaunchTraceDataInitializationReportProcess(report);
        }

        /// <summary>
        /// S1F4: Selected Equipment Status Data report
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public bool LaunchSelectedEquipmentStatusReportProcess(string[] report)
        {
            var dc = scenarioControllers[Scenarios.Data_Collection] as DataCollection;
            return dc.LaunchSelectedEquipmentStatusReportProcess(report);
        }
        /// <summary>
        /// Formatted Status Report
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public bool LaunchFormattedStatusReportProcess(FormattedStatusDataReport report)
        {
            var dc = scenarioControllers[Scenarios.Data_Collection] as DataCollection;
            return dc.LaunchFormattedStatusReportProcess(report);
        }
        /// <summary>
        /// Equipment Constants data report
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public bool LaunchEquipmentConstantChangeReportProcess(EquipmentConstantChangeReport report)
        {
            var dc = scenarioControllers[Scenarios.Data_Collection] as DataCollection;
            return dc.LaunchEquipmentConstantChangeReportProcess(report);
        }

        /// <summary>
        /// Equipment Constants Data Report
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public bool LaunchEquipmentConstantsDataReportProcess(string[] report)
        {
            var dc = scenarioControllers[Scenarios.Data_Collection] as DataCollection;
            return dc.LaunchEquipmentConstantsDataReportProcess(report);
        }
        #endregion

        #region Process Program Management
        /// <summary>
        /// local端recipe发生变化时向host发送通知
        /// </summary>
        /// <returns></returns>
        public bool LaunchRecipeChangeProcess(RecipeChangeReport report)
        {
            var rm = scenarioControllers[Scenarios.Data_Collection] as RecipeManagement;
            return rm.LaunchRecipeChangeReportProcess(report);
        }

        /// <summary>
        /// 对CurrentEPPDRequest的回复
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public bool LaunchCurrentEPPDReportProcess(CurrentEPPDReport report)
        {
            var rm = scenarioControllers[Scenarios.Data_Collection] as RecipeManagement;
            return rm.LaunchCurrentEPPDReportProcess(report);
        }

        /// <summary>
        /// 对 Formatted Process Program Request 的回复
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public bool LaunchFormattedProcessProgramReport(FormattedProcessProgramReport report)
        {
            var rm = scenarioControllers[Scenarios.Data_Collection] as RecipeManagement;
            return rm.LaunchFormattedProcessProgramReport(report);
        }
        #endregion

        #region Alarm Management
        /// <summary>
        /// 对RequestAlarmList的回复
        /// </summary>
        /// <param name="currentAlarmListReport"></param>
        /// <returns></returns>
        public bool LaunchCurrentAlarmListReport(CurrentAlarmListReport currentAlarmListReport)
        {
            var am = scenarioControllers[Scenarios.Alarm_Management] as AlarmManagement;
            return am.LaunchCurrentAlarmListReport(currentAlarmListReport);
        }
        #endregion

        #region Remote Control
        /// <summary>
        /// Remote Control Scenario: 
        /// Process (Start/Cancel/Abort/Pause/Resume/Operator Call) 
        /// Report
        /// </summary>
        /// <param name="rcmd"></param>
        /// <param name="processLaunchReport"></param>
        /// <param name="equipmentInfo"></param>
        /// <returns></returns>
        public bool LaunchProcessReport(RCMD rcmd, ProcessLaunchReport processLaunchReport, EquipmentInfo equipmentInfo)
        {
            var rc = scenarioControllers[Scenarios.Remote_Control] as RemoteControl;
            return rc.LaunchProcessReport(rcmd, processLaunchReport, equipmentInfo.EquipmentBase);
        }
        #endregion

        #endregion
    }
}
