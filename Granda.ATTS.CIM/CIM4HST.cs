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
using Granda.ATTS.CIM.Data.ENUM;
using Granda.ATTS.CIM.Model;
using Granda.ATTS.CIM.Scenario;
using Granda.HSMS;
using System;

namespace Granda.ATTS.CIM
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
    public class CIM4HST : CIMBASE
    {
        #region 构造方法
        /// <summary>
        /// 构造方法， 提供SecsGem参数
        /// </summary>
        /// <param name="secsGem"></param>
        /// <param name="deviceId">设备Id号， 默认为1</param>
        public CIM4HST(SecsHsms secsGem, short deviceId = 1) : base(secsGem, deviceId)
        {
        }
        /// <summary>
        /// 构造方法，提供创建SecsGem所需参数
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="isActive"></param>
        /// <param name="deviceId">设备Id号， 默认为1</param>
        public CIM4HST(string ipAddress, int port, bool isActive, short deviceId = 1) : base(ipAddress, port, isActive, deviceId)
        {
        }
        #endregion

        #region Event
        /// <summary>
        /// Host收到SelectedEquipmnentStatusData响应事件
        /// </summary>
        //public event EventHandler<TEventArgs<string[]>> SelectedEquipmnentStatusDataReceived;
        ///// <summary>
        ///// 接口方法，触发事件，无需调用
        ///// </summary>
        //public override void SelectedEquipmentStatusRequestEvent(string[] data, bool needReply = true)
        //{
        //    base.SelectedEquipmentStatusRequestEvent(data);
        //    SelectedEquipmnentStatusDataReceived?.Invoke(this, new TEventArgs<string[]>(data, needReply));
        //}
        #endregion

        #region host端测试用例
        /// <summary>
        /// Host端设置online/offline状态
        /// </summary>
        /// <param name="onLine">true表示请求在线，反之离线</param>
        /// <returns></returns>
        public bool LaunchOnOffLineProcess(bool onLine)
        {
            var initi = scenarioControllers[Scenarios.Intialize_Scenario] as InitializeScenario;
            if (onLine)
            {
                return initi.LaunchOnlineByHostProcess();
            }
            else
            {
                return initi.LaunchOfflineByHostProcess();
            }
        }
        /// <summary>
        /// host端测试Remote Control场景相关功能
        /// </summary>
        /// <returns></returns>
        public bool LaunchHostCommandProcess(RCMD hostCommand)
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


        ///// <summary>
        ///// Host Request formatted status
        ///// </summary>
        ///// <param name="SFCD"></param>
        ///// <returns></returns>
        //public bool RequestFormattedStatus(int SFCD)
        //{
        //    var dc = scenarioControllers[Scenarios.Data_Collection] as DataCollection;
        //    return dc.RequestFormattedStatus(SFCD);
        //}

        ///// <summary>
        ///// Host requests the new value of Equipment Constants Variables(ECV)
        ///// </summary>
        ///// <returns></returns>
        //public bool EquipmentConstantsRequest()
        //{
        //    var dc = scenarioControllers[Scenarios.Data_Collection] as DataCollection;
        //    return dc.EquipmentConstantsRequest();
        //}
        ///// <summary>
        ///// Host requests Enable or Disable Events
        ///// </summary>
        ///// <param name="CEED">1=>Disable Event, 0=>Enable Event</param>
        ///// <returns></returns>
        //public bool EnableDisableEventRequest(int CEED)
        //{
        //    var dc = scenarioControllers[Scenarios.Data_Collection] as DataCollection;
        //    return dc.EnableDisableEventRequest(CEED);
        //}

        ///// <summary>
        ///// Host端尝试直接进行Recipe管理
        ///// </summary>
        ///// <param name="pptype"></param>
        ///// <param name="UnitId"></param>
        ///// <returns></returns>
        //public bool LaunchCurrentEPPDRequestProcess(PPTYPE pptype, string UnitId)
        //{
        //    var rm = scenarioControllers[Scenarios.Data_Collection] as RecipeManagement;
        //    return rm.LaunchCurrentEPPDRequestProcess(pptype, UnitId);
        //}
        /// <summary>
        /// Host端发送Formatted Process Program Request
        /// </summary>
        /// <returns></returns>
        public bool LaunchFormattedRecipeRequestProcess()
        {
            var rm = scenarioControllers[Scenarios.Data_Collection] as RecipeManagement;
            return rm.LaunchFormattedRecipeRequestProcess();
        }
        #endregion




    }
}
