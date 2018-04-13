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

namespace Granda.ATTS.CIM.Scenario
{
    internal class DataCollection : BaseScenario, IScenario
    {
        public DataCollection()
        {
            ScenarioType = Scenarios.Data_Collection;
        }
        IDataCollection dataCollection = new DefaultDataCollection();
        public DataCollection(IDataCollection callback) : this()
        {
            dataCollection = callback;
        }

        #region data collection process
        public void HandleSecsMessage(SecsMessage secsMessage)
        {
            PrimaryMessage = secsMessage;
            switch (PrimaryMessage.GetSFString())
            {
                case "S6F3"://Discrete Variable Data Send
                    SubScenarioName = Resource.DCS_Discrete_Variable_Data_Send;
                    PrimaryMessage.S6F4(0);
                    break;
                case "S2F23":
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
                case "S2F15":
                    break;
                case "S2F37":
                    SubScenarioName = Resource.DCS_Host_Requests_Enable_Disable_Event;
                    handleS2F37();
                    break;
                default:
                    break;
            }
        }
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
            return false;
        }
        /// <summary>
        /// Equipment Constant Change
        /// </summary>
        /// <returns></returns>
        public bool EquipmentConstantChangeProcess()
        {
            SubScenarioName = Resource.DCS_Equipment_Constants_Change;
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>()
            {
                A("DATAID"),
                A("109"),
            });
            // need to be finished.

            var replyMsg = S6F11(ParseItem(stack), 109);
            if (replyMsg != null && replyMsg.GetSFString() == "S6F12")
            {
                var ack = replyMsg.GetCommandValue();
                if (ack == 0)
                    return true;
            }
            return false;
        }

        // S2F23待完成

        /// <summary>
        /// Host requests the value of Status Variables(SV)
        /// </summary>
        /// <returns></returns>
        public bool RequestValueOfSV()
        {
            SubScenarioName = Resource.DCS_Host_request_value_status;
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>()
            {
                A("SVID"),
            });
            // need to be finished.
            var replyMsg = S1F3(ParseItem(stack));
            if (replyMsg != null && replyMsg.GetSFString() == "S1F4")
            {
                dataCollection.ReceivedSelectedEquipmnentStatusData(GetData(replyMsg.SecsItem));
                return true;
            }
            return false;
        }
        /// <summary>
        /// Host Request formatted status
        /// </summary>
        /// <param name="SFCD"></param>
        /// <returns></returns>
        public bool RequestFormattedStatus(int SFCD)
        {
            SubScenarioName = Resource.DCS_Host_request_Formatted_status;
            var replyMsg = S1F5(SFCD);
            if (replyMsg != null && replyMsg.GetSFString() == "S1F6")
            {
                // need to be finished
                dataCollection.ReceivedFormattedStatusData(replyMsg.SecsItem);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Host requests the new value of Equipment Constants Variables(ECV)
        /// </summary>
        /// <returns></returns>
        public bool EquipmentConstantsRequest()
        {
            SubScenarioName = Resource.DCS_Equipment_Constants_Request;
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>()
            {
                A("ECID"),
            });
            // need to be finished.
            var replyMsg = S2F13(ParseItem(stack));
            if (replyMsg != null && replyMsg.GetSFString() == "S2F14")
            {
                // need to be finished
                dataCollection.ReceivedFormattedStatusData(replyMsg.SecsItem);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Host requests Enable or Disable Events
        /// </summary>
        /// <param name="CEED">1=>Disable Event, 0=>Enable Event</param>
        /// <returns></returns>
        public bool EnableDisableEventRequest(int CEED)
        {
            SubScenarioName = Resource.DCS_Host_Requests_Enable_Disable_Event;
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>()
            {
                A(CEED.ToString()),
            });
            // need to be finished.

            var replyMsg = S2F37(ParseItem(stack));
            if (replyMsg != null && replyMsg.GetSFString() == "S2F38")
            {
                var ack = replyMsg.GetCommandValue();
                if (ack == 0)
                    return true;
            }
            return false;
        }
        #endregion

        #region message handle methods
        void handleS1F3()
        {
            var requestData = GetData(PrimaryMessage.SecsItem);
            // need to be finished;

            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>()
            {
                A("SV"),
            });
            PrimaryMessage.S1F4(ParseItem(stack));
        }

        void handleS1F5()
        {
            var sfcd = PrimaryMessage.GetCommandValue();
            // need to be finished
            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>()
            {
                A(sfcd.ToString()),
            });

            PrimaryMessage.S1F6(ParseItem(stack));
        }

        void handleS2F13()
        {
            var requestData = GetData(PrimaryMessage.SecsItem);
            // need to be finished;

            var stack = new Stack<List<Item>>();
            stack.Push(new List<Item>()
            {
                A("SV"),
            });
            PrimaryMessage.S2F14(ParseItem(stack));
        }

        void handleS2F37()
        {
            var requestData = GetData(PrimaryMessage.SecsItem);
            var ceed = PrimaryMessage.GetCommandValue();
            if (requestData.Length >= 2)
            {
                var ceids = requestData.Skip(1).Take(requestData.Length - 1).ToArray();
                dataCollection.UpdateEventStatus(ceed == 0, ceids);
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
                    if (item.Count == 1)
                        result.Add(item.GetString());
                    else
                        result.AddRange(GetData(item));
                }
            }
            return result.ToArray();
        }
        #endregion

        public interface IDataCollection
        {
            void ReceivedSelectedEquipmnentStatusData(string[] data);
            void ReceivedFormattedStatusData(object data);
            void UpdateEventStatus(bool enable, string[] ceidArr);
        }

        private class DefaultDataCollection : IDataCollection
        {
            public void ReceivedFormattedStatusData(object data)
            {

            }

            public void ReceivedSelectedEquipmnentStatusData(string[] data)
            {
                Debug.WriteLine("");
            }

            public void UpdateEventStatus(bool enable, string[] ceidArr)
            {
            }
        }
    }
}
