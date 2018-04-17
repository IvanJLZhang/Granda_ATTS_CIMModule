using System;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using Granda.HSMS;
namespace SecsClient
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
            this.Load += Client_Load;
        }
        SecsHsms secsHsms;

        private void Client_Load(object sender, EventArgs e)
        {
            secsHsms = new SecsHsms(true, IPAddress.Parse("192.168.0.145"), 1024);
            secsHsms.ConnectionChanged += SecsHsms_ConnectionChanged;
            secsHsms.PrimaryMessageReceived += SecsHsms_PrimaryMessageReceived;
            secsHsms.Start();
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void SecsHsms_ConnectionChanged(object sender, TEventArgs<ConnectionState> e)
        {
            LogMsg("Connection Change: " + e.Data.ToString());
        }

        private void SecsHsms_PrimaryMessageReceived(object sender, PrimaryMessageWrapper e)
        {
            //throw new NotImplementedException();
        }



        private void btnSecSend_Click(object sender, EventArgs e)
        {
        }

        private void btnSendPrimaryMsg_Click(object sender, EventArgs e)
        {
            try
            {
                var message = this.primaryMsgToSend.Text.ToSecsMessage();
                //var str = message.ToSML();
                var task = secsHsms.SendAsync(this.primaryMsgToSend.Text.ToSecsMessage());
                //task.Start();
                //task.Wait();
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
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString());
        }

        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString());
        }
    }
}
