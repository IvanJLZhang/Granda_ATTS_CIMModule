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
using static Granda.ATTS.CIMModule.Scenario.AlarmManagement;
using static Granda.ATTS.CIMModule.Scenario.Clock;
using static Granda.ATTS.CIMModule.Scenario.InitializeScenario;
using static Granda.ATTS.CIMModule.Scenario.RemoteControl;

namespace Granda.ATTS.CIMModule
{
    /// <summary>
    /// CIM模块运行主入口
    /// </summary>
    public class CimModuleForEQT : CimModuleBase, IItializeScenario, IRCSCallBack, IAMSCallBack, IClock
    {
        public CimModuleForEQT(SecsGem secsGem) : base(secsGem)
        {
        }

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
        #endregion
    }
}
