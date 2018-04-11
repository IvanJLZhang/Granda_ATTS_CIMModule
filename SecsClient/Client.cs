using Granda.ATTS.CIM.Data.ENUM;
using Granda.ATTS.CIM;
using Granda.ATTS.CIM.Model;
using Secs4Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Granda.ATTS.CIM.Data.Report;

namespace SecsClient
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
            this.Load += Client_Load;
        }
        CimModuleForEQT cimModule;
        SecsGem secsGem;
        private void Client_Load(object sender, EventArgs e)
        {
            secsGem = new SecsGem(IPAddress.Parse("192.168.0.145"), 1024, true);
            secsGem.PrimaryMessageRecived += SecsGem_PrimaryMessageRecived;
            secsGem.ConnectionChanged += SecsGem_ConnectionChanged;
            cimModule = new CimModuleForEQT(secsGem);
            cimModule.ControlStateChanged += CimModule_ControlStateChanged;
            CimModuleForEQT.ErrorOccured += CimModuleForEQT_ErrorOccured;
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CimModuleForEQT_ErrorOccured(object sender, TEventArgs<Exception> e)
        {
            MessageBox.Show(e.Data.Message.ToString());
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString());
        }

        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString());
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
                Debug.WriteLine(e.Data.ToSML());
            });
        }

        private void btnSecSend_Click(object sender, EventArgs e)
        {
        }

        private void btnSendPrimaryMsg_Click(object sender, EventArgs e)
        {
            //secsGem.Send(this.primaryMsgToSend.Text.ToSecsMessage());
            //cimModule.LaunchOnOffLineProcess(true);
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
