using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;
using Granda.ATTS.CIM;
using Granda.ATTS.CIM.Data.ENUM;
using Granda.ATTS.CIM.Data.Message;
using Granda.ATTS.CIM.Data.Report;
using Granda.ATTS.CIM.Scenario;
using Granda.HSMS;

namespace CIMClient
{
    public partial class Form1 : Form,
        IInitializeScenario,
        IRCSCallBack,
        IEqtTerminalService,
        IRecipeManagement,
        IClock,
        IAMSCallBack,
        IDataCollection
    {
        CIM4EQT cimClient = null;
        EquipmentInfo _equipmentInfo;
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _equipmentInfo = new EquipmentInfo()
            {
                CRST = CRST.O,
                EQST = CommonStatus.I,
                EQSTCODE = 1000,
                MDLN = "equipment",
                SOFTREV = "v1.0.",
            };
            AddLog(_equipmentInfo.ToString());
        }
        private void AddLog(string msg)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                this.tb_msg.Text += msg + "\r\n";
            });
        }
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.tb_msg.Text = String.Empty;
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            SecsGem secsGem = new SecsGem(this.rbtn_active.Checked, IPAddress.Parse(this.tb_ip.Text.Trim()), Int32.Parse(this.tb_port.Text));
            cimClient = new CIM4EQT(secsGem, 1000);
            cimClient.ConnectionChanged += CimClient_ConnectionChanged;
            cimClient.ScenarioInitialize(this, this, this, this, this, this, this);
        }

        private void CimClient_ConnectionChanged(object sender, CIMEventArgs<ConnectionStatus> e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.lb_connectionState.Text = e.Data.ToString();
                AddLog("connection Changed: " + e.Data.ToString());
            });
        }

        #region Initialization Scenario
        private void btn_i_online_Click(object sender, EventArgs e)
        {
            cimClient?.LaunchOnOffLineProcess(true, _equipmentInfo);
        }

        private void btn_i_offline_Click(object sender, EventArgs e)
        {
            cimClient?.LaunchOnOffLineProcess(false, _equipmentInfo);
        }

        private void btn_i_datetime_Click(object sender, EventArgs e)
        {
            cimClient?.LaunchRequestDateTimeProcess();
        }

        public void UpdateControlState(CRST controlState)
        {
            this._equipmentInfo.CRST = controlState;
            AddLog("Control State Change: " + this._equipmentInfo.CRST);
        }

        public void UpdateDateTime(string dateTimeStr)
        {
            AddLog("update date time: " + dateTimeStr);
        }

        #endregion

        #region Equipment Terminal Service
        private void btn_e_send_display_message_Click(object sender, EventArgs e)
        {
            cimClient?.LaunchSendDisplayMessageProcess(new string[]
            {
                "Test message",
                "Test message1",
            });
        }
        public void ReceiveTestMessage(string[] messages)
        {
            foreach (var item in messages)
            {
                AddLog("Receive Display Message: " + item);
            }
        }

        public void SendMessageDone(string[] messages)
        {
            foreach (var item in messages)
            {
                AddLog("Send Display Message: " + item);
            }
        }
        #endregion

        #region Remote Control
        private void btn_r_process_control_Click(object sender, EventArgs e)
        {
            ProcessLaunchReport processLaunchReport = new ProcessLaunchReport()
            {
                CSTID = "CST001",
                LOTID = "20180418",
                PPID = "Test Recipe",
                PTID = "P01",
                PTTYPE = PTTYPE.PTTYPELIST["Load Port"],
                PTUSETYPE = PTUSETYPE.PTUSETYPELIST["Good"],
            };
            cimClient?.LaunchProcessReport(RCMD.START, processLaunchReport, this._equipmentInfo);
        }

        public void RemoteControlCommandRequestEvent(RemoteControlCommandRequest remoteControlCommandJob)
        {
            AddLog(remoteControlCommandJob.ToString());
        }
        #endregion

        #region Recipe Managment
        private void btn_rc_rcr_Click(object sender, EventArgs e)
        {
            RecipeChangeReport recipeChangeReport = new RecipeChangeReport();
            recipeChangeReport.EquipmentStatus = _equipmentInfo.EquipmentStatus;
            recipeChangeReport.PPID = "Test Recipe";
            recipeChangeReport.PPTYPE = PPTYPE.U;
            recipeChangeReport.PPCINFO = PPCINFO.Created;
            recipeChangeReport.LCTIME = DateTime.Now.ToString("yyyyMMddHHmmss");

            recipeChangeReport.ProcessCommandList = new ProcessCommands();

            #region process Command List1
            ProcessCommands processCommands = new ProcessCommands();
            processCommands.CCODE = "1";
            processCommands.RCPSTEP = "VacTR.PR2";
            processCommands.UNITID = "Unit1";
            processCommands.SUNITID = "SUnit1";
            processCommands.ParameterList = new Parameters();
            processCommands.ParameterList.Add(new Parameters()
            {
                PPARMNAME = "param1",
                PPARMVALUE = "value1",
            });
            processCommands.ParameterList.Add(new Parameters()
            {
                PPARMNAME = "param2",
                PPARMVALUE = "value2",
            });
            processCommands.ParameterList.Add(new Parameters()
            {
                PPARMNAME = "param3",
                PPARMVALUE = "value3",
            });
            recipeChangeReport.ProcessCommandList.Add(processCommands);

            processCommands = new ProcessCommands();
            processCommands.CCODE = "2";
            processCommands.RCPSTEP = "VacTR.PR2";
            processCommands.UNITID = "Unit1";
            processCommands.SUNITID = "SUnit1";
            processCommands.ParameterList = new Parameters();
            processCommands.ParameterList.Add(new Parameters()
            {
                PPARMNAME = "param1",
                PPARMVALUE = "value1",
            });
            processCommands.ParameterList.Add(new Parameters()
            {
                PPARMNAME = "param2",
                PPARMVALUE = "value2",
            });
            processCommands.ParameterList.Add(new Parameters()
            {
                PPARMNAME = "param3",
                PPARMVALUE = "value3",
            });
            recipeChangeReport.ProcessCommandList.Add(processCommands);
            #endregion

            cimClient?.LaunchRecipeChangeProcess(recipeChangeReport);
        }

        public void CurrentEPPDRequestEvent(CurrentEPPDRequest currentEPPDRequest, bool needReply = true)
        {
            AddLog(currentEPPDRequest.ToString());
            if (!needReply)
                return;

            cimClient?.LaunchCurrentEPPDReportProcess(new CurrentEPPDReport()
            {
                UNITID = currentEPPDRequest.UNITID,
                PPTYPE = currentEPPDRequest.PPTYPE,
                PPIDLIST = new List<string>()
                {
                    "Test Recipe1",
                    "Test Recipe2",
                    "Test Recipe3",
                    "Test Recipe4",
                },
            });
        }

        public void FormattedProcessProgramRequestEvent(FormattedProcessProgramRequest formattedProcessProgramRequest, bool needReply = true)
        {
            AddLog(formattedProcessProgramRequest.ToString());
            if (!needReply)
                return;
            FormattedProcessProgramReport report = new FormattedProcessProgramReport();
            report.PPID = formattedProcessProgramRequest.PPID;
            report.PPTYPE = formattedProcessProgramRequest.PPTYPE;
            report.EquipmentBaseInfo = _equipmentInfo.EquipmentBase;
            report.LCTIME = DateTime.Now.ToString("yyyyMMddHHmmss");
            report.ProcessCommandList = new ProcessCommands();

            #region process Command List1
            ProcessCommands processCommands = new ProcessCommands();
            processCommands.CCODE = "1";
            processCommands.RCPSTEP = "VacTR.PR2";
            processCommands.UNITID = "Unit1";
            processCommands.SUNITID = "SUnit1";
            processCommands.ParameterList = new Parameters();
            processCommands.ParameterList.Add(new Parameters()
            {
                PPARMNAME = "param1",
                PPARMVALUE = "value1",
            });
            processCommands.ParameterList.Add(new Parameters()
            {
                PPARMNAME = "param2",
                PPARMVALUE = "value2",
            });
            processCommands.ParameterList.Add(new Parameters()
            {
                PPARMNAME = "param3",
                PPARMVALUE = "value3",
            });
            report.ProcessCommandList.Add(processCommands);

            processCommands = new ProcessCommands();
            processCommands.CCODE = "2";
            processCommands.RCPSTEP = "VacTR.PR2";
            processCommands.UNITID = "Unit1";
            processCommands.SUNITID = "SUnit1";
            processCommands.ParameterList = new Parameters();
            processCommands.ParameterList.Add(new Parameters()
            {
                PPARMNAME = "param1",
                PPARMVALUE = "value1",
            });
            processCommands.ParameterList.Add(new Parameters()
            {
                PPARMNAME = "param2",
                PPARMVALUE = "value2",
            });
            processCommands.ParameterList.Add(new Parameters()
            {
                PPARMNAME = "param3",
                PPARMVALUE = "value3",
            });
            report.ProcessCommandList.Add(processCommands);
            #endregion

            cimClient?.LaunchFormattedProcessProgramReport(report);
        }
        #endregion

        #region Data Collection
        private void btn_d_prr_Click(object sender, EventArgs e)
        {
            ProcessResultReport report = new ProcessResultReport();
            report.CEID = PTYPE.Glass;
            report.UNITID = "1AED06-IND";
            report.SUNITID = "";
            report.LOTID = "TestLot001";
            report.CSTID = "CST001";
            report.GLSID = "TestGlass001";
            report.OPERID = "Opper001";
            report.PRODID = "NORMALPROD";
            report.PPID = "TestRecipe001";

            #region
            DVNAMES dvnameList = new DVNAMES();
            dvnameList.Add(new DVNAMES()
            {
                DVNAME = "Inspection Result",
                SITENAMELIST = new Func<SITENAMES>(() =>
                {
                    SITENAMES SiteNameList = new SITENAMES();
                    SiteNameList.Add(new SITENAMES() { DV = "data value 1", SITENAME = "G", });
                    SiteNameList.Add(new SITENAMES() { DV = "data value 2", SITENAME = "H", });
                    return SiteNameList;
                })(),
            });
            dvnameList.Add(new DVNAMES()
            {
                DVNAME = "Inspection Result1",
                SITENAMELIST = new Func<SITENAMES>(() =>
                {
                    SITENAMES SiteNameList = new SITENAMES();
                    SiteNameList.Add(new SITENAMES() { DV = "data value 1", SITENAME = "G", });
                    SiteNameList.Add(new SITENAMES() { DV = "data value 2", SITENAME = "H", });
                    return SiteNameList;
                })(),
            });
            report.DVNAMELIST = dvnameList;
            #endregion
            cimClient?.LaunchProcessResultReportProcess(report);
        }

        private void btn_d_ecc_Click(object sender, EventArgs e)
        {
            EquipmentConstantChangeReport report = new EquipmentConstantChangeReport();
            report.EquipmentStatus = _equipmentInfo.EquipmentStatus;
            report.ECIDLIST = (new Func<ECIDDatas>(() =>
            {
                ECIDDatas eciddataList = new ECIDDatas();
                eciddataList.Add(new ECIDDatas()
                {
                    ECID = "005",
                    ECV = "10",
                });
                eciddataList.Add(new ECIDDatas()
                {
                    ECID = "006",
                    ECV = "10",
                });
                eciddataList.Add(new ECIDDatas()
                {
                    ECID = "007",
                    ECV = "10",
                });
                return eciddataList;
            })());
            cimClient?.LaunchEquipmentConstantChangeReportProcess(report);
        }
        public void SelectedEquipmentStatusRequestEvent(string[] data, bool needReply = true)
        {
            if (!needReply)
                return;
            // 需要根据data中的SVID查询得到SV的值
            cimClient?.LaunchSelectedEquipmentStatusReportProcess(new string[]
            {
                "0.015",
                "0.015",
            });
        }

        public void EquipmentConstantsRequestEvent(string[] data, bool needReply = true)
        {
            if (!needReply)
                return;
            // 需要根据data中的ECID查询得到ECV的值
            cimClient?.LaunchEquipmentConstantsDataReportProcess(new string[] {
                "10",
                "11",
            });
        }

        public void FormattedStatusRequestEvent(SFCD sfcd, bool needReply = true)
        {
            if (!needReply)
                return;
            FormattedStatusDataReport report = new FormattedStatusDataReport();
            report.SFCD = sfcd;
            switch (sfcd)
            {
                case SFCD.EquipmentStatus:
                    report.EquipmentStatus = _equipmentInfo.EquipmentStatus;
                    break;
                case SFCD.PortStatus:
                    report.PortStatusDataList = new PortStatusDatas();
                    report.PortStatusDataList.Add(new PortStatusDatas()
                    {
                        PTID = "01",
                        PTTYPE = PTTYPE.PTTYPELIST["Both Port(Load/Unload)"],
                        PTUSETYPE = PTUSETYPE.PTUSETYPELIST["Normal using type"],
                        TRSMODE = TRSMODE.AUTO,
                        PTST = PTST.LoadRequest,
                        CSTID = "CST001",
                        LOTID = "TestLot001",
                        PPID = "TestRecipe001",
                        SLOTINFO = SLOTINFO.Empty,
                    });
                    report.PortStatusDataList.Add(new PortStatusDatas()
                    {
                        PTID = "02",
                        PTTYPE = PTTYPE.PTTYPELIST["Both Port(Load/Unload)"],
                        PTUSETYPE = PTUSETYPE.PTUSETYPELIST["Normal using type"],
                        TRSMODE = TRSMODE.AUTO,
                        PTST = PTST.LoadRequest,
                        CSTID = "CST001",
                        LOTID = "TestLot001",
                        PPID = "TestRecipe001",
                        SLOTINFO = SLOTINFO.Empty,
                    });
                    report.PortStatusDataList.Add(new PortStatusDatas()
                    {
                        PTID = "03",
                        PTTYPE = PTTYPE.PTTYPELIST["Both Port(Load/Unload)"],
                        PTUSETYPE = PTUSETYPE.PTUSETYPELIST["Normal using type"],
                        TRSMODE = TRSMODE.AUTO,
                        PTST = PTST.LoadRequest,
                        CSTID = "CST001",
                        LOTID = "TestLot001",
                        PPID = "TestRecipe001",
                        SLOTINFO = SLOTINFO.Empty,
                    });
                    break;
                case SFCD.OperationMode:
                    report.OPERMODE = "01";
                    report.OPERMODEDESC = "01";
                    break;
                case SFCD.UnitStatus:
                    report.UnitStatusDataList = new UnitStatusDatas();
                    report.UnitStatusDataList.Add(new UnitStatusDatas()
                    {
                        UNITID = "1AED06-IND",
                        UNITST = CommonStatus.R,
                        UNITSTCODE = "301",
                        USLOTNOLIST = new Func<USLOTNOS>(() =>
                        {
                            USLOTNOS uSLOTNOS = new USLOTNOS();
                            uSLOTNOS.Add(new USLOTNOS()
                            {
                                USLOTNO = "0001",
                                GLSID = "TestGlass001",
                            });
                            uSLOTNOS.Add(new USLOTNOS()
                            {
                                USLOTNO = "0002",
                                GLSID = "TestGlass001",
                            });
                            return uSLOTNOS;

                        })(),
                    });
                    report.UnitStatusDataList.Add(new UnitStatusDatas()
                    {
                        UNITID = "1AED06-INE",
                        UNITST = CommonStatus.R,
                        UNITSTCODE = "301",
                        USLOTNOLIST = new Func<USLOTNOS>(() =>
                        {
                            USLOTNOS uSLOTNOS = new USLOTNOS();
                            uSLOTNOS.Add(new USLOTNOS()
                            {
                                USLOTNO = "0001",
                                GLSID = "TestGlass001",
                            });
                            uSLOTNOS.Add(new USLOTNOS()
                            {
                                USLOTNO = "0002",
                                GLSID = "TestGlass001",
                            });
                            return uSLOTNOS;

                        })(),

                    });
                    break;
                case SFCD.SubUnitStatus:
                    break;
                case SFCD.MaskStatus:
                    report.MaskStatusDataList = new MaskStatusDatas();
                    report.MaskStatusDataList.Add(new MaskStatusDatas()
                    {
                        UNITID = "1AED06-IND",
                        MaskStatusList = new Func<MaskStatuss>(() =>
                        {
                            MaskStatuss maskStatuss = new MaskStatuss();
                            maskStatuss.Add(new MaskStatuss()
                            {
                                MASKID = "TestMask001",
                                MASKST = "10",
                                MASKUSECNT = "2",
                                SUNITMaskStatusList = new Func<MaskStatusDatas>(() =>
                                {
                                    MaskStatusDatas maskStatusDatas = new MaskStatusDatas();
                                    maskStatusDatas.Add(new MaskStatusDatas()
                                    {
                                        UNITID = " 1AED06-P01",
                                        MaskStatusList = new Func<MaskStatuss>(() =>
                                        {
                                            MaskStatuss maskStatuss1 = new MaskStatuss();
                                            maskStatuss1.Add(new MaskStatuss()
                                            {
                                                MASKID = "TestMask001",
                                                MASKST = "10",
                                                MASKUSECNT = "2",
                                                SUNITMaskStatusList = null,
                                            });
                                            maskStatuss1.Add(new MaskStatuss()
                                            {
                                                MASKID = "TestMask002",
                                                MASKST = "10",
                                                MASKUSECNT = "2",
                                                SUNITMaskStatusList = null,
                                            });
                                            return maskStatuss1;
                                        })()
                                    });
                                    maskStatusDatas.Add(new MaskStatusDatas()
                                    {
                                        UNITID = " 1AED06-P02",
                                        MaskStatusList = new Func<MaskStatuss>(() =>
                                        {
                                            MaskStatuss maskStatuss1 = new MaskStatuss();
                                            maskStatuss1.Add(new MaskStatuss()
                                            {
                                                MASKID = "TestMask001",
                                                MASKST = "10",
                                                MASKUSECNT = "2",
                                                SUNITMaskStatusList = null,
                                            });
                                            maskStatuss1.Add(new MaskStatuss()
                                            {
                                                MASKID = "TestMask002",
                                                MASKST = "10",
                                                MASKUSECNT = "2",
                                                SUNITMaskStatusList = null,
                                            });
                                            return maskStatuss1;
                                        })()
                                    });
                                    return maskStatusDatas;
                                })(),
                            });
                            maskStatuss.Add(new MaskStatuss()
                            {
                                MASKID = "TestMask002",
                                MASKST = "10",
                                MASKUSECNT = "2",
                                SUNITMaskStatusList = new Func<MaskStatusDatas>(() =>
                                {
                                    MaskStatusDatas maskStatusDatas = new MaskStatusDatas();
                                    maskStatusDatas.Add(new MaskStatusDatas()
                                    {
                                        UNITID = " 1AED06-P01",
                                        MaskStatusList = new Func<MaskStatuss>(() =>
                                        {
                                            MaskStatuss maskStatuss1 = new MaskStatuss();
                                            maskStatuss1.Add(new MaskStatuss()
                                            {
                                                MASKID = "TestMask001",
                                                MASKST = "10",
                                                MASKUSECNT = "2",
                                                SUNITMaskStatusList = null,
                                            });
                                            maskStatuss1.Add(new MaskStatuss()
                                            {
                                                MASKID = "TestMask002",
                                                MASKST = "10",
                                                MASKUSECNT = "2",
                                                SUNITMaskStatusList = null,
                                            });
                                            return maskStatuss1;
                                        })()
                                    });
                                    maskStatusDatas.Add(new MaskStatusDatas()
                                    {
                                        UNITID = " 1AED06-P02",
                                        MaskStatusList = new Func<MaskStatuss>(() =>
                                        {
                                            MaskStatuss maskStatuss1 = new MaskStatuss();
                                            maskStatuss1.Add(new MaskStatuss()
                                            {
                                                MASKID = "TestMask001",
                                                MASKST = "10",
                                                MASKUSECNT = "2",
                                                SUNITMaskStatusList = null,
                                            });
                                            maskStatuss1.Add(new MaskStatuss()
                                            {
                                                MASKID = "TestMask002",
                                                MASKST = "10",
                                                MASKUSECNT = "2",
                                                SUNITMaskStatusList = null,
                                            });
                                            return maskStatuss1;
                                        })()
                                    });
                                    return maskStatusDatas;
                                })(),
                            });
                            return maskStatuss;
                        })(),
                    });
                    report.MaskStatusDataList.Add(new MaskStatusDatas()
                    {
                        UNITID = "1AED06-INE",
                        MaskStatusList = new Func<MaskStatuss>(() =>
                        {
                            MaskStatuss maskStatuss = new MaskStatuss();
                            maskStatuss.Add(new MaskStatuss()
                            {
                                MASKID = "TestMask001",
                                MASKST = "10",
                                MASKUSECNT = "2",
                                SUNITMaskStatusList = new Func<MaskStatusDatas>(() =>
                                {
                                    MaskStatusDatas maskStatusDatas = new MaskStatusDatas();
                                    maskStatusDatas.Add(new MaskStatusDatas()
                                    {
                                        UNITID = " 1AED06-P01",
                                        MaskStatusList = new Func<MaskStatuss>(() =>
                                        {
                                            MaskStatuss maskStatuss1 = new MaskStatuss();
                                            maskStatuss1.Add(new MaskStatuss()
                                            {
                                                MASKID = "TestMask001",
                                                MASKST = "10",
                                                MASKUSECNT = "2",
                                                SUNITMaskStatusList = null,
                                            });
                                            maskStatuss1.Add(new MaskStatuss()
                                            {
                                                MASKID = "TestMask002",
                                                MASKST = "10",
                                                MASKUSECNT = "2",
                                                SUNITMaskStatusList = null,
                                            });
                                            return maskStatuss1;
                                        })()
                                    });
                                    maskStatusDatas.Add(new MaskStatusDatas()
                                    {
                                        UNITID = " 1AED06-P02",
                                        MaskStatusList = new Func<MaskStatuss>(() =>
                                        {
                                            MaskStatuss maskStatuss1 = new MaskStatuss();
                                            maskStatuss1.Add(new MaskStatuss()
                                            {
                                                MASKID = "TestMask001",
                                                MASKST = "10",
                                                MASKUSECNT = "2",
                                                SUNITMaskStatusList = null,
                                            });
                                            maskStatuss1.Add(new MaskStatuss()
                                            {
                                                MASKID = "TestMask002",
                                                MASKST = "10",
                                                MASKUSECNT = "2",
                                                SUNITMaskStatusList = null,
                                            });
                                            return maskStatuss1;
                                        })()
                                    });
                                    return maskStatusDatas;
                                })(),
                            });
                            maskStatuss.Add(new MaskStatuss()
                            {
                                MASKID = "TestMask002",
                                MASKST = "10",
                                MASKUSECNT = "2",
                                SUNITMaskStatusList = new Func<MaskStatusDatas>(() =>
                                {
                                    MaskStatusDatas maskStatusDatas = new MaskStatusDatas();
                                    maskStatusDatas.Add(new MaskStatusDatas()
                                    {
                                        UNITID = " 1AED06-P01",
                                        MaskStatusList = new Func<MaskStatuss>(() =>
                                        {
                                            MaskStatuss maskStatuss1 = new MaskStatuss();
                                            maskStatuss1.Add(new MaskStatuss()
                                            {
                                                MASKID = "TestMask001",
                                                MASKST = "10",
                                                MASKUSECNT = "2",
                                                SUNITMaskStatusList = null,
                                            });
                                            maskStatuss1.Add(new MaskStatuss()
                                            {
                                                MASKID = "TestMask002",
                                                MASKST = "10",
                                                MASKUSECNT = "2",
                                                SUNITMaskStatusList = null,
                                            });
                                            return maskStatuss1;
                                        })()
                                    });
                                    maskStatusDatas.Add(new MaskStatusDatas()
                                    {
                                        UNITID = " 1AED06-P02",
                                        MaskStatusList = new Func<MaskStatuss>(() =>
                                        {
                                            MaskStatuss maskStatuss1 = new MaskStatuss();
                                            maskStatuss1.Add(new MaskStatuss()
                                            {
                                                MASKID = "TestMask001",
                                                MASKST = "10",
                                                MASKUSECNT = "2",
                                                SUNITMaskStatusList = null,
                                            });
                                            maskStatuss1.Add(new MaskStatuss()
                                            {
                                                MASKID = "TestMask002",
                                                MASKST = "10",
                                                MASKUSECNT = "2",
                                                SUNITMaskStatusList = null,
                                            });
                                            return maskStatuss1;
                                        })()
                                    });
                                    return maskStatusDatas;
                                })(),
                            });
                            return maskStatuss;
                        })(),
                    });
                    break;
                case SFCD.MaterialStatus:
                    break;
                case SFCD.SorterJobList:
                    break;
                case SFCD.CratePortStatus:
                    break;
                case SFCD.PortLoad:
                    break;
                case SFCD.EquipmentRecycleStatus:
                    break;
                default:
                    break;
            }
            cimClient?.LaunchFormattedStatusReportProcess(report);
        }

        public void EnableDisableEventReportRequestEvent(string[] ceidArr)
        {
            if (ceidArr.Length > 0)
                AddLog("CEED: " + ceidArr[0]);
            for (int index = 1; index < ceidArr.Length; index++)
            {
                AddLog("CEID: " + ceidArr[index]);
            }
        }

        public void TraceDataInitializationRequestEvent(TraceDataInitializationRequest traceDataInitializationRequest, bool needReply = true)
        {
            if (!needReply)
                return;
            TraceDataInitializationReport report = new TraceDataInitializationReport()
            {
                TRID = traceDataInitializationRequest.TRID,
                SMPLN = "00001",
                STIME = DateTime.Now.ToString("yyyyMMddHHmmss"),
                SVIDLIST = new Func<SVIDS>(() =>
                {
                    SVIDS svidList = new SVIDS();
                    svidList.Add(new SVIDS()
                    {
                        SVID = "10003",
                        SV = "0.015",
                    });
                    svidList.Add(new SVIDS()
                    {
                        SVID = "10004",
                        SV = "0.015",
                    });
                    return svidList;
                })(),
            };
            cimClient?.LaunchTraceDataInitializationReportProcess(report);
        }
        #endregion

        #region Alarm Management
        public void AlarmEnableDisableRequestEvent(AlarmEnableDisableRequest alarmEnableDisableJob)
        {
            AddLog(alarmEnableDisableJob.ToString());
        }

        public void CurrentAlarmListRequestEvent(CurrentAlarmListRequest currentAlarmListJob, bool needReply = true)
        {
            AddLog(currentAlarmListJob.ToString());
            if (!needReply) return;
            CurrentAlarmListReport report = new CurrentAlarmListReport();
            report.Add(new CurrentAlarmListReport()
            {
                UNITID = "1AED06-IND",
                ALIDLIST = new List<string>()
                {
                    "6239",
                    "6240",
                    "6241",
                    "6242",
                },
            });
            report.Add(new CurrentAlarmListReport()
            {
                UNITID = "1AED06-IND1",
                ALIDLIST = new List<string>()
                {
                    "6239",
                    "6240",
                    "6241",
                    "6242",
                },
            });
            cimClient?.LaunchCurrentAlarmListReport(report);
        }
        #endregion

    }
}
