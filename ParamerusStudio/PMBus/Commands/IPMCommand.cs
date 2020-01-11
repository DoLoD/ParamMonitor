using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialSkinDemo.PMBus.Commands
{
    public interface IPMCommand
    {
        void Execute(object val);
        object ReadCmd();

    }
}
