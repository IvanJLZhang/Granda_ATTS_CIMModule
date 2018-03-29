using Secs4Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Granda.ATTS.CIMModule.Extension.ExtensionHelper;

namespace Granda.ATTS.CIMModule
{
    /// <summary>
    /// CIM模块运行主入口
    /// </summary>
    public class CimModuleProcess
    {
        public static SecsGem secsGemService = null;
        public CimModuleProcess(SecsGem secsGem)
        {
            secsGemService = secsGem;
            secsGemService.ConnectionChanged += SecsGemService_ConnectionChanged;
            secsGemService.PrimaryMessageRecived += SecsGemService_PrimaryMessageRecived;
        }
        public CimModuleProcess(string ipAddress, int port, bool isActive, SecsTracer secsTracer)
        {
            secsGemService = new SecsGem(IPAddress.Parse(ipAddress), port, isActive, (primaryMsg, reply) =>
            {
                SecsGemService_PrimaryMessageRecived(null, primaryMsg);
            }, null, 1024);
        }
        private void SecsGemService_PrimaryMessageRecived(object sender, SecsMessage message)
        {
            switch (message.GetSFString())
            {
                default:
                    break;
            }
        }

        private void SecsGemService_ConnectionChanged(object sender, ConnectionState e)
        {
            Debug.WriteLine("connection state change: " + e.ToString());
        }

        public static SecsMessage SendMessage(byte s, byte f, bool replyExpected, Item item = null, int ceid = 0)
        {
            if (secsGemService.State == ConnectionState.Selected)
            {
                return secsGemService.Send(new SecsMessage(s, f, GetFunctionName(s, f, ceid), replyExpected, item));
            }
            return null;
        }
    }
}
