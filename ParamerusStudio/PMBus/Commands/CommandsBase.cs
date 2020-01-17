using System;
using TIDP.PMBus;

namespace ParamerusStudio.PMBus.Commands
{
    public class ReadVoutCommand : PMBusReadCommand
    {
        public ReadVoutCommand(PMBusDevice _dev, Action<object> act, Func<bool> check) : base(_dev, act, check) 
        {
            command_code = PMBusCode.READ_VOUT;
        }

        public override object ReadCmd()
        {
            double? res = null;
            if (checkNullPropertyDelegate == null || !checkNullPropertyDelegate())
            {
                var res_byte = GetParamBytes(command_code);
                if (res_byte == null)
                    goto EXIT_FUNC;
                byte? mode_byte = GetCurrentMode();
                if (mode_byte == null)
                    goto EXIT_FUNC;
                PMBusMode mode = (PMBusMode)(mode_byte >> 5);
                byte exp = (byte)(mode_byte & 0x1F);
                exp = (exp & 0x10) == 0 ? exp : (byte)(exp | 0xE0);
                ushort mantissa = (ushort)((res_byte[0] << 8) | res_byte[1]);

                if (mode == PMBusMode.Linear)
                    res = (double?)CalculateParam((sbyte)exp, mantissa);

                EXIT_FUNC:;
            }
            propertyChangeDelegate(res);
            return res;
        }
    }

    public class ReadIoutCommand : PMBusReadCommand
    {
        public ReadIoutCommand(PMBusDevice _dev, Action<object> act, Func<bool> check) : base(_dev, act, check)
        {
            command_code = PMBusCode.READ_IOUT;
        }


        public override object ReadCmd()
        {
            double? res = null;
            if (checkNullPropertyDelegate == null || !checkNullPropertyDelegate())
            {
                var res_byte = GetParamBytes(command_code);
                if (res_byte != null)
                {
                    ushort res_val = (ushort)((res_byte[0] << 8) | res_byte[1]);
                    byte exp = (byte)(res_val >> 11);
                    exp = (exp & 0x10) == 0 ? exp : (byte)(exp | 0xF0);
                    ushort mantissa = (ushort)(res_val & 0x7FF);
                    res = (double?)CalculateParam((sbyte)exp, mantissa);
                }                    
            }
            propertyChangeDelegate(res);
            return res;
        }
    }

    public class ReadVinCommand : ReadIoutCommand
    {
        public ReadVinCommand(PMBusDevice _dev, Action<object> act, Func<bool> check) : base(_dev, act, check)
        {
            command_code = PMBusCode.READ_VIN;
        }
    }

    public class ReadIinCommand : ReadIoutCommand
    {
        public ReadIinCommand(PMBusDevice _dev, Action<object> act, Func<bool> check) : base(_dev, act, check)
        {
            command_code = PMBusCode.READ_IIN;
        }
    }

    public class ReadTemp1Command : ReadIoutCommand
    {
        public ReadTemp1Command(PMBusDevice _dev, Action<object> act, Func<bool> check) : base(_dev, act, check)
        {
            command_code = PMBusCode.READ_TEMP_1;
        }
    }

    public class ReadTemp2Command : ReadIoutCommand
    {
        public ReadTemp2Command(PMBusDevice _dev, Action<object> act, Func<bool> check) : base(_dev, act, check)
        {
            command_code = PMBusCode.READ_TEMP_2;
        }
    }

    public class ReadFreqCommand : ReadIoutCommand
    {
        public ReadFreqCommand(PMBusDevice _dev, Action<object> act, Func<bool> check) : base(_dev, act, check)
        {
            command_code = PMBusCode.READ_FREQ;
        }
    }
}
