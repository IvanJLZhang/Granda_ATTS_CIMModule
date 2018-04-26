#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: BaseScenario
// Author: Ivan JL Zhang    Date: 2018/4/3 17:50:13    Version: 1.0.0
// Description: 
//   所有Scenario的基类， 用于定义一些共有的属性
// 
// Revision History: 
// <Author>  		<Date>     	 	<Revision>  		<Modification>
// 	
//----------------------------------------------------------------------------*/
#endregion
using Granda.ATTS.CIM.Model;
using Granda.HSMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granda.ATTS.CIM.Scenario
{
    internal class BaseScenario
    {
        /// <summary>
        /// 场景类型
        /// </summary>
        public Scenarios ScenarioType { get; protected set; }

        /// <summary>
        /// 场景类型名称
        /// </summary>
        public string ScenarioName { get => ScenarioType.ToString().Replace("_", " "); }
        private string _subScenarioName = String.Empty;
        /// <summary>
        /// 场景下功能名称
        /// </summary>
        public string SubScenarioName
        {
            get
            {
                return _subScenarioName;
            }
            set
            {
                _subScenarioName = value;
                CIMBASE.WriteLog(AATS.Log.LogLevel.INFO, "Scenario: " + ScenarioName + ", Function: " + _subScenarioName);
            }
        }

        public SecsMessage PrimaryMessage { get; set; }
        //public SecsTracer secsTracer { get; set; }

        public BaseScenario()
        {
            ScenarioType = Scenarios.Default_Scenario;
        }
    }
}
