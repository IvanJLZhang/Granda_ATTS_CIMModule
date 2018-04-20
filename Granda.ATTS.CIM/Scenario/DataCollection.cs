#region 文件说明
/*------------------------------------------------------------------------------
// Copyright © 2018 Granda. All Rights Reserved.
// 苏州广林达电子科技有限公司 版权所有
//------------------------------------------------------------------------------
// File Name: DataCollectionScenario
// Author: Ivan JL Zhang    Date: 2018/4/3 12:02:59    Version: 1.0.0
// Description: 
//   用于收集Equipment Information场景
// 
// Revision History: 
// <Author>  		<Date>     	 	<Revision>  		<Modification>
// 	
//----------------------------------------------------------------------------*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granda.ATTS.CIM.Extension;
using Granda.ATTS.CIM.Model;
using Secs4Net;
using static Secs4Net.Item;
using static Granda.ATTS.CIM.StreamType.Stream6_DataCollection;
using static Granda.ATTS.CIM.StreamType.Stream1_EquipmentStatus;
using static Granda.ATTS.CIM.StreamType.Stream2_EquipmentControl;
using static Granda.ATTS.CIM.Extension.SmlExtension;
using System.Diagnostics;
using Granda.ATTS.CIM.Data;
using Granda.ATTS.CIM.Data.Report;
using Granda.ATTS.CIM.Data.Message;
using Granda.ATTS.CIM.Data.ENUM;

namespace Granda.ATTS.CIM.Scenario
{
    internal class DataCollection : BaseScenario, IScenario
    {
        #region 构造方法和变量
        public DataCollection()
        {
            ScenarioType = Scenarios.Data_Collection;
        }
        IDataCollection dataCollection = new DefaultDataCollection();
        public DataCollection(IDataCollection callback) : this()
        {
            dataCollection = callback;
        }
        #endregion

        #region message handle methods
        public void HandleSecsMessage(SecsMessage secsMessage)
        {
            PrimaryMessage = secsMessage;
            switch (PrimaryMessage.GetSFString())
            {
                case "S6F3"://Discrete Variable Data Send
                    SubScenarioName = Resource.DCS_Discrete_Variable_Data_Send;
                    PrimaryMessage.S6F4(0);
                    break;
                case "S2F23":// trace data initialization request
                    SubScenarioName = Resource.DCS_Host_Initiates_Trace_Report;
                    PrimaryMessage.S2F24("0");
                    TraceDataInitializationRequest traceDataInitializationRequest = new TraceDataInitializationRequest();
                    traceDataInitializationRequest.Parse(PrimaryMessage.SecsItem);
                    dataCollection.TraceDataInitializationRequestEvent(traceDataInitializationRequest);
                    break;
                case "S6F1":
                    break;
                case "S1F3":// Selected Equipment Status Request
                    SubScenarioName = Resource.DCS_Host_request_value_status;
                    handleS1F3();
                    break;
                case "S1F5":// Request formatted status
                    SubScenarioName = Resource.DCS_Host_request_Formatted_status;
                    handleS1F5();
                    break;
                case "S2F13":// equipment constants request
                    SubScenarioName = Resource.DCS_Equipment_Constants_Request;
                    handleS2F13();
                    break;
                case "S2F15":// X
                    break;
                case "S2F37":// host request enable or disable events
                    SubScenarioName = Resource.DCS_Host_Requests_Enable_Disable_Event;
                    handleS2F37();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Selected equipment Status request
        /// </summary>
        void handleS1F3()
        {
            var requestData = GetData(PrimaryMessage.SecsItem);
            dataCollection.SelectedEquipmentStatusRequestEvent(requestData, true);
        }
        /// <summary>
        /// Host requests the formatted status
        /// </summary>
        void handleS1F5()
        {
            var sfcd = PrimaryMessage.GetCommandValue();
            dataCollection.FormattedStatusRequestEvent((SFCD)sfcd, true);
        }
        /// <summary>
        /// Equipment constants request
        /// </summary>
        void handleS2F13()
        {
            var requestData = GetData(PrimaryMessage.SecsItem);
            dataCollection.EquipmentConstantsRequestEvent(requestData, true);
        }
        /// <summary>
        /// Host requests Enable or Disable events
        /// </summary>
        void handleS2F37()
        {
            var requestData = GetData(PrimaryMessage.SecsItem);
            var ceed = PrimaryMessage.GetCommandValue();
            if (requestData.Length >= 2)
            {
                var ceids = requestData.Skip(1).Take(requestData.Length - 1).ToArray();
                dataCollection.EnableDisableEventReportRequestEvent(requestData);
            }

            PrimaryMessage.S2F38(0);
        }

        string[] GetData(Item itemList)
        {
            List<string> result = new List<string>();
            if (itemList != null)
            {
                foreach (var item in itemList.Items)
                {
                    if (item.Format == SecsFormat.ASCII)
                    {
                        result.Add(item.GetString());
                    }
                    else if (item.Format == SecsFormat.List)
                    {
                        result.AddRange(GetData(item));
                    }
                }
            }
            return result.ToArray();
        }
        #endregion

        #region data collection process
        /// <summary>
        /// Glass/Lot/Mask Process result report
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public bool LaunchProcessResultReportProcess(IReport report)
        {
            SubScenarioName = Resource.DCS_Discrete_Variable_Data_Send;
            var processResult = (ProcessResultReport)report;
            var replyMsg = S6F3(report.SecsItem, (int)processResult.CEID);
            if (replyMsg != null && replyMsg.GetSFString() == "S6F4")
            {
                var ack = replyMsg.GetCommandValue();
                if (ack == 0)
                    return true;
            }
            CIMBASE.WriteLog(AATS.Log.LogLevel.ERROR, "something wrong was happened when sending process result report");
            return false;
        }
        /// <summary>
        /// S6F1: Trace Data Send
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public bool LaunchTraceDataInitializationReportProcess(IReport report)
        {
            SubScenarioName = Resource.DCS_Host_Initiates_Trace_Report;
            S6F1(report.SecsItem);
            return true;
        }
        /// <summary>
        /// S1F4: Selected Equipment Status Data report
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public bool LaunchSelectedEquipmentStatusReportProcess(string[] report)
        {
            SubScenarioName = Resource.DCS_Host_request_value_status;
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>());
            foreach (var item in report)
            {
                stack.Peek().Add(A(item));
            }
            PrimaryMessage.S1F4(ParseItem(stack));
            return true;
        }
        /// <summary>
        /// Formatted Status Report
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public bool LaunchFormattedStatusReportProcess(IReport report)
        {
            SubScenarioName = Resource.DCS_Host_request_Formatted_status;
            PrimaryMessage.S1F6(report.SecsItem);
            return true;
        }
        /// <summary>
        /// Equipment Constants data report
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public bool LaunchEquipmentConstantChangeReportProcess(IReport report)
        {
            SubScenarioName = Resource.DCS_Host_request_value_status;
            var replyMsg = S6F11(report.SecsItem, 109);
            if (replyMsg != null && replyMsg.GetSFString() == "S6F12")
            {
                var ack = replyMsg.GetCommandValue();
                if (ack == 0)
                    return true;
            }
            CIMBASE.WriteLog(AATS.Log.LogLevel.ERROR, "something wrong was happened when send Epuipment Constanrs Report message.");
            return false;
        }
        /// <summary>
        /// Equipment Constants Data Report
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public bool LaunchEquipmentConstantsDataReportProcess(string[] report)
        {
            SubScenarioName = Resource.DCS_Equipment_Constants_Request;
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>());
            foreach (var item in report)
            {
                stack.Peek().Add(A(item));
            }
            PrimaryMessage.S2F14(ParseItem(stack));
            return true;
        }
        #region host message
        ///// <summary>
        ///// Host requests the value of Status Variables(SV)
        ///// </summary>
        ///// <returns></returns>
        //public bool RequestValueOfSV()
        //{
        //    SubScenarioName = Resource.DCS_Host_request_value_status;
        //    var stack = new Stack<List<Item>>();
        //    stack.Push(new List<Item>()
        //    {
        //        A("SVID"),
        //    });
        //    // need to be finished.
        //    var replyMsg = S1F3(ParseItem(stack));
        //    if (replyMsg != null && replyMsg.GetSFString() == "S1F4")
        //    {
        //        dataCollection.SelectedEquipmentStatusRequestEvent(GetData(replyMsg.SecsItem));
        //        return true;
        //    }
        //    return false;
        //}
        ///// <summary>
        ///// Host Request formatted status
        ///// </summary>
        ///// <param name="SFCD"></param>
        ///// <returns></returns>
        //public bool RequestFormattedStatus(int SFCD)
        //{
        //    SubScenarioName = Resource.DCS_Host_request_Formatted_status;
        //    var replyMsg = S1F5(SFCD);
        //    if (replyMsg != null && replyMsg.GetSFString() == "S1F6")
        //    {
        //        // need to be finished
        //        dataCollection.ReceivedFormattedStatusData(replyMsg.SecsItem);
        //        return true;
        //    }
        //    return false;
        //}
        ///// <summary>
        ///// Host requests the new value of Equipment Constants Variables(ECV)
        ///// </summary>
        ///// <returns></returns>
        //public bool EquipmentConstantsRequest()
        //{
        //    SubScenarioName = Resource.DCS_Equipment_Constants_Request;
        //    var stack = new Stack<List<Item>>();
        //    stack.Push(new List<Item>()
        //    {
        //        A("ECID"),
        //    });
        //    // need to be finished.
        //    var replyMsg = S2F13(ParseItem(stack));
        //    if (replyMsg != null && replyMsg.GetSFString() == "S2F14")
        //    {
        //        // need to be finished
        //        dataCollection.ReceivedFormattedStatusData(replyMsg.SecsItem);
        //        return true;
        //    }
        //    return false;
        //}
        ///// <summary>
        ///// Host requests Enable or Disable Events
        ///// </summary>
        ///// <param name="CEED">1=>Disable Event, 0=>Enable Event</param>
        ///// <returns></returns>
        //public bool EnableDisableEventRequest(int CEED)
        //{
        //    SubScenarioName = Resource.DCS_Host_Requests_Enable_Disable_Event;
        //    var stack = new Stack<List<Item>>();
        //    stack.Push(new List<Item>()
        //    {
        //        A(CEED.ToString()),
        //    });
        //    // need to be finished.

        //    var replyMsg = S2F37(ParseItem(stack));
        //    if (replyMsg != null && replyMsg.GetSFString() == "S2F38")
        //    {
        //        var ack = replyMsg.GetCommandValue();
        //        if (ack == 0)
        //            return true;
        //    }
        //    return false;
        //}
        #endregion

        #endregion

        #region 接口默认实例类
        private class DefaultDataCollection : IDataCollection
        {
            public void EquipmentConstantsRequestEvent(string[] data, bool needReply = true)
            {
                throw new NotImplementedException();
            }

            public void FormattedStatusRequestEvent(SFCD sfcd, bool needReply = true)
            {
                throw new NotImplementedException();
            }

            public void ReceivedFormattedStatusData(object data, bool needReply = true)
            {

            }

            public void SelectedEquipmentStatusRequestEvent(string[] data, bool needReply = true)
            {
                Debug.WriteLine("");
            }

            public void TraceDataInitializationRequestEvent(TraceDataInitializationRequest traceDataInitializationRequest, bool needReply = true)
            {
                throw new NotImplementedException();
            }

            public void EnableDisableEventReportRequestEvent(string[] ceidArr)
            {
            }
        }
        #endregion
    }

    #region 接口
    /// <summary>
    /// Data Collection 回调方法接口
    /// </summary>
    public interface IDataCollection
    {
        /// <summary>
        /// Selected Equipment Status Request
        /// </summary>
        void SelectedEquipmentStatusRequestEvent(string[] data, bool needReply = true);
        /// <summary>
        /// Equipment Constants Request
        /// </summary>
        void EquipmentConstantsRequestEvent(string[] data, bool needReply = true);
        /// <summary>
        /// Formatted Status Request
        /// </summary>
        void FormattedStatusRequestEvent(SFCD sfcd, bool needReply = true);
        /// <summary>
        /// Enable Disable Event Report
        /// </summary>
        void EnableDisableEventReportRequestEvent(string[] ceidArr);
        /// <summary>
        /// Trace Data Initialization Request
        /// </summary>
        void TraceDataInitializationRequestEvent(TraceDataInitializationRequest traceDataInitializationRequest, bool needReply = true);
    }
    #endregion
}
