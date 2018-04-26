using System;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Granda.ATTS.CIM;
using Granda.ATTS.CIM.Data.ENUM;
using Granda.ATTS.CIM.Data.Report;
using Granda.ATTS.CIM.Scenario;
using Granda.HSMS;
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
        SecsHsms secsGem;
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

            secsGem = new SecsHsms(true, IPAddress.Parse("192.168.0.145"), 7000);
            secsGem.ConnectionChanged += SecsGem_ConnectionChanged;
            secsGem.PrimaryMessageReceived += SecsGem_PrimaryMessageReceived;
            secsGem.Start();
            //cimClient = new CIM4EQT(secsGem);
            //cimClient.ScenarioInitialize(this);


            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void SecsGem_PrimaryMessageReceived(object sender, PrimaryMessageWrapper e)
        {
            SmlLog(e.Message.ToSml());
            string reply = @"Establish Communication Request:S1F14
  <L [2] 
    <A [1] '0'>
    <L [2] 
      <A [4] 'MDLN'>
      <A [7] 'SOFTREV'>
    >
  >
.";
            secsGem.SendAsync(reply.ToSecsMessage(), e.MessageId);
        }

        private void SecsGem_ConnectionChanged(object sender, TEventArgs<ConnectionState> e)
        {
            LogMsg("Connection Change: " + e.Data.ToString());
        }

        private void btnSecSend_Click(object sender, EventArgs e)
        {

        }

        private void btnSendPrimaryMsg_Click(object sender, EventArgs e)
        {
            try
            {
                //cimClient.LaunchOnOffLineProcess(true, this._equipmentInfo);
                secsGem.SendAsync(@"Establish Communication Request:S1F13 W
  <L [2] 
    <A [4] 'MDLN'>
    <A [7] 'SOFTREV'>
  >
.".ToSecsMessage());

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
