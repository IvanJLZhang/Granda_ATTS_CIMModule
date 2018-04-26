using System;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Granda.ATTS.CIM;
using Granda.ATTS.CIM.Data.ENUM;
using Granda.ATTS.CIM.Scenario;
using Granda.HSMS;
namespace SecsServer
{
    public partial class Server : Form,
        IInitializeScenario
    {
        public Server()
        {
            InitializeComponent();
            this.Load += Server_Load;
        }
        SecsHsms secsGem;
        CIM4HST cimServer;
        private void Server_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.Name = "Main";

            secsGem = new SecsHsms(false, IPAddress.Parse("192.168.0.145"), 1024);
            secsGem.ConnectionChanged += SecsGem_ConnectionChanged;
            secsGem.PrimaryMessageReceived += SecsGem_PrimaryMessageReceived;
            cimServer = new CIM4HST(secsGem);
            cimServer.ScenarioInitialize(this);

            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void SecsGem_PrimaryMessageReceived(object sender, PrimaryMessageWrapper e)
        {
            SmlLog(e.Message.ToSml());

            string reply = @"Establish Communication Request:'S1F14'
  <L [2] 
    <A [1] '0'>
    <L [2] 
      <A [4] 'MDLN'>
      <A [7] 'SOFTREV'>
    >
  >
.";
            //Thread.Sleep(1000);
            //secsGem.SendAsync(reply.ToSecsMessage(), e.MessageId);
        }

        private void SecsGem_ConnectionChanged(object sender, TEventArgs<ConnectionState> e)
        {
            LogMsg("Connection Change: " + e.Data.ToString());
        }

        private void btnSendPrimaryMsg_Click(object sender, EventArgs e)
        {
            secsGem.SendAsync(this.primaryMsgToSend.Text.ToSecsMessage());
        }

        private void btnSecSend_Click(object sender, EventArgs e)
        {

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

        private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString());
        }

        public void UpdateControlState(CRST controlState)
        {
            throw new NotImplementedException();
        }

        public void UpdateDateTime(string dateTimeStr)
        {
            throw new NotImplementedException();
        }
    }
}
