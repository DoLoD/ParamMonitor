using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParamerusStudio.PMBus.Commands
{
    interface IPMBusWriteCommand
    {
        bool WriteCmd(object val);
    }
}
