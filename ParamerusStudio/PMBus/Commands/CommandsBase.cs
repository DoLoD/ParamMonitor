using System;
using TIDP.PMBus;
using TIDP.SAA;

namespace ParamerusStudio.PMBus.Commands
{
    public class ReadVoutCommand : PMBusCommandBase
    {
        public ReadVoutCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.READ_VOUT;
            this.isWritable = false;
        }

        public override bool WriteCmd(object val)
        {
            return false;
        }

        protected override byte? GetCurrentMode()
        {
            var res_query = dev.Device.SMBus_Read_Byte((byte)PMBusCode.VOUT_MODE);
            if (!res_query.Success)
                return null;
            return res_query.Byte;
        }

        public override object ReadCmd()
        {
            if (isReadable == false)
            {
                LastValue = null;
                return null;
            }
                
            double? res = null;
            if (LastValue != null)
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
            else
            {
                isReadable = false;
            }
            LastValue = res;
            propertyChangeDelegate(res);
            return res;
        }
    }

    public class ReadIoutCommand : PMBusCommandBase
    {
        public ReadIoutCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.READ_IOUT;
            this.isWritable = false;
        }
        protected override byte? GetCurrentMode()
        {
            return null;
        }
        public override bool WriteCmd(object val)
        {
            return false;
        }

        public override object ReadCmd()
        {
            if (isReadable == false)
            {
                LastValue = null;
                return null;
            }
            double? res = null;
            if (LastValue != null)
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
                else
                {
                    isReadable = false;
                }
            }
            LastValue = res;
            propertyChangeDelegate(res);
            return res;
        }
    }

    public class ReadVinCommand : ReadIoutCommand
    {
        public ReadVinCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.READ_VIN;
            this.isWritable = false;
        }
    }

    public class ReadIinCommand : ReadIoutCommand
    {
        public ReadIinCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.READ_IIN;
            this.isWritable = false;
        }
    }

    public class ReadTemp1Command : ReadIoutCommand
    {
        public ReadTemp1Command(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.READ_TEMP_1;
            this.isWritable = false;
        }
    }

    public class ReadTemp2Command : ReadIoutCommand
    {
        public ReadTemp2Command(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.READ_TEMP_2;
            this.isWritable = false;
        }
    }

    public class ReadFreqCommand : ReadIoutCommand
    {
        public ReadFreqCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.READ_FREQ;
            this.isWritable = false;
        }
    }



    public class StatusVOUTCommand : PMBusCommandBase
    {
        public StatusVOUTCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev,act)
        {
            command_code = PMBusCode.STATUS_VOUT;
        }
        public override object ReadCmd()
        {
            if (isReadable == false)
            {
                LastValue = null;
                return null;
            }
            byte? res = null;
            if (LastValue != null)
            {
                var reg_val = dev.Device.SMBus_Read_Byte((byte)command_code);
                if (reg_val.SAA_Status == SAAStatus.Success)
                    res = reg_val.Byte;
            }
            LastValue = res;
            propertyChangeDelegate?.Invoke(res);
            return res;
        }

        public override bool WriteCmd(object val)
        {
            if (!isWritable && val != null && !(val is byte))
                return false;
            SAAStatus res = dev.Device.SMBus_Write_Byte((byte)command_code, (byte)val);
            if (res == SAAStatus.Success)
                return true;
            else
                return false;
        }

        protected override byte? GetCurrentMode()
        {
            return null;
        }
    }

    public class StatusIOUTCommand : StatusVOUTCommand
    {
        public StatusIOUTCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev,act)
        {
            command_code = PMBusCode.STATUS_IOUT;
        }
    }

    public class StatusTempCommand : StatusVOUTCommand
    {
        public StatusTempCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.STATUS_TEMP;
        }
    }

    public class StatusCMLCommand : StatusVOUTCommand
    {
        public StatusCMLCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.STATUS_CML;
        }
    }

    public class StatusINPUTCommand : StatusVOUTCommand
    {
        public StatusINPUTCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.STATUS_INPUT;
        }
    }

    public class StatusFANS12Command : StatusVOUTCommand
    {
        public StatusFANS12Command(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.STATUS_FANS_1_2;
        }
    }

    public class StatusFANS34Command : StatusVOUTCommand
    {
        public StatusFANS34Command(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.STATUS_FANS_3_4;
        }
    }

    public class StatusMFRSpecificCommand : StatusVOUTCommand
    {
        public StatusMFRSpecificCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.STATUS_MFR_SPECIFIC;
        }
    }

    public class StatusOTHERCommand : StatusVOUTCommand
    {
        public StatusOTHERCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.STATUS_OTHER;
        }
    }



    public class VinOVFaultLimitCommand : PMBusCommandBase
    {
        public VinOVFaultLimitCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.VIN_OV_FAULT_LIMIT;
        }
        public override object ReadCmd()
        {
            if (isReadable == false)
            {
                LastValue = null;
                return null;
            }
            short? res = null;
            if (LastValue != null)
            {
                var reg_val = dev.Device.SMBus_Read_Byte((byte)command_code);
                if (reg_val.SAA_Status == SAAStatus.Success)
                    res = BitConverter.ToInt16(reg_val.Bytes,0);
            }
            LastValue = res;
            propertyChangeDelegate?.Invoke(res);
            return res;
        }

        public override bool WriteCmd(object val)
        {
            if (!isWritable && val != null && !(val is ushort))
                return false;
            byte[] bytes = BitConverter.GetBytes((ushort)val);
            SAAStatus res = dev.Device.SMBus_Write_Word((byte)command_code, bytes[1],bytes[0]);
            if (res == SAAStatus.Success)
                return true;
            else
                return false;
        }

        protected override byte? GetCurrentMode()
        {
            return null;
        }
    }

    public class VinOVFaultResponeCommand : StatusVOUTCommand
    {
        public VinOVFaultResponeCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.VIN_OV_FAULT_RESPONSE;
        }
    }

    public class VinOVWarnLimitCommand : VinOVFaultLimitCommand
    {
        public VinOVWarnLimitCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.VIN_OV_WARN_LIMIT;
        }
    }

    public class VinUVWarnLimitCommand : VinOVFaultLimitCommand
    {
        public VinUVWarnLimitCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.VIN_UV_WARN_LIMIT;
        }
    }

    public class VinUVFaultLimitCommand : VinOVFaultLimitCommand
    {
        public VinUVFaultLimitCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.VIN_UV_FAULT_LIMIT;
        }
    }

    public class VinUVFaultResponceCommand : StatusVOUTCommand
    {
        public VinUVFaultResponceCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.VIN_UV_FAULT_RESPONSE;
        }
    }

    public class VoutOVFaultLimitCommand : VinOVFaultLimitCommand
    {
        public VoutOVFaultLimitCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.VOUT_OV_FAULT_LIMIT;
        }
    }

    public class VoutOVFaultResponceCommand : StatusVOUTCommand
    {
        public VoutOVFaultResponceCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.VOUT_OV_FAULT_RESPONSE;
        }
    }

    public class VoutOVWarnLimitCommand : VinOVFaultLimitCommand
    {
        public VoutOVWarnLimitCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.VOUT_OV_WARN_LIMIT;
        }
    }

    public class VoutUVWarnLimitCommand : VinOVFaultLimitCommand
    {
        public VoutUVWarnLimitCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.VOUT_UV_WARN_LIMIT;
        }
    }

    public class VoutUVFaultLimitCommand : VinOVFaultLimitCommand
    {
        public VoutUVFaultLimitCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.VOUT_UV_FAULT_LIMIT;
        }
    }

    public class VoutUVFaultResponceCommand : StatusVOUTCommand
    {
        public VoutUVFaultResponceCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.VOUT_UV_FAULT_RESPONS;
        }
    }

    public class IoutOCFaultLimitCommand : VinOVFaultLimitCommand
    {
        public IoutOCFaultLimitCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.IOUT_OC_FAULT_LIMIT;
        }
    }

    public class IoutOCFaultResponseCommand : StatusVOUTCommand
    {
        public IoutOCFaultResponseCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.IOUT_OC_FAULT_RESPONSE;
        }
    }

    public class IoutOCWarnLimitCommand : VinOVFaultLimitCommand
    {
        public IoutOCWarnLimitCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.IOUT_OC_WARN_LIMIT;
        }
    }

    public class PoutOPFaultLimitCommand : VinOVFaultLimitCommand
    {
        public PoutOPFaultLimitCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.POUT_OP_FAULT_LIMIT;
        }
    }

    public class PoutOPWarnLimitCommand : VinOVFaultLimitCommand
    {
        public PoutOPWarnLimitCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.POUT_OP_WARN_LIMIT;
        }
    }

    public class OTFaultLimitCommand : VinOVFaultLimitCommand
    {
        public OTFaultLimitCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.OT_FAULT_LIMIT;
        }
    }

    public class OTFaultResponseCommand : StatusVOUTCommand
    {
        public OTFaultResponseCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.OT_FAULT_RESPONSE;
        }
    }

    public class OTWarnLimitCommand : VinOVFaultLimitCommand
    {
        public OTWarnLimitCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.OT_WARN_LIMIT;
        }
    }

    public class UTWarnLimitCommand : VinOVFaultLimitCommand
    {
        public UTWarnLimitCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.UT_WARN_LIMIT;
        }
    }

    public class UTFaultLimitCommand : VinOVFaultLimitCommand
    {
        public UTFaultLimitCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.UT_FAULT_LIMIT;
        }
    }

    public class UTFaultResponseCommand : StatusVOUTCommand
    {
        public UTFaultResponseCommand(ParamerusPMBusDevice _dev, Action<object> act) : base(_dev, act)
        {
            command_code = PMBusCode.UT_FAULT_RESPONSE;
        }
    }
}