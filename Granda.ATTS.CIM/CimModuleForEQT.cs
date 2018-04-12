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
using Granda.ATTS.CIM.Data;
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
    public class CimModuleForEQT : CimModuleBase
    {
        #region 构造方法
        /// <summary>
        /// 构造方法， 提供SecsGem参数
        /// </summary>
        /// <param name="secsGem"></param>
        /// <param name="deviceId">设备Id号， 默认为1<</param>
        public CimModuleForEQT(SecsGem secsGem, short deviceId = 1) : base(secsGem, deviceId)
        {
        }
        /// <summary>
        /// 构造方法，提供创建SecsGem所需参数
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="isActive"></param>
        /// <param name="deviceId">设备Id号， 默认为1</param>
        public CimModuleForEQT(string ipAddress, int port, bool isActive, short deviceId = 1) : base(ipAddress, port, isActive, deviceId)
        {
        }
        #endregion

        #region 公开的方法
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

        /// <summary>
        /// report Glass Process data
        /// </summary>
        /// <returns></returns>
        public bool ReportGlassProcessData()
        {
            var dc = scenarioControllers[Scenarios.Data_Collection] as DataCollection;
            return dc.ReportGlassProcessData();
        }

        /// <summary>
        /// report Lot Process data
        /// </summary>
        /// <returns></returns>
        public bool ReportLotProcessData()
        {
            var dc = scenarioControllers[Scenarios.Data_Collection] as DataCollection;
            return dc.ReportLotProcessData();
        }

        /// <summary>
        /// report Mask Process data
        /// </summary>
        /// <returns></returns>
        public bool ReportMaskProcessData()
        {
            var dc = scenarioControllers[Scenarios.Data_Collection] as DataCollection;
            return dc.ReportMaskProcessData();
        }

        /// <summary>
        /// Equipment Constant Change
        /// </summary>
        /// <returns></returns>
        public bool EquipmentConstantChangeProcess()
        {
            var dc = scenarioControllers[Scenarios.Data_Collection] as DataCollection;
            return dc.EquipmentConstantChangeProcess();
        }

        /// <summary>
        /// local端recipe发生变化时向host发送通知
        /// </summary>
        /// <returns></returns>
        public bool LaunchRecipeChangeProcess()
        {
            var rm = scenarioControllers[Scenarios.Data_Collection] as RecipeManagement;
            return rm.LaunchRecipeChangeProcess();
        }
        #endregion
    }
}
