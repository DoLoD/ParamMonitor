using System;
using TIDP.PMBus;

namespace ParamerusStudio.PMBus.Commands
{
    public abstract class PMBusReadCommand : IPMBusReadCommand
    {
        protected PMBusDevice dev;
        public PMBusCode command_code;
        public abstract object ReadCmd();
        protected Action<object> propertyChangeDelegate;
        protected Func<bool> checkNullPropertyDelegate;
        protected byte? GetCurrentMode()
        {
            var res_query = dev.SMBus_Read_Byte((byte)PMBusCode.VOUT_MODE);
            if (!res_query.Success)
                return null;
            return res_query.Byte;

        }

        protected byte[] GetParamBytes(PMBusCode cmd)
        {
            var result_query = dev.SMBus_Read_Word((byte)cmd);
            if (!result_query.Success)
                return null;
            return result_query.Bytes;
        }
        public PMBusReadCommand(PMBusDevice _dev, Action<object> _propertyChangeDelegate, Func<bool> _checkNullPropertyDelegate)
        {
            dev = _dev;
            propertyChangeDelegate = _propertyChangeDelegate;
            checkNullPropertyDelegate = _checkNullPropertyDelegate;
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
