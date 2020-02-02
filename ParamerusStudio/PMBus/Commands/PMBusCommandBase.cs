using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ParamerusStudio.PMBus.Commands
{
    public abstract class PMBusCommandBase : IPMBusReadCommand, IPMBusWriteCommand, INotifyPropertyChanged
    {
        protected ParamerusPMBusDevice dev;
        protected Action<object> propertyChangeDelegate;
        protected Func<bool> checkNullPropertyDelegate;
        private object _last_value = 0;

        public object LastValue
        {
            get => _last_value;
            set
            {
                if (_last_value != value)
                {
                    _last_value = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool isReadable { get; protected set; } = true;
        public bool isWritable { get; protected set; } = true;
        public PMBusCode command_code;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public abstract object ReadCmd();
        public abstract bool WriteCmd(object val);
        protected abstract byte? GetCurrentMode();

        protected byte[] GetParamBytes(PMBusCode cmd)
        {
            var result_query = dev.Device.SMBus_Read_Word((byte)cmd);
            if (!result_query.Success)
                return null;
            return result_query.Bytes;
        }
        public PMBusCommandBase(ParamerusPMBusDevice _dev, Action<object> _propertyChangeDelegate)
        {
            dev = _dev;
            propertyChangeDelegate = _propertyChangeDelegate;
        }

        protected virtual object CalculateParam(sbyte exponent, ushort mantissa)
        {
            return Math.Pow(2, exponent) * mantissa;
        }

        protected virtual object CalculateParam(ushort val, byte[] parametres)
        {
            if (parametres == null || parametres.Length != 5)
                return null;
            ushort m = (ushort)((parametres[0] << 8) | parametres[1]);
            ushort b = (ushort)((parametres[2] << 8) | parametres[3]);
            byte r = parametres[4];
            double? param = (1.0 / m) * (val * Math.Pow(10, (-r)) - b);
            return param;
        }
    }
}
