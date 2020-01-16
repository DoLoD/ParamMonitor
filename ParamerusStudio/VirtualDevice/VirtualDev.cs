using System;
using System.Collections.Generic;
using System.Text;
using TIDP.SAA;

namespace ParamerusStudio.VirtualDevice
{
    public class VirtualDeviceDriverFactory : IAdapterDriverFactory
    {
        public IList<IAdapterDriver> Discover()
        {
            var drivers = new List<IAdapterDriver>();

            var driver = new VirtualDev();
            drivers.Add(driver);
            return drivers;
        }

    }


    class VirtualDev : IAdapterDriver, ISMBusAdapterDriver
    {
        // This is used to simulate an adapter controlling a TPS53819
        private const byte DEVICE_ADDRESS = 100;
        private Dictionary<byte, byte> Byte_Data = new Dictionary<byte, byte>();
        private Dictionary<byte, PMBusWord> Word_Data = new Dictionary<byte, PMBusWord>();
        private LogicLevel Control = LogicLevel.Low;
        private LogicLevel SMB_Alert = LogicLevel.High;

        public VirtualDev()
        {
            // TPS53819 (mostly) device power on defaults; we simulate talking to this device
            // STATUS_WORD/BYTE has a bit set to make it interesting
            Byte_Data[0x01] = 0x00;                     // OPERATION
            Byte_Data[0x02] = 0x17;                     // ON_OFF_CONFIG
            Byte_Data[0x10] = 0x00;                     // WRITE_PROTECT
            Byte_Data[0x78] = 0x01;                     // STATUS_BYTE
            Word_Data[0x79] = new PMBusWord("0x0001");  // STATUS_WORD
            Byte_Data[0x7A] = 0x7;                      // STATUS_VOUT
            Byte_Data[0xD0] = 0x00;                     // MFR00
            Byte_Data[0xD1] = 0x12;                     // MFR01
            Byte_Data[0xD2] = 0x00;                     // MFR02
            Byte_Data[0xD3] = 0xB2;                     // MFR03
            Byte_Data[0xD4] = 0x10;                     // MFR04
            Byte_Data[0xD5] = 0x65;                     // MFR05
            Byte_Data[0xD6] = 0x05;                     // MFR06
            Word_Data[0xFC] = new PMBusWord("0x0090");  // MFR44 [DEVICE_CODE]
            Word_Data[0xFD] = new PMBusWord("0x00FF");  // MFR45 [DEVICE_ID]
        }

        #region IAdapterDriver
        public SAAStatus Get_Version(out Version version)
        {
            // With USB-TO-GPIO, the adapter firmware version is queried via
            // an adapter command. In this example we just hard code something.
            //
            // Note that the return value is cached within SMBusAdater, so you 
            // don't need to cache internally here.
            version = new Version(1, 0);
            return SAAStatus.Success;
        }

        public bool Is_Feature_Supported(SAAFeature feature)
        {
            switch (feature)
            {
                case SAAFeature.PullupResistors:
                case SAAFeature.GPIO:
                case SAAFeature.GroupCommand:
                case SAAFeature.ParallelMode:
                case SAAFeature.ProcessCall:
                    return false;

                case SAAFeature.SMBAlert:
                case SAAFeature.BlockWriteBlockReadProcessCall:
                    return true;

                default:
                    throw new Exception("unexpected SAAFeature");
            }
        }

        public string Long_Name
        {
            get { return "Sample Driver"; }
        }

        public string Short_Name
        {
            get { return "Sample"; }
        }
        #endregion

        #region ISMBusAdapterDriver
        public void Dispose()
        {
            // You would do any required de-allocation of unmanaged resources, closing down anything, etc. here
            // This is called for you automatically.
        }

        public SAAStatus Block_Write_Block_Read_Process_Call(byte addr, byte cmd_code, int byteCount,
            byte[] Write_Block, out byte[] Read_Block)
        {
            throw new NotImplementedException();
        }

        public SAAStatus Get_Control(int control_line_number, out LogicLevel logic_level)
        {
            if (control_line_number != 1)
            {
                // SMBusAdapter should never call Get_Control with invalid control line, based
                // on Num_Control_Lines property
                logic_level = LogicLevel.Low; // No data to return
                return SAAStatus.UnsupportedFeature;
            }
            else
            {
                logic_level = Control;
                return SAAStatus.Success;
            }
        }

        public SAAStatus Get_SMBusAlert(out LogicLevel logic_level)
        {
            logic_level = SMB_Alert;
            return SAAStatus.Success;
        }

        public int Num_Control_Lines
        {
            get { return 1; }
        }

        public SAAStatus Process_Call(byte addr, byte cmd_code, byte writeHi, byte writeLo, out byte readHi, out byte readLo)
        {
            readHi = readLo = 0x00;
            return SAAStatus.UnsupportedFeature;
        }

        public SAAStatus Read_Block(byte addr, byte cmd_code, out byte[] data)
        {
            // No data to return
            data = new byte[] { };
            if (addr != DEVICE_ADDRESS)
            {
                // No data to return
                return SAAStatus.InvalidResponse; // NACK equivalent
            }

            if (cmd_code == 0xFD)
            {
                data = Encoding.Default.GetBytes("UCD3100ISO1|0.0.11.0105|141009");
                return SAAStatus.Success; // ACK equivalent
            }
            else if (cmd_code == 0xE7)
            {
                data = Encoding.Default.GetBytes("VERSION1|BUCKSYNC000");
                return SAAStatus.Success; // ACK equivalent
            }
            else if (cmd_code == 0x9B)
            {
                data = Encoding.Default.GetBytes("E1");
                return SAAStatus.Success; // ACK equivalent
            }
            else if (cmd_code == 0xE4)
            {
                data = new byte[33];
                data[0] = 32;
                return SAAStatus.Success; // ACK equivalent
            }
            else if (cmd_code == 0xE5)
            {
                data = new byte[33];
                data[0] = 32;
                data[1] = 0xE;
                return SAAStatus.Success; // ACK equivalent
            }
            else if (cmd_code == 0xE2)
            {
                data = new byte[33];
                data[0] = 32;
                data[1] = 0xE;
                return SAAStatus.Success; // ACK equivalent
            }
            return SAAStatus.InvalidResponse;

        }

        public SAAStatus Read_Byte(byte addr, byte cmd_code, out byte data)
        {
            data = 0x00;
            if (addr != DEVICE_ADDRESS || !Byte_Data.ContainsKey(cmd_code))
                return SAAStatus.InvalidResponse;
            else
            {
                data = Byte_Data[cmd_code];
                return SAAStatus.Success; // ACK equivalent
            }
        }

        public SAAStatus Read_Word(byte addr, byte cmd_code, out byte hi_byte, out byte lo_byte)
        {
            if (addr != DEVICE_ADDRESS || !Word_Data.ContainsKey(cmd_code))
            {
                // No data to return
                hi_byte = lo_byte = 0x00;
                return SAAStatus.InvalidResponse; // NACK equivalent
            }

            else
            {
                var word = Word_Data[cmd_code];
                hi_byte = word.Hi_Byte;
                lo_byte = word.Lo_Byte;
                return SAAStatus.Success; // ACK equivalent
            }
        }

        public SAAStatus Receive_Byte(byte addr, out byte data)
        {
            // No data to return
            data = 0x00;
            return SAAStatus.UnsupportedFeature;
        }

        public SAAStatus Send_Byte(byte addr, byte cmd_code)
        {
            // Allow CLEAR_FAULTS command to device
            if (addr == DEVICE_ADDRESS && cmd_code == 0x03)
                return SAAStatus.Success; // ACK equivalent
            else
                return SAAStatus.InvalidResponse; // NACK equivalent
        }

        public SAAStatus Send_Group_Command(byte[] group)
        {
            return SAAStatus.UnsupportedFeature;
        }

        public SAAStatus Set_Bus_Speed(SMBusAdapter.BusSpeed speed)
        {
            // Similate that we would do something with this setting
            return SAAStatus.Success;
        }

        public SAAStatus Set_Control(int control_line_number, LogicLevel logic_level)
        {
            if (control_line_number != 1)
            {
                // SMBusAdapter should never call Get_Control with invalid control line, based
                // on Num_Control_Lines property
                return SAAStatus.UnsupportedFeature;
            }
            else
            {
                Control = logic_level;
                return SAAStatus.Success;
            }
        }

        public SAAStatus Set_PEC_Enabled(bool enabled)
        {
            // Similate that we would do something with this setting
            return SAAStatus.Success;
        }

        public SAAStatus Write_Block(byte addr, byte cmd_code, int len, byte[] data)
        {
            // TPS53819 does not have any write block commands
            return SAAStatus.InvalidResponse; // NACK equivalent
        }

        public SAAStatus Write_Byte(byte addr, byte cmd_code, byte data_byte)
        {
            // Simulate allow writing of all TPS53819 write byte commands (STATUS_BYTE=0x78 is r/o)
            if (addr != DEVICE_ADDRESS || !Byte_Data.ContainsKey(cmd_code) || cmd_code == 0x78)
            {
                return SAAStatus.InvalidResponse; // NACK equivalent
            }
            else
            {
                Byte_Data[cmd_code] = data_byte;
                return SAAStatus.Success; // ACK equivalent
            }
        }

        public SAAStatus Write_Word(byte addr, byte cmd_code, byte hi_byte, byte lo_byte)
        {
            // TPS53819 does not have any write word commands
            return SAAStatus.InvalidResponse; // NACK equivalent
        }
        #endregion
    }
}
