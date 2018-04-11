using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Granda.ATTS.CIM.Model
{
    enum StreamType : byte
    {
        Equipment_Status        = 0b0000_0001,// S1
        Equipment_Control       = 0b0000_0010,// S2
        Exception_Report        = 0b0000_0101,// S5
        Data_Collection         = 0b0000_0110,// S6
        Process_Program_Management = 0b0000_0111,// S7
        System_Errors           = 0b0000_1001,// S9
        Terminal_Services       = 0b0000_1010,// S10
    }
}
