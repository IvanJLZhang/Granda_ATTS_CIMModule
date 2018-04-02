using Granda.ATTS.CIMModule;
using Granda.ATTS.CIMModule.Model;
using Granda.ATTS.CIMModule.Scenario;
using Secs4Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
namespace HelloWorld
{
    public partial class Server : Form
    {
        public Server()
        {
            InitializeComponent();
            this.Load += Server_Load;

        }
        CimModuleProcess cimModule;
        SecsGem secsGem;
        private void Server_Load(object sender, EventArgs e)
        {
            secsGem = new SecsGem(IPAddress.Parse("192.168.0.49"), 1024, false, 1024);
            secsGem.PrimaryMessageRecived += SecsGem_PrimaryMessageRecived;
            secsGem.ConnectionChanged += SecsGem_ConnectionChanged;
            cimModule = new CimModuleProcess(secsGem);
            cimModule.ControlStateChanged += CimModule_ControlStateChanged;
            cimModule.DateTimeUpdate += CimModule_DateTimeUpdate;
        }

        private void CimModule_DateTimeUpdate(object sender, TEventArgs<string> e)
        {
            LogMsg("Date and Time Update: " + e.Data.ToString());
        }

        private void CimModule_ControlStateChanged(object sender, TEventArgs<ControlState> e)
        {
            LogMsg("Control State Change: " + e.Data.ToString());
        }

        private void SecsGem_ConnectionChanged(object sender, TEventArgs<Secs4Net.ConnectionState> e)
        {
            LogMsg("Connection State Change: " + e.Data.ToString());
        }

        private void SecsGem_PrimaryMessageRecived(object sender, TEventArgs<SecsMessage> e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.recvdMessage.Text = e.Data.ToSML();
            });
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
    }
}
