using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIDP.PMBus;

namespace MaterialSkinDemo.PMBus.Commands
{
    public abstract class PMCommand : IPMCommand
    {
        public String NameCommand { get; set; }
        public object ValueCommand { get; set; }
        public PMBusDevice device { get; set; }
        public PMCommand(PMBusDevice dev, String _NameCommand)
        {
            NameCommand = _NameCommand;
            device = dev;
            ValueCommand = null;
        }
        public virtual void Execute(object val)
        {
            ValueCommand = val;
        }


        public abstract object ReadCmd();


        public override string ToString()
        {
            return NameCommand;
        }
    }
}
