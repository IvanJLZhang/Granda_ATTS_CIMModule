using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Granda.HSMS;

namespace SecsServer
{
    public partial class Server : Form
    {
        public Server()
        {
            InitializeComponent();
            this.Load += Server_Load;
        }
        SecsHsms secsHsms = null;
        private void Server_Load(object sender, EventArgs e)
        {
            secsHsms = new SecsHsms(false, IPAddress.Parse("192.168.0.145"), 1024);
            secsHsms.ConnectionChanged += SecsHsms_ConnectionChanged;
            secsHsms.PrimaryMessageReceived += SecsHsms_PrimaryMessageReceived;
            secsHsms.Start();

            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void SecsHsms_PrimaryMessageReceived(object sender, PrimaryMessageWrapper e)
        {
//            string sml = @"Establish Communication Request:'S1F14'
//  <L [2] 
//    <A [1] '0'>
//    <L [2] 
//      <A [4] 'MDLN'>
//      <A [7] 'SOFTREV'>
//    >
//  >
//.";
//            e.ReplyAsync(sml.ToSecsMessage());

        }

        private void SecsHsms_ConnectionChanged(object sender, TEventArgs<ConnectionState> e)
        {
            LogMsg("Connection Change: " + e.Data.ToString());
        }

        private void btnSendPrimaryMsg_Click(object sender, EventArgs e)
        {
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
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString());
        }

        private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString());
        }
    }
}
