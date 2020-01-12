using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TIDP;
using TIDP.PMBus;
using TIDP.PMBus.Commands;
using TIDP.SAA;

namespace ParamerusStudio.PMBus
{
    public enum PMBusCode
    {
        VOUT_COMMAND = 0x21,
        STATUS_BYTE = 0x78,
        STATUS_WORD = 0x79,
        STATUS_VOUT = 0x7A,
        STATUS_IOUT = 0x7B,
        STATUS_INPUT = 0x7C,
        STATUS_TEMP = 0x7D,
        STATUS_CML = 0x7E,
        STATUS_OTHER = 0x7F,
        STATUS_MFR_SPECIFIC = 0x80,
        STATUS_FANS_1_2 = 0x81,
        STATUS_FANS_3_4 = 0x82,
        READ_VIN = 0x88,
        READ_IIN = 0x89,
        READ_VOUT = 0x8B,
        READ_IOUT = 0x8C,
        READ_TEMP_1 = 0x8D,
        READ_TEMP_2 = 0x8E,
        READ_TEMP_3 = 0x8F,
        READ_FREQ = 0x95,
        READ_POUT = 0x96,
    }
    /// <summary>
    /// Класс, описывающий PMBus устройство
    /// </summary>
    public class ParamerusPMBusDevice :  INotifyPropertyChanged
    {
        #region Static Methods

        /// <summary>
        /// Поиск SMBus-адаптера
        /// </summary>
        /// <returns>Найденные адаптер или null если не найден</returns>
        public static SMBusAdapter SMBusAdaptherSearch()
        {
            if (SMBusAdapter.Discover() == 0)
                return null;
            return SMBusAdapter.Adapter;
        }

        /// <summary>
        /// Поиск PMBus устройств
        /// </summary>
        /// <returns>Коллекция найденных устройств, null - если устройства не найдены</returns>
        public static List<ParamerusPMBusDevice> PMBusInit()
        {
            var opts = new PMBusDevice.DiscoverOptions();
            opts.Scan_Mode = PMBusDevice.ScanMode.DeviceID;
            if (PMBusDevice.Discover(opts) == 0)
            {
                opts.Scan_Mode = PMBusDevice.ScanMode.TPS53951;
                if (PMBusDevice.Discover(opts) == 0)
                    return null;
            }
            List<ParamerusPMBusDevice> listDevices = new List<ParamerusPMBusDevice>();
            foreach(var dev in PMBusDevice.Devices)
            {
                listDevices.Add(new ParamerusPMBusDevice(dev));
            }

            return listDevices;
        }
        #endregion


        #region Status registers
        private ParamerusRegisterStatus _status_VOUT;
        private ParamerusRegisterStatus _status_IOUT;
        private ParamerusRegisterStatus _status_TEMP;
        private ParamerusRegisterStatus _status_CML;
        private ParamerusRegisterStatus _status_INPUT;
        private ParamerusRegisterStatus _status_Fans_1_2;
        private ParamerusRegisterStatus _status_Fans_3_4;
        private ParamerusRegisterStatus _status_Mfr_Specific;
        private ParamerusRegisterStatus _status_Other;

        

        public ParamerusRegisterStatus Status_VOUT
        {
            get => _status_VOUT;
            set
            {
                _status_VOUT = value;
                OnPropertyChanged();
            }
        }
        public ParamerusRegisterStatus Status_IOUT
        {
            get => _status_IOUT;
            set
            {
                _status_IOUT = value;
                OnPropertyChanged();
            }
        }
        public ParamerusRegisterStatus Status_TEMP
        {
            get => _status_TEMP;
            set
            {
                _status_TEMP = value;
                OnPropertyChanged();
            }
        }
        public ParamerusRegisterStatus Status_CML
        {
            get => _status_CML;
            set
            {
                _status_CML = value;
                OnPropertyChanged();
            }
        }
        public ParamerusRegisterStatus Status_INPUT
        {
            get => _status_INPUT;
            set
            {
                _status_INPUT = value;
                OnPropertyChanged();
            }
        }
        public ParamerusRegisterStatus Status_Fans_1_2
        {
            get => _status_Fans_1_2;
            set
            {
                _status_Fans_1_2 = value;
                OnPropertyChanged();
            }
        }
        public ParamerusRegisterStatus Status_Fans_3_4
        {
            get => _status_Fans_3_4;
            set
            {
                _status_Fans_3_4 = value;
                OnPropertyChanged();
            }
        }
        public ParamerusRegisterStatus Status_Mfr_Specific
        {
            get => _status_Mfr_Specific;
            set
            {
                _status_Mfr_Specific = value;
                OnPropertyChanged();
            }
        }
        public ParamerusRegisterStatus Status_Other
        {
            get => _status_Other;
            set
            {
                _status_Other = value;
                OnPropertyChanged();
            }
        }

        #endregion

        private LogicLevelResult _control_line;
        public LogicLevelResult ControlLine
        {
            get => _control_line;
            set
            {
                    _control_line = value;
                    OnPropertyChanged();
                
            }
        }

        private PMBusDevice _device;
        public PMBusDevice Device
        {
            get => _device;
            set
            {
                _device = value;
                OnPropertyChanged();
            }
        }

        public ParamerusPMBusDevice(PMBusDevice device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device), "Device cannot be null");
            Device = device;
            InitRegs();
            
        }

        /// <summary>
        /// Обновление параметров устройства
        /// </summary>
        public void Update()
        {
            ControlLineStatus();
            var stat_vout = Device.SMBus_Read_Byte((byte)PMBusCode.STATUS_VOUT);
            var stat_iout = Device.SMBus_Read_Byte((byte)PMBusCode.STATUS_IOUT);
            var stat_temp = Device.SMBus_Read_Byte((byte)PMBusCode.STATUS_TEMP);
            var stat_cml = Device.SMBus_Read_Byte((byte)PMBusCode.STATUS_CML);
            var stat_input = Device.SMBus_Read_Byte((byte)PMBusCode.STATUS_INPUT);
            var stat_fans_1_2 = Device.SMBus_Read_Byte((byte)PMBusCode.STATUS_FANS_1_2);
            var stat_fans_3_4 = Device.SMBus_Read_Byte((byte)PMBusCode.STATUS_FANS_3_4);
            var stat_mfr_specific = Device.SMBus_Read_Byte((byte)PMBusCode.STATUS_MFR_SPECIFIC);
            var stat_other = Device.SMBus_Read_Byte((byte)PMBusCode.STATUS_OTHER);
        }

        /// <summary>
        /// Инициализация регистров
        /// </summary>
        private void InitRegs()
        {
            #region Status_VOUT
            List<ParamerusRegisterBit> list_Status_VOUT = new List<ParamerusRegisterBit>();
            list_Status_VOUT.Add(new ParamerusRegisterBit("Vout Tracking Error", list_Status_VOUT.Count, BitStatus.Fault));
            list_Status_VOUT.Add(new ParamerusRegisterBit("TOFF_MAX Warning", list_Status_VOUT.Count, BitStatus.Warning));
            list_Status_VOUT.Add(new ParamerusRegisterBit("TON_MAX Fault", list_Status_VOUT.Count, BitStatus.Fault));
            list_Status_VOUT.Add(new ParamerusRegisterBit("VOUT_MAX Warning", list_Status_VOUT.Count, BitStatus.Warning));
            list_Status_VOUT.Add(new ParamerusRegisterBit("Vout UV Fault", list_Status_VOUT.Count, BitStatus.Fault));
            list_Status_VOUT.Add(new ParamerusRegisterBit("Vout UV Warning", list_Status_VOUT.Count, BitStatus.Warning));
            list_Status_VOUT.Add(new ParamerusRegisterBit("Vout OV Warning", list_Status_VOUT.Count, BitStatus.Warning));
            list_Status_VOUT.Add(new ParamerusRegisterBit("Vout OV Fault", list_Status_VOUT.Count, BitStatus.Fault));
            Status_VOUT = new ParamerusRegisterStatus(list_Status_VOUT);
            #endregion
            #region Status_IOUT
            List<ParamerusRegisterBit> list_Status_IOUT = new List<ParamerusRegisterBit>();
            list_Status_IOUT.Add(new ParamerusRegisterBit("IOUT OV Fault", list_Status_IOUT.Count, BitStatus.Fault));
            list_Status_IOUT.Add(new ParamerusRegisterBit("IOUT OC Fault with LV Shutdown", list_Status_IOUT.Count, BitStatus.Fault));
            list_Status_IOUT.Add(new ParamerusRegisterBit("IOUT OC Fault", list_Status_IOUT.Count, BitStatus.Fault));
            list_Status_IOUT.Add(new ParamerusRegisterBit("IOUT UC Fault", list_Status_IOUT.Count, BitStatus.Fault));
            list_Status_IOUT.Add(new ParamerusRegisterBit("Current Share Fault", list_Status_IOUT.Count, BitStatus.Fault));
            list_Status_IOUT.Add(new ParamerusRegisterBit("Power Limiting Mode", list_Status_IOUT.Count, BitStatus.Fault));
            list_Status_IOUT.Add(new ParamerusRegisterBit("POUT OP Fault", list_Status_IOUT.Count, BitStatus.Fault));
            list_Status_IOUT.Add(new ParamerusRegisterBit("POUT OP Warning", list_Status_IOUT.Count, BitStatus.Warning));
            Status_IOUT = new ParamerusRegisterStatus(list_Status_IOUT);
            #endregion
            #region Status_TEMP
            List<ParamerusRegisterBit> list_Status_TEMP = new List<ParamerusRegisterBit>();
            list_Status_TEMP.Add(new ParamerusRegisterBit("Reserved", list_Status_TEMP.Count, BitStatus.BitNotImplemented));
            list_Status_TEMP.Add(new ParamerusRegisterBit("Reserved", list_Status_TEMP.Count, BitStatus.BitNotImplemented));
            list_Status_TEMP.Add(new ParamerusRegisterBit("Reserved", list_Status_TEMP.Count, BitStatus.BitNotImplemented));
            list_Status_TEMP.Add(new ParamerusRegisterBit("Reserved", list_Status_TEMP.Count, BitStatus.BitNotImplemented));
            list_Status_TEMP.Add(new ParamerusRegisterBit("UT Warning", list_Status_TEMP.Count, BitStatus.Warning));
            list_Status_TEMP.Add(new ParamerusRegisterBit("UT Fault", list_Status_TEMP.Count, BitStatus.Fault));
            list_Status_TEMP.Add(new ParamerusRegisterBit("OT Warning", list_Status_TEMP.Count, BitStatus.Warning));
            list_Status_TEMP.Add(new ParamerusRegisterBit("OT Fault", list_Status_TEMP.Count, BitStatus.Fault));
            Status_TEMP = new ParamerusRegisterStatus(list_Status_TEMP);
            #endregion
            #region Status_CML
            List<ParamerusRegisterBit> list_Status_CML = new List<ParamerusRegisterBit>();
            list_Status_CML.Add(new ParamerusRegisterBit("Other Memory/Logic Fault", list_Status_CML.Count, BitStatus.Fault));
            list_Status_CML.Add(new ParamerusRegisterBit("Other Comms Fault", list_Status_CML.Count, BitStatus.Fault));
            list_Status_CML.Add(new ParamerusRegisterBit("Reserved", list_Status_CML.Count, BitStatus.BitNotImplemented));
            list_Status_CML.Add(new ParamerusRegisterBit("Processor Fault", list_Status_CML.Count, BitStatus.Fault));
            list_Status_CML.Add(new ParamerusRegisterBit("Memory Fault", list_Status_CML.Count, BitStatus.Fault));
            list_Status_CML.Add(new ParamerusRegisterBit("PEC Fault", list_Status_CML.Count, BitStatus.Fault));
            list_Status_CML.Add(new ParamerusRegisterBit("Invalid Data", list_Status_CML.Count, BitStatus.Fault));
            list_Status_CML.Add(new ParamerusRegisterBit("Invalid Command", list_Status_CML.Count, BitStatus.Fault));
            Status_CML = new ParamerusRegisterStatus(list_Status_CML);
            #endregion
            #region Status_INPUT
            List<ParamerusRegisterBit> list_Status_INPUT = new List<ParamerusRegisterBit>();
            list_Status_INPUT.Add(new ParamerusRegisterBit("PIN OP Warning", list_Status_INPUT.Count, BitStatus.Warning));
            list_Status_INPUT.Add(new ParamerusRegisterBit("IIN OC Warning", list_Status_INPUT.Count, BitStatus.Warning));
            list_Status_INPUT.Add(new ParamerusRegisterBit("IIN OC Fault", list_Status_INPUT.Count, BitStatus.Fault));
            list_Status_INPUT.Add(new ParamerusRegisterBit("Unit off: Insufficient Vin", list_Status_INPUT.Count, BitStatus.Fault));
            list_Status_INPUT.Add(new ParamerusRegisterBit("Vin UV Fault", list_Status_INPUT.Count, BitStatus.Fault));
            list_Status_INPUT.Add(new ParamerusRegisterBit("Vin UV Warning", list_Status_INPUT.Count, BitStatus.Warning));
            list_Status_INPUT.Add(new ParamerusRegisterBit("Vin OV Warning", list_Status_INPUT.Count, BitStatus.Warning));
            list_Status_INPUT.Add(new ParamerusRegisterBit("Vin OV Fault", list_Status_INPUT.Count, BitStatus.Fault));
            Status_INPUT = new ParamerusRegisterStatus(list_Status_INPUT);
            #endregion
            #region Status_Fans_1_2
            List<ParamerusRegisterBit> list_Status_Fans_1_2 = new List<ParamerusRegisterBit>();
            list_Status_Fans_1_2.Add(new ParamerusRegisterBit("Air Flow Warning", list_Status_Fans_1_2.Count, BitStatus.Warning));
            list_Status_Fans_1_2.Add(new ParamerusRegisterBit("Air Flow Fault", list_Status_Fans_1_2.Count, BitStatus.Fault));
            list_Status_Fans_1_2.Add(new ParamerusRegisterBit("Fan 2 Speed Overridden", list_Status_Fans_1_2.Count, BitStatus.Fault));
            list_Status_Fans_1_2.Add(new ParamerusRegisterBit("Fan 1 Speed Overridden", list_Status_Fans_1_2.Count, BitStatus.Fault));
            list_Status_Fans_1_2.Add(new ParamerusRegisterBit("Fan 2 Warning", list_Status_Fans_1_2.Count, BitStatus.Warning));
            list_Status_Fans_1_2.Add(new ParamerusRegisterBit("Fan 1 Warning", list_Status_Fans_1_2.Count, BitStatus.Warning));
            list_Status_Fans_1_2.Add(new ParamerusRegisterBit("Fan 2 Fault", list_Status_Fans_1_2.Count, BitStatus.Fault));
            list_Status_Fans_1_2.Add(new ParamerusRegisterBit("Fan 1 Fault", list_Status_Fans_1_2.Count, BitStatus.Fault));
            Status_Fans_1_2 = new ParamerusRegisterStatus(list_Status_Fans_1_2);
            #endregion
            #region Status_Fans_3_4
            List<ParamerusRegisterBit> list_Status_Fans_3_4 = new List<ParamerusRegisterBit>();
            list_Status_Fans_3_4.Add(new ParamerusRegisterBit("Reserved", list_Status_Fans_3_4.Count, BitStatus.BitNotImplemented));
            list_Status_Fans_3_4.Add(new ParamerusRegisterBit("Reserved", list_Status_Fans_3_4.Count, BitStatus.BitNotImplemented));
            list_Status_Fans_3_4.Add(new ParamerusRegisterBit("Fan 4 Speed Overridden", list_Status_Fans_3_4.Count, BitStatus.Fault));
            list_Status_Fans_3_4.Add(new ParamerusRegisterBit("Fan 3 Speed Overridden", list_Status_Fans_3_4.Count, BitStatus.Fault));
            list_Status_Fans_3_4.Add(new ParamerusRegisterBit("Fan 4 Warning", list_Status_Fans_3_4.Count, BitStatus.Warning));
            list_Status_Fans_3_4.Add(new ParamerusRegisterBit("Fan 3 Warning", list_Status_Fans_3_4.Count, BitStatus.Warning));
            list_Status_Fans_3_4.Add(new ParamerusRegisterBit("Fan 4 Fault", list_Status_Fans_3_4.Count, BitStatus.Fault));
            list_Status_Fans_3_4.Add(new ParamerusRegisterBit("Fan 3 Fault", list_Status_Fans_3_4.Count, BitStatus.Fault));
            Status_Fans_3_4 = new ParamerusRegisterStatus(list_Status_Fans_3_4);
            #endregion
            #region Status_Mfr_Specific
            List<ParamerusRegisterBit> list_Status_Mfr_Specific= new List<ParamerusRegisterBit>();
            list_Status_Mfr_Specific.Add(new ParamerusRegisterBit("Manufacturer 0", list_Status_Mfr_Specific.Count, BitStatus.Fault));
            list_Status_Mfr_Specific.Add(new ParamerusRegisterBit("Manufacturer 1", list_Status_Mfr_Specific.Count, BitStatus.Fault));
            list_Status_Mfr_Specific.Add(new ParamerusRegisterBit("Manufacturer 2", list_Status_Mfr_Specific.Count, BitStatus.Fault));
            list_Status_Mfr_Specific.Add(new ParamerusRegisterBit("Manufacturer 3", list_Status_Mfr_Specific.Count, BitStatus.Fault));
            list_Status_Mfr_Specific.Add(new ParamerusRegisterBit("Manufacturer 4", list_Status_Mfr_Specific.Count, BitStatus.Fault));
            list_Status_Mfr_Specific.Add(new ParamerusRegisterBit("Manufacturer 5", list_Status_Mfr_Specific.Count, BitStatus.Fault));
            list_Status_Mfr_Specific.Add(new ParamerusRegisterBit("Manufacturer 6", list_Status_Mfr_Specific.Count, BitStatus.Fault));
            list_Status_Mfr_Specific.Add(new ParamerusRegisterBit("Manufacturer 7", list_Status_Mfr_Specific.Count, BitStatus.Fault));
            Status_Mfr_Specific = new ParamerusRegisterStatus(list_Status_Mfr_Specific);
            #endregion
            #region Status_Other
            List<ParamerusRegisterBit> list_Status_Other = new List<ParamerusRegisterBit>();
            list_Status_Other.Add(new ParamerusRegisterBit("Reserved", list_Status_Other.Count, BitStatus.BitNotImplemented));
            list_Status_Other.Add(new ParamerusRegisterBit("Output OR-ing device fault", list_Status_Other.Count, BitStatus.Fault));
            list_Status_Other.Add(new ParamerusRegisterBit("Input B OR-ing device fault", list_Status_Other.Count, BitStatus.Fault));
            list_Status_Other.Add(new ParamerusRegisterBit("Input A OR-ing device fault", list_Status_Other.Count, BitStatus.Fault));
            list_Status_Other.Add(new ParamerusRegisterBit("Input B fuse Or circuit breaker fault", list_Status_Other.Count, BitStatus.Fault));
            list_Status_Other.Add(new ParamerusRegisterBit("Input A fuse rr circuit breaker fault", list_Status_Other.Count, BitStatus.Fault));
            list_Status_Other.Add(new ParamerusRegisterBit("Reserved", list_Status_Other.Count, BitStatus.BitNotImplemented));
            list_Status_Other.Add(new ParamerusRegisterBit("Reserved", list_Status_Other.Count, BitStatus.BitNotImplemented));
            Status_Other = new ParamerusRegisterStatus(list_Status_Other);
            #endregion
        }



        #region Control line methods
        /// <summary>
        /// Подтяжка контрольной линии к 1
        /// </summary>
        /// <returns>true - если линия выставлена в 1, false - если не удалось подтянуть линию</returns>
        public bool ControlLineSetHigh()
        {
            if (Device == null)
                return false;
            LogicLevelResult control_status = ControlLineStatus();
            if (!control_status.Success)
                return false;
            if (control_status.Level == LogicLevel.Low)
                Device.Adapter.Set_Control(LogicLevel.High);
            Update();
            return true;
        }
        /// <summary>
        /// Подтяжка контрольной линии к 0
        /// </summary>
        /// <returns>true - если линия выставлена в 0, false - если не удалось выставить линию</returns>
        public bool ControlLineSetLow()
        {
            if (Device == null)
                return false;
            LogicLevelResult control_status = ControlLineStatus();
            if (!control_status.Success)
                return false;
            if (control_status.Level == LogicLevel.High)
                Device.Adapter.Set_Control(LogicLevel.Low);
            Update();
            return true;
        }
        /// <summary>
        /// Получение состояния контрольной линии, запрашивает состояние линии у устройства, и обновляет свойство ControlLine
        /// </summary>
        /// <returns></returns>
        public LogicLevelResult ControlLineStatus()
        {
            if (Device == null)
                return null;
            ControlLine = Device.Adapter.Get_Control();
            return ControlLine;
        }
        #endregion

        public void OutListParamDevs()
        {
            Debug.WriteLine("ID\tCode\tRail\tRead Only?\tStatus\tDecoded\tDecoded Formated\tEncoded");

            foreach (ParameterBase param in Device.Commands.Parameters)
            {

                if (param.Is_Meta_Command)
                    continue;

                if (!param.Show_To_User || !param.Phase_Available() || param.Is_Write_Only)
                    continue;

                param.Refresh();


                Debug.WriteLine(StringUtils.Join("\t", param.ID, param.Code, param.Page.Number,
                    (param.Is_Read_Only_Parameter) ? "Yes" : "No",
                    PMBusUtils.ACK_NACK(param.Last_Status), // "ACK" or "NACK"
                    param.oLatest_No_Immediate, param.Latest_No_Immediate_Formatted,
                    param.oLatest_Encoded_No_Immediate));
            }
        }
        public override string ToString()
        {
            if (Device == null)
                return "Error";
            return Device.ToString();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] String name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }


}
