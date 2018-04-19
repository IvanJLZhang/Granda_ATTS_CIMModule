using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Granda.ATTS.CIM;
using Granda.ATTS.CIM.Data.ENUM;
using Granda.ATTS.CIM.Data.Report;
using Granda.ATTS.CIM.Scenario;
using static Granda.ATTS.CIM.Extension.SmlExtension;
using Secs4Net;
using Secs4Net.Sml;
using static Secs4Net.Item;
namespace SecsClient
{
    public partial class Client : Form,
        IInitializeScenario
    {
        public Client()
        {
            InitializeComponent();
            this.Load += Client_Load;
        }


        CIM4EQT cimClient;
        SecsGem secsGem;
        EquipmentInfo _equipmentInfo = new EquipmentInfo()
        {
            CRST = CRST.O,
            EQST = CommonStatus.I,
            EQSTCODE = 1000,
            MDLN = "equipment",
            SOFTREV = "v1.0.",
        };
        private void Client_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.Name = "Main";

            secsGem = new SecsGem(true, IPAddress.Parse("192.168.0.145"), 1024);
            secsGem.ConnectionChanged += SecsGem_ConnectionChanged;
            secsGem.PrimaryMessageReceived += SecsGem_PrimaryMessageReceived;
            secsGem.Start();
            cimClient = new CIM4EQT(secsGem);
            cimClient.ScenarioInitialize(this);


            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void SecsGem_PrimaryMessageReceived(object sender, PrimaryMessageWrapper e)
        {
            SmlLog(e.Message.ToSml());
        }

        private void SecsGem_ConnectionChanged(object sender, ConnectionState e)
        {
            LogMsg("Connection Change: " + e.ToString());
        }

        private void btnSecSend_Click(object sender, EventArgs e)
        {

        }

        private void btnSendPrimaryMsg_Click(object sender, EventArgs e)
        {
            try
            {
                //var result = secsGem.SendAsync(this.primaryMsgToSend.Text.ToSecsMessage());
                //result.Wait();
                //var reply = result.Result;
                //SmlLog(reply.ToSml());
                cimClient.LaunchOnOffLineProcess(true, this._equipmentInfo);

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void LogMsg(string msg)
        {
            msg = $"{DateTime.Now.ToShortTimeString()}-{msg}\r\n";
            this.Invoke((MethodInvoker)delegate
            {
                this.logMsg.Text += msg;
            });
        }

        private void SmlLog(string msg)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.recvdMessage.Text += msg;
            });
        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString());
        }

        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString());
        }

        public void UpdateControlState(CRST controlState)
        {
            LogMsg("Control State Change: " + controlState.ToString());
        }

        public void UpdateDateTime(string dateTimeStr)
        {
            LogMsg("Update date Time: " + dateTimeStr);
        }
    }
}
