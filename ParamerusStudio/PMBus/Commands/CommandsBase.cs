using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIDP.PMBus;
using TIDP.PMBus.Standard.Commands;

namespace MaterialSkinDemo.PMBus.Commands
{
    class PMCommandOperation : PMCommand
    {
        public PMCommandOperation(PMBusDevice dev) : base(dev, "OPERATION")
        {}

        public override void Execute(object val)
        {
            base.Execute(val);
            
            
            throw new NotSupportedException();
        }

        public override object ReadCmd()
        {
            if (device.Commands.OPERATION() != null)
                ValueCommand = device.Commands.OPERATION().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }

    }

    class PMCommandOnOffConfig : PMCommand
    {
        public PMCommandOnOffConfig(PMBusDevice dev) : base(dev, "ON_OFF_CONFIG")
        {

        }

        public override void Execute(object val)
        {
            base.Execute(val);
            throw new NotSupportedException();
        }

        public override object ReadCmd()
        {

            if (device.Commands.ON_OFF_CONFIG() != null)
                ValueCommand = device.Commands.ON_OFF_CONFIG().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }

    }

    class PMCommandVOUTCommand : PMCommand
    {
        public PMCommandVOUTCommand(PMBusDevice dev) : base(dev, "VOUT_COMMAND")
        {

        }

        public override void Execute(object val)
        {
            base.Execute(val);
            if(device.Commands.VOUT_COMMAND() != null)
            {
                device.Commands.VOUT_COMMAND().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {
            if (device.Commands.VOUT_COMMAND() != null)
                ValueCommand = device.Commands.VOUT_COMMAND().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }

    }

    class PMCommandVOUTTrim : PMCommand
    {
        public PMCommandVOUTTrim(PMBusDevice dev) : base(dev, "VOUT_TRIM")
        {

        }

        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.VOUT_TRIM() != null)
            {
                device.Commands.VOUT_TRIM().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {
            if (device.Commands.VOUT_TRIM() != null)
                ValueCommand = device.Commands.VOUT_TRIM().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }

    }

    class PMCommandMarginHigh : PMCommand
    {
        public PMCommandMarginHigh(PMBusDevice dev) : base(dev, "VOUT_MARGIN_HIGH")
        {

        }
        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.VOUT_MARGIN_HIGH() != null)
            {
                device.Commands.VOUT_MARGIN_HIGH().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.VOUT_MARGIN_HIGH() != null)
                ValueCommand = device.Commands.VOUT_MARGIN_HIGH().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }

    }

    class PMCommandMarginLow : PMCommand
    {
        public PMCommandMarginLow(PMBusDevice dev) : base(dev, "VOUT_MARGIN_LOW")
        {

        }

        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.VOUT_MARGIN_LOW() != null)
            {
                device.Commands.VOUT_MARGIN_LOW().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.VOUT_MARGIN_LOW() != null)
                ValueCommand = device.Commands.VOUT_MARGIN_LOW().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }

    }

    class PMCommandTransitionRate : PMCommand
    {
        public PMCommandTransitionRate(PMBusDevice dev) : base(dev, "VOUT_TRANSITION_RATE")
        {

        }

        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.VOUT_TRANSITION_RATE() != null)
            {
                device.Commands.VOUT_TRANSITION_RATE().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.VOUT_TRANSITION_RATE() != null)
                ValueCommand = device.Commands.VOUT_TRANSITION_RATE().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }

    }

    class PMCommandPOUTMax : PMCommand
    {
        public PMCommandPOUTMax(PMBusDevice dev) : base(dev, "POUT_MAX")
        {

        }

        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.POUT_MAX() != null)
            {
                device.Commands.POUT_MAX().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.POUT_MAX() != null)
                ValueCommand = device.Commands.POUT_MAX().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }

    }

    class PMCommandFrequencySwitch : PMCommand
    {
        public PMCommandFrequencySwitch(PMBusDevice dev) : base(dev, "FREQUENCY_SWITCH")
        {

        }
        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.FREQUENCY_SWITCH() != null)
            {
                device.Commands.FREQUENCY_SWITCH().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.FREQUENCY_SWITCH() != null)
                ValueCommand = device.Commands.FREQUENCY_SWITCH().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }

    }

    class PMCommandVINOn : PMCommand
    {
        public PMCommandVINOn(PMBusDevice dev) : base(dev, "VIN_ON")
        {

        }
        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.VIN_ON() != null)
            {
                device.Commands.VIN_ON().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.VIN_ON() != null)
                ValueCommand = device.Commands.VIN_ON().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }

    }

    class PMCommandVINOff : PMCommand
    {
        public PMCommandVINOff(PMBusDevice dev) : base(dev, "VIN_OFF")
        {

        }
        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.VIN_OFF() != null)
            {
                device.Commands.VIN_OFF().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.VIN_OFF() != null)
                ValueCommand = device.Commands.VIN_OFF().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }

    }

    class PMCommandClearFaults : PMCommand
    {
        public PMCommandClearFaults(PMBusDevice dev) : base(dev, "CLEAR_FAULTS")
        {

        }

        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.CLEAR_FAULTS() != null)
            {
                device.Commands.CLEAR_FAULTS().Execute();
            }
        }

        public override object ReadCmd()
        {

            throw new NotSupportedException();
        }

    }

    class PMCommandVOUTOvFaultLimits : PMCommand
    {
        public PMCommandVOUTOvFaultLimits(PMBusDevice dev) : base(dev, "VOUT_OV_FAULT_LIMIT")
        {

        }
        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.VOUT_OV_FAULT_LIMIT() != null)
            {
                device.Commands.VOUT_OV_FAULT_LIMIT().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.VOUT_OV_FAULT_LIMIT() != null)
                ValueCommand = device.Commands.VOUT_OV_FAULT_LIMIT().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }

    }

    class PMCommandVOUTOvFaultResponse : PMCommand
    {
        public PMCommandVOUTOvFaultResponse(PMBusDevice dev) : base(dev, "VOUT_OV_FAULT_RESPONSE")
        {

        }

        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.VOUT_OV_FAULT_RESPONSE() != null)
            {
                throw new NotSupportedException();
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.VOUT_OV_FAULT_RESPONSE() != null)
                ValueCommand = device.Commands.VOUT_OV_FAULT_RESPONSE().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }

    }

    class PMCommandVOUTOvWarnLimit : PMCommand
    {
        public PMCommandVOUTOvWarnLimit(PMBusDevice dev) : base(dev, "VOUT_OV_WARN_LIMIT")
        {

        }
        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.VOUT_OV_WARN_LIMIT() != null)
            {
                device.Commands.VOUT_OV_WARN_LIMIT().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.VOUT_OV_WARN_LIMIT() != null)
                ValueCommand = device.Commands.VOUT_OV_WARN_LIMIT().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }


    }

    class PMCommandVOUTUvWarnLimit : PMCommand
    {
        public PMCommandVOUTUvWarnLimit(PMBusDevice dev) : base(dev, "VOUT_UV_WARN_LIMIT")
        {

        }
        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.VOUT_UV_WARN_LIMIT() != null)
            {
                device.Commands.VOUT_UV_WARN_LIMIT().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.VOUT_UV_WARN_LIMIT() != null)
                ValueCommand = device.Commands.VOUT_UV_WARN_LIMIT().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }

    }

    class PMCommandVOUTUvFaultLimit : PMCommand
    {
        public PMCommandVOUTUvFaultLimit(PMBusDevice dev) : base(dev, "VOUT_UV_FAULT_LIMIT")
        {

        }
        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.VOUT_UV_FAULT_LIMIT() != null)
            {
                device.Commands.VOUT_UV_FAULT_LIMIT().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.VOUT_UV_FAULT_LIMIT() != null)
                ValueCommand = device.Commands.VOUT_UV_FAULT_LIMIT().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }

    }

    class PMCommandVOUTUvFaultResponse : PMCommand
    {
        public PMCommandVOUTUvFaultResponse(PMBusDevice dev) : base(dev, "VOUT_UV_FAULT_RESPONSE")
        {

        }
        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.VOUT_UV_FAULT_RESPONSE() != null)
            {
                throw new NotSupportedException();
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.VOUT_UV_FAULT_RESPONSE() != null)
                ValueCommand = device.Commands.VOUT_UV_FAULT_RESPONSE().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }

    }

    class PMCommandIOUTOcFaultLimit : PMCommand
    {
        public PMCommandIOUTOcFaultLimit(PMBusDevice dev) : base(dev, "IOUT_OC_FAULT_LIMIT")
        {

        }

        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.IOUT_OC_FAULT_LIMIT() != null)
            {
                device.Commands.IOUT_OC_FAULT_LIMIT().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.IOUT_OC_FAULT_LIMIT() != null)
                ValueCommand = device.Commands.IOUT_OC_FAULT_LIMIT().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }

    }

    class PMCommandIOUTOcFaultResponse : PMCommand
    {
        public PMCommandIOUTOcFaultResponse(PMBusDevice dev) : base(dev, "IOUT_OC_FAULT_RESPONSE")
        {

        }
        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.IOUT_OC_FAULT_RESPONSE() != null)
            {
                throw new NotSupportedException();
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.IOUT_OC_FAULT_RESPONSE() != null)
                ValueCommand = device.Commands.IOUT_OC_FAULT_RESPONSE().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }

    }

    class PMCommandIOUTOcWarnLimit : PMCommand
    {
        public PMCommandIOUTOcWarnLimit(PMBusDevice dev) : base(dev, "IOUT_OC_WARN_LIMIT")
        {

        }
        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.IOUT_OC_WARN_LIMIT() != null)
            {
                device.Commands.IOUT_OC_WARN_LIMIT().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.IOUT_OC_WARN_LIMIT() != null)
                ValueCommand = device.Commands.IOUT_OC_WARN_LIMIT().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }

    }

    class PMCommandOTFaultLimit : PMCommand
    {
        public PMCommandOTFaultLimit(PMBusDevice dev) : base(dev, "OT_FAULT_LIMIT")
        {

        }
        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.OT_FAULT_LIMIT() != null)
            {
                device.Commands.OT_FAULT_LIMIT().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.OT_FAULT_LIMIT() != null)
                ValueCommand = device.Commands.OT_FAULT_LIMIT().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }


    }

    class PMCommandOTFaultResponse : PMCommand
    {
        public PMCommandOTFaultResponse(PMBusDevice dev) : base(dev, "OT_FAULT_RESPONSE")
        {

        }

        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.OT_FAULT_RESPONSE() != null)
            {
                throw new NotSupportedException();
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.OT_FAULT_RESPONSE() != null)
                ValueCommand = device.Commands.OT_FAULT_RESPONSE().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }


    }

    class PMCommandOTWarnLimit : PMCommand
    {
        public PMCommandOTWarnLimit(PMBusDevice dev) : base(dev, "OT_WARN_LIMIT")
        {

        }
        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.OT_WARN_LIMIT() != null)
            {
                device.Commands.OT_WARN_LIMIT().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.OT_WARN_LIMIT() != null)
                ValueCommand = device.Commands.OT_WARN_LIMIT().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }


    }

    class PMCommandUTWarnLimit : PMCommand
    {
        public PMCommandUTWarnLimit(PMBusDevice dev) : base(dev, "UT_WARN_LIMIT")
        {

        }

        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.UT_WARN_LIMIT() != null)
            {
                device.Commands.UT_WARN_LIMIT().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.UT_WARN_LIMIT() != null)
                ValueCommand = device.Commands.UT_WARN_LIMIT().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }


    }

    class PMCommandUTFaultLimit : PMCommand
    {
        public PMCommandUTFaultLimit(PMBusDevice dev) : base(dev, "UT_FAULT_LIMIT")
        {

        }

        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.UT_FAULT_LIMIT() != null)
            {
                device.Commands.UT_FAULT_LIMIT().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.UT_FAULT_LIMIT() != null)
                ValueCommand = device.Commands.UT_FAULT_LIMIT().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }

    }

    class PMCommandUTFaultResponce : PMCommand
    {
        public PMCommandUTFaultResponce(PMBusDevice dev) : base(dev, "UT_FAULT_RESPONSE")
        {

        }
        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.UT_FAULT_RESPONSE() != null)
            {
                throw new NotSupportedException();
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.UT_FAULT_RESPONSE() != null)
                ValueCommand = device.Commands.UT_FAULT_RESPONSE().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }

    }

    class PMCommandVINOvFaultLimit : PMCommand
    {
        public PMCommandVINOvFaultLimit(PMBusDevice dev) : base(dev, "VIN_OV_FAULT_LIMIT")
        {

        }

        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.VIN_OV_FAULT_LIMIT() != null)
            {
                device.Commands.VIN_OV_FAULT_LIMIT().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.VIN_OV_FAULT_LIMIT() != null)
                ValueCommand = device.Commands.VIN_OV_FAULT_LIMIT().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }


    }

    class PMCommandVINOvWarnLimit : PMCommand
    {
        public PMCommandVINOvWarnLimit(PMBusDevice dev) : base(dev, "VIN_OV_WARN_LIMIT")
        {

        }

        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.VIN_OV_WARN_LIMIT() != null)
            {
                device.Commands.VIN_OV_WARN_LIMIT().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.VIN_OV_WARN_LIMIT() != null)
                ValueCommand = device.Commands.VIN_OV_WARN_LIMIT().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }


    }

    class PMCommandVINUvFaultLimit : PMCommand
    {
        public PMCommandVINUvFaultLimit(PMBusDevice dev) : base(dev, "VIN_UV_FAULT_LIMIT")
        {

        }

        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.VIN_UV_FAULT_LIMIT() != null)
            {
                device.Commands.VIN_UV_FAULT_LIMIT().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.VIN_UV_FAULT_LIMIT() != null)
                ValueCommand = device.Commands.VIN_UV_FAULT_LIMIT().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }


    }

    class PMCommandVINUvWarnLimit : PMCommand
    {
        public PMCommandVINUvWarnLimit(PMBusDevice dev) : base(dev, "VIN_UV_WARN_LIMIT")
        {

        }

        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.VIN_UV_WARN_LIMIT() != null)
            {
                device.Commands.VIN_UV_WARN_LIMIT().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.VIN_UV_WARN_LIMIT() != null)
                ValueCommand = device.Commands.VIN_UV_WARN_LIMIT().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }


    }

    class PMCommandPowerGoodOn : PMCommand
    {
        public PMCommandPowerGoodOn(PMBusDevice dev) : base(dev, "POWER_GOOD_ON")
        {

        }

        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.POWER_GOOD_ON() != null)
            {
                device.Commands.POWER_GOOD_ON().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.POWER_GOOD_ON() != null)
                ValueCommand = device.Commands.POWER_GOOD_ON().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }


    }

    class PMCommandPowerGoodOff : PMCommand
    {
        public PMCommandPowerGoodOff(PMBusDevice dev) : base(dev, "POWER_GOOD_OFF")
        {

        }
        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.POWER_GOOD_OFF() != null)
            {
                device.Commands.POWER_GOOD_OFF().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.POWER_GOOD_OFF() != null)
                ValueCommand = device.Commands.POWER_GOOD_OFF().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }


    }

    class PMCommandTONDelay : PMCommand
    {
        public PMCommandTONDelay(PMBusDevice dev) : base(dev, "TON_DELAY")
        {

        }

        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.TON_DELAY() != null)
            {
                device.Commands.TON_DELAY().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.TON_DELAY() != null)
                ValueCommand = device.Commands.TON_DELAY().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }


    }

    class PMCommandTONRise : PMCommand
    {
        public PMCommandTONRise(PMBusDevice dev) : base(dev, "TON_RISE")
        {

        }
        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.TON_RISE() != null)
            {
                device.Commands.TON_RISE().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.TON_RISE() != null)
                ValueCommand = device.Commands.TON_RISE().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }


    }

    class PMCommandTONMaxFaultsLimit : PMCommand
    {
        public PMCommandTONMaxFaultsLimit(PMBusDevice dev) : base(dev, "TON_MAX_FAULT_LIMIT")
        {

        }

        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.TON_MAX_FAULT_LIMIT() != null)
            {
                device.Commands.TON_MAX_FAULT_LIMIT().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.TON_MAX_FAULT_LIMIT() != null)
                ValueCommand = device.Commands.TON_MAX_FAULT_LIMIT().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }


    }

    class PMCommandTONMaxFaultsResponse : PMCommand
    {
        public PMCommandTONMaxFaultsResponse(PMBusDevice dev) : base(dev, "TON_MAX_FAULT_RESPONSE")
        {

        }
        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.TON_MAX_FAULT_RESPONSE() != null)
            {
                throw new NotSupportedException();
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.TON_MAX_FAULT_RESPONSE() != null)
                ValueCommand = device.Commands.TON_MAX_FAULT_RESPONSE().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }


    }

    class PMCommandTOFFDelay : PMCommand
    {
        public PMCommandTOFFDelay(PMBusDevice dev) : base(dev, "TOFF_DELAY")
        {

        }

        public override void Execute(object val)
        {
            base.Execute(val);
            if (device.Commands.TOFF_DELAY() != null)
            {
                device.Commands.TOFF_DELAY().Write(Convert.ToDouble(val));
            }
        }

        public override object ReadCmd()
        {

            if (device.Commands.TOFF_DELAY() != null)
                ValueCommand = device.Commands.TOFF_DELAY().Latest.ToString();
            else
                throw new NotSupportedException();
            return ValueCommand;
        }

    }
}
