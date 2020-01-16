using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIDP.PMBus;

namespace ParamerusStudio.PMBus.Commands
{
    public interface IPMBusReadCommand
    {
        object ReadCmd();
    }
}
