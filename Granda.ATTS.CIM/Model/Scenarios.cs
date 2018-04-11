using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granda.ATTS.CIM.Model
{
    /// <summary>
    /// 场景类型
    /// </summary>
    public enum Scenarios
    {
        /// <summary>
        /// Default Scenario
        /// </summary>
        Default_Scenario,
        /// <summary>
        /// Intialize Scenario
        /// </summary>
        Intialize_Scenario,
        /// <summary>
        /// Data Collection
        /// </summary>
        Data_Collection,
        /// <summary>
        /// Remote Control
        /// </summary>
        Remote_Control,
        /// <summary>
        /// Cassette Information Download
        /// </summary>
        Cassette_Information_Download,
        /// <summary>
        /// Alarm Management
        /// </summary>
        Alarm_Management,
        /// <summary>
        /// Recipe Management
        /// </summary>
        Recipe_Management,
        /// <summary>
        /// Clock
        /// </summary>
        Clock,
        /// <summary>
        /// Error Message
        /// </summary>
        Error_Message,
        /// <summary>
        /// Equipment Terminal Service
        /// </summary>
        Equipment_Terminal_Service,
        /// <summary>
        /// Operation Normal Sequence
        /// </summary>
        Operation_Normal_Sequence,
        /// <summary>
        /// Operation Abnormal Sequence
        /// </summary>
        Operation_Abnormal_Sequence,
        /// <summary>
        /// Operation Abnormal Sequence MASK
        /// </summary>
        Operation_Abnormal_Sequence_MASK,
    }
}
