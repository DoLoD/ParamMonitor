using ParamerusStudio.Components;
using ParamerusStudio.PMBus.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TIDP.PMBus;
using TIDP.SAA;

namespace ParamerusStudio.PMBus
{
    public enum PMBusMode
    {
        Linear = 0x0,
        VID = 0x1,
        Direct = 0x2
    }
    public enum PMBusCode
    {
        VOUT_MODE = 0x20,
        VOUT_COMMAND = 0x21,
        VOUT_TRIM = 0x22,
        VOUT_CAL_OFFSET = 0x23,
        VOUT_OV_FAULT_LIMIT = 0x40,
        VOUT_OV_FAULT_RESPONSE = 0x41,
        VOUT_OV_WARN_LIMIT = 0x42,
        VOUT_UV_WARN_LIMIT = 0x43,
        VOUT_UV_FAULT_LIMIT = 0x44,
        VOUT_UV_FAULT_RESPONS = 0x45,
        IOUT_OC_FAULT_LIMIT = 0x46,
        IOUT_OC_FAULT_RESPONSE = 0x47,
        IOUT_OC_LV_FAULT_LIMIT = 0x48,
        IOUT_OC_LV_FAULT_RESPONSE = 0x49,
        IOUT_OC_WARN_LIMIT = 0x4A,
        IOUT_UC_FAULT_LIMIT = 0x4B,
        IOUT_UC_FAULT_RESPONSE = 0x4C,
        OT_FAULT_LIMIT = 0x4F,
        OT_FAULT_RESPONSE = 0x50,
        OT_WARN_LIMIT = 0x51,
        UT_WARN_LIMIT = 0x52,
        UT_FAULT_LIMIT = 0x53,
        UT_FAULT_RESPONSE = 0x54,
        VIN_OV_FAULT_LIMIT = 0x55,
        VIN_OV_FAULT_RESPONSE = 0x56,
        VIN_OV_WARN_LIMIT = 0x57,
        VIN_UV_WARN_LIMIT = 0x58,
        VIN_UV_FAULT_LIMIT = 0x59,
        VIN_UV_FAULT_RESPONSE = 0x5A,
        IIN_OC_FAULT_LIMIT = 0x5B,
        IIN_OC_FAULT_RESPONSE = 0x5C,
        IIN_OC_WARN_LIMIT = 0x5D,
        POWER_GOOD_ON = 0x5E,
        POWER_GOOD_OFF = 0x5F,
        TON_DELAY = 0x60,
        TON_RISE = 0x61,
        TON_MAX_FAULT_LIMIT = 0x62,
        TON_MAX_FAULT_RESPONSE = 0x63,
        TOFF_DELAY = 0x64,
        TOFF_FALL = 0x65,
        TOFF_MAX_WARN_LIMIT = 0x66,
        POUT_OP_FAULT_LIMIT = 0x68,
        POUT_OP_FAULT_RESPONSE = 0x69,
        POUT_OP_WARN_LIMIT = 0x6A,
        PIN_OP_WARN_LIMIT = 0x6B,
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
    public enum PMBusLimitType
    {
        Warning,
        Fault
    }

    public class Limit : INotifyPropertyChanged
    {
        private ushort? _value = 0;
        public PMBusCode Code_limit { get; set; }
        public PMBusLimitType LimitType { get; set; }
        public ushort? Value 
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        public Limit(PMBusCode _code, PMBusLimitType _type)
        {
            Code_limit = _code;
            LimitType = _type;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]String name ="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
    /// <summary>
    /// Класс, описывающий PMBus устройство
    /// </summary>
    public class ParamerusPMBusDevice : INotifyPropertyChanged
    {

        #region Static Methods

        /// <summary>
        /// Поиск SMBus-адаптера
        /// </summary>
        /// <returns>Найденные адаптер или null если не найден</returns>
        public static SMBusAdapter SMBusAdaptherSearch()
        {
            if (SMBusAdapter.Discover() == 0)
            {
#if VIRTUAL_DEVICE
                SMBusAdapter.Register_Driver(new VirtualDeviceDriverFactory());
                if (SMBusAdapter.Discover() == 0)
                {
#endif
                return null;
#if VIRTUAL_DEVICE
            }
#endif
            }

            return SMBusAdapter.Adapter;
        }

        /// <summary>
        /// Поиск PMBus устройств
        /// </summary>
        /// <returns>Коллекция найденных устройств, null - если устройства не найдены</returns>
        public static List<ParamerusPMBusDevice> PMBusInit()
        {
            var opts = new PMBusDevice.DiscoverOptions();
            opts.Scan_Mode = PMBusDevice.ScanMode.DeviceCode;
            if (PMBusDevice.Discover(opts) == 0)
            {

                opts.Scan_Mode = PMBusDevice.ScanMode.DeviceID;
                if (PMBusDevice.Discover(opts) == 0)
                {
                    opts.Scan_Mode = PMBusDevice.ScanMode.TPS53951;
                    if (PMBusDevice.Discover(opts) == 0)
                        return null;
                }
            }
            List<ParamerusPMBusDevice> listDevices = new List<ParamerusPMBusDevice>();
            foreach (var dev in PMBusDevice.Devices)
            {
                listDevices.Add(new ParamerusPMBusDevice(dev));
            }

            return listDevices;
        }
        #endregion

        #region Properties
        private List<PMBusCode> ConstantlyUpdatetCommands;
        private List<PMBusCode> OnceExecutedCommands;
        private Dictionary<PMBusCode, PMBusCommandBase> pmbus_commands;
        private Task updateTask = null;
        private CancellationTokenSource stopUpdateTask;
        public ObservableDictionary<PMBusCode, Limit> VIN_Limits { get; set; }
        public ObservableDictionary<PMBusCode, Limit> VOUT_Limits { get; set; }
        public ObservableDictionary<PMBusCode, Limit> IOUT_Limits { get; set; }
        public ObservableDictionary<PMBusCode, Limit> POUT_Limits { get; set; }
        public ObservableDictionary<PMBusCode, Limit> TEMP_Limits { get; set; }
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

        #region Input vals
        private double? _read_vin = 0.0;
        private double? _read_iin = 0.0;
        private double? _read_vout = 0.0;
        private double? _read_iout = 0.0;
        private double? _read_temp_int = 0.0;
        private double? _read_temp_ext = 0.0;
        private double? _freq = 0.0;
        private double? _pout = 0.0;

        public double? Read_vin
        {
            get => _read_vin;
            set
            {
                if (_read_vin == value)
                    return;
                _read_vin = value;
                OnPropertyChanged();
            }
        }
        public double? Read_iin
        {
            get => _read_iin;
            set
            {
                if (_read_iin == value)
                    return;
                _read_iin = value;
                OnPropertyChanged();
            }
        }
        public double? Read_vout
        {
            get => _read_vout;
            set
            {
                if (_read_vout == value)
                    return;
                _read_vout = value;
                OnPropertyChanged();
                Read_pout = (_read_vout != null && _read_iout != null) ? _read_vout * _read_iout : null;
            }
        }
        public double? Read_iout
        {
            get => _read_iout;
            set
            {
                if (_read_iout == value)
                    return;
                _read_iout = value;
                OnPropertyChanged();
                Read_pout = (_read_vout != null && _read_iout != null) ? _read_vout * _read_iout : null;
            }
        }
        public double? Read_temp_int
        {
            get => _read_temp_int;
            set
            {
                if (_read_temp_int == value)
                    return;
                _read_temp_int = value;
                OnPropertyChanged();
            }
        }
        public double? Read_temp_ext
        {
            get => _read_temp_ext;
            set
            {
                if (_read_temp_ext == value)
                    return;
                _read_temp_ext = value;
                OnPropertyChanged();
            }
        }
        public double? Read_freq
        {
            get => _freq;
            set
            {
                if (_freq == value)
                    return;
                _freq = value;
                OnPropertyChanged();
            }
        }
        public double? Read_pout
        {
            get => _pout;
            set
            {
                _pout = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Control line properties
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

        #endregion

        #region Device property
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
        #endregion
        #endregion

        #region Init methods
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
            List<ParamerusRegisterBit> list_Status_Mfr_Specific = new List<ParamerusRegisterBit>();
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
        private void InitReadCommands()
        {
            ConstantlyUpdatetCommands = new List<PMBusCode>();
            ConstantlyUpdatetCommands.Add(PMBusCode.READ_VOUT);
            ConstantlyUpdatetCommands.Add(PMBusCode.READ_IOUT);
            ConstantlyUpdatetCommands.Add(PMBusCode.READ_VIN);
            ConstantlyUpdatetCommands.Add(PMBusCode.READ_IIN);
            ConstantlyUpdatetCommands.Add(PMBusCode.READ_TEMP_1);
            ConstantlyUpdatetCommands.Add(PMBusCode.READ_TEMP_2);
            ConstantlyUpdatetCommands.Add(PMBusCode.READ_FREQ);

            ConstantlyUpdatetCommands.Add(PMBusCode.STATUS_VOUT);
            ConstantlyUpdatetCommands.Add(PMBusCode.STATUS_IOUT);
            ConstantlyUpdatetCommands.Add(PMBusCode.STATUS_TEMP);
            ConstantlyUpdatetCommands.Add(PMBusCode.STATUS_CML);
            ConstantlyUpdatetCommands.Add(PMBusCode.STATUS_INPUT);
            ConstantlyUpdatetCommands.Add(PMBusCode.STATUS_FANS_1_2);
            ConstantlyUpdatetCommands.Add(PMBusCode.STATUS_FANS_3_4);
            ConstantlyUpdatetCommands.Add(PMBusCode.STATUS_MFR_SPECIFIC);
            ConstantlyUpdatetCommands.Add(PMBusCode.STATUS_OTHER);
        }
        private void InitPMBusCommands()
        {
            pmbus_commands = new Dictionary<PMBusCode, PMBusCommandBase>();
            pmbus_commands.Add(PMBusCode.READ_VOUT, new ReadVoutCommand(this, ((e) => Read_vout = (double?)e)));
            pmbus_commands.Add(PMBusCode.READ_IOUT, new ReadIoutCommand(this, ((e) => Read_iout = (double?)e)));
            pmbus_commands.Add(PMBusCode.READ_VIN, new ReadVinCommand(this, ((e) => Read_vin = (double?)e)));
            pmbus_commands.Add(PMBusCode.READ_IIN, new ReadIinCommand(this, ((e) => Read_iin = (double?)e)));
            pmbus_commands.Add(PMBusCode.READ_TEMP_1, new ReadTemp1Command(this, ((e) => Read_temp_int = (double?)e)));
            pmbus_commands.Add(PMBusCode.READ_TEMP_2, new ReadTemp2Command(this, ((e) => Read_temp_ext = (double?)e)));
            pmbus_commands.Add(PMBusCode.READ_FREQ, new ReadFreqCommand(this, ((e) => Read_freq = (double?)e)));

            pmbus_commands.Add(PMBusCode.STATUS_VOUT, new StatusVOUTCommand(this, ((e) => Status_VOUT.RegisterValue = (byte?)e)));
            pmbus_commands.Add(PMBusCode.STATUS_IOUT, new StatusIOUTCommand(this, ((e) => Status_IOUT.RegisterValue = (byte?)e)));
            pmbus_commands.Add(PMBusCode.STATUS_TEMP, new StatusTempCommand(this, ((e) => Status_TEMP.RegisterValue = (byte?)e)));
            pmbus_commands.Add(PMBusCode.STATUS_CML, new StatusCMLCommand(this, ((e) => Status_CML.RegisterValue = (byte?)e)));
            pmbus_commands.Add(PMBusCode.STATUS_INPUT, new StatusINPUTCommand(this, ((e) => Status_INPUT.RegisterValue = (byte?)e)));
            pmbus_commands.Add(PMBusCode.STATUS_FANS_1_2, new StatusFANS12Command(this, ((e) => Status_Fans_1_2.RegisterValue = (byte?)e)));
            pmbus_commands.Add(PMBusCode.STATUS_FANS_3_4, new StatusFANS34Command(this, ((e) => Status_Fans_3_4.RegisterValue = (byte?)e)));
            pmbus_commands.Add(PMBusCode.STATUS_MFR_SPECIFIC, new StatusMFRSpecificCommand(this, ((e) => Status_Mfr_Specific.RegisterValue = (byte?)e)));
            pmbus_commands.Add(PMBusCode.STATUS_OTHER, new StatusOTHERCommand(this, ((e) => Status_Other.RegisterValue = (byte?)e)));

            pmbus_commands.Add(PMBusCode.VIN_OV_FAULT_LIMIT, new VinOVFaultLimitCommand(this, ((e) => VIN_Limits[PMBusCode.VIN_OV_FAULT_LIMIT].Value = (ushort?)e)));
            pmbus_commands.Add(PMBusCode.VIN_OV_FAULT_RESPONSE, new VinOVFaultResponeCommand(this, ((e) => VIN_Limits[PMBusCode.VIN_OV_FAULT_RESPONSE].Value = (ushort?)e)));
            pmbus_commands.Add(PMBusCode.VIN_OV_WARN_LIMIT, new VinOVWarnLimitCommand(this, ((e) => VIN_Limits[PMBusCode.VIN_OV_WARN_LIMIT].Value = (ushort?)e)));
            pmbus_commands.Add(PMBusCode.VIN_UV_WARN_LIMIT, new VinUVWarnLimitCommand(this, ((e) => VIN_Limits[PMBusCode.VIN_UV_WARN_LIMIT].Value = (ushort?)e)));
            pmbus_commands.Add(PMBusCode.VIN_UV_FAULT_LIMIT, new VinUVFaultLimitCommand(this, ((e) => VIN_Limits[PMBusCode.VIN_UV_FAULT_LIMIT].Value = (ushort?)e)));
            pmbus_commands.Add(PMBusCode.VIN_UV_FAULT_RESPONSE, new VinUVFaultResponceCommand(this, ((e) => VIN_Limits[PMBusCode.VIN_UV_FAULT_RESPONSE].Value = (ushort?)e)));

            pmbus_commands.Add(PMBusCode.VOUT_OV_FAULT_LIMIT, new VoutOVFaultLimitCommand(this, ((e) => VOUT_Limits[PMBusCode.VOUT_OV_FAULT_LIMIT].Value = Convert.ToUInt16(e))));
            pmbus_commands.Add(PMBusCode.VOUT_OV_FAULT_RESPONSE, new VoutOVFaultResponceCommand(this, ((e) => VOUT_Limits[PMBusCode.VOUT_OV_FAULT_RESPONSE].Value = Convert.ToUInt16(e))));
            pmbus_commands.Add(PMBusCode.VOUT_OV_WARN_LIMIT, new VoutOVWarnLimitCommand(this, ((e) => VOUT_Limits[PMBusCode.VOUT_OV_WARN_LIMIT].Value = Convert.ToUInt16(e))));
            pmbus_commands.Add(PMBusCode.VOUT_UV_WARN_LIMIT, new VoutUVWarnLimitCommand(this, ((e) => VOUT_Limits[PMBusCode.VOUT_UV_WARN_LIMIT].Value = Convert.ToUInt16(e))));
            pmbus_commands.Add(PMBusCode.VOUT_UV_FAULT_LIMIT, new VoutUVFaultLimitCommand(this, ((e) => VOUT_Limits[PMBusCode.VOUT_UV_FAULT_LIMIT].Value = Convert.ToUInt16(e))));
            pmbus_commands.Add(PMBusCode.VOUT_UV_FAULT_RESPONS, new VoutUVFaultResponceCommand(this, ((e) => VOUT_Limits[PMBusCode.VOUT_UV_FAULT_RESPONS].Value = Convert.ToUInt16(e))));

            pmbus_commands.Add(PMBusCode.IOUT_OC_FAULT_LIMIT, new IoutOCFaultLimitCommand(this, ((e) => IOUT_Limits[PMBusCode.IOUT_OC_FAULT_LIMIT].Value = Convert.ToUInt16(e))));
            pmbus_commands.Add(PMBusCode.IOUT_OC_FAULT_RESPONSE, new IoutOCFaultResponseCommand(this, ((e) => IOUT_Limits[PMBusCode.IOUT_OC_FAULT_RESPONSE].Value = Convert.ToUInt16(e))));
            pmbus_commands.Add(PMBusCode.IOUT_OC_WARN_LIMIT, new IoutOCWarnLimitCommand(this, ((e) => IOUT_Limits[PMBusCode.IOUT_OC_WARN_LIMIT].Value = Convert.ToUInt16(e))));

            pmbus_commands.Add(PMBusCode.POUT_OP_FAULT_LIMIT, new PoutOPFaultLimitCommand(this, ((e) => POUT_Limits[PMBusCode.POUT_OP_FAULT_LIMIT].Value = Convert.ToUInt16(e))));
            pmbus_commands.Add(PMBusCode.POUT_OP_WARN_LIMIT, new PoutOPWarnLimitCommand(this, ((e) => POUT_Limits[PMBusCode.POUT_OP_WARN_LIMIT].Value = Convert.ToUInt16(e))));

            pmbus_commands.Add(PMBusCode.OT_FAULT_LIMIT, new OTFaultLimitCommand(this, ((e) => TEMP_Limits[PMBusCode.OT_FAULT_LIMIT].Value = Convert.ToUInt16(e))));
            pmbus_commands.Add(PMBusCode.OT_FAULT_RESPONSE, new OTFaultResponseCommand(this, ((e) => TEMP_Limits[PMBusCode.OT_FAULT_RESPONSE].Value = Convert.ToUInt16(e))));
            pmbus_commands.Add(PMBusCode.OT_WARN_LIMIT, new OTWarnLimitCommand(this, ((e) => TEMP_Limits[PMBusCode.OT_WARN_LIMIT].Value = Convert.ToUInt16(e))));
            pmbus_commands.Add(PMBusCode.UT_WARN_LIMIT, new UTWarnLimitCommand(this, ((e) => TEMP_Limits[PMBusCode.UT_WARN_LIMIT].Value = Convert.ToUInt16(e))));
            pmbus_commands.Add(PMBusCode.UT_FAULT_LIMIT, new UTFaultLimitCommand(this, ((e) => TEMP_Limits[PMBusCode.UT_FAULT_LIMIT].Value = Convert.ToUInt16(e))));
            pmbus_commands.Add(PMBusCode.UT_FAULT_RESPONSE, new UTFaultResponseCommand(this, ((e) => TEMP_Limits[PMBusCode.UT_FAULT_RESPONSE].Value = Convert.ToUInt16(e))));
        }
        private void InitOnceExecutedCommands()
        {
            OnceExecutedCommands = new List<PMBusCode>();
            OnceExecutedCommands.Add(PMBusCode.VIN_OV_FAULT_LIMIT);
            OnceExecutedCommands.Add(PMBusCode.VIN_OV_FAULT_RESPONSE);
            OnceExecutedCommands.Add(PMBusCode.VIN_OV_WARN_LIMIT);
            OnceExecutedCommands.Add(PMBusCode.VIN_UV_WARN_LIMIT);
            OnceExecutedCommands.Add(PMBusCode.VIN_UV_FAULT_LIMIT);
            OnceExecutedCommands.Add(PMBusCode.VIN_UV_FAULT_RESPONSE);
            OnceExecutedCommands.Add(PMBusCode.VOUT_OV_FAULT_LIMIT);
            OnceExecutedCommands.Add(PMBusCode.VOUT_OV_FAULT_RESPONSE);
            OnceExecutedCommands.Add(PMBusCode.VOUT_OV_WARN_LIMIT);
            OnceExecutedCommands.Add(PMBusCode.VOUT_UV_WARN_LIMIT);
            OnceExecutedCommands.Add(PMBusCode.VOUT_UV_FAULT_LIMIT);
            OnceExecutedCommands.Add(PMBusCode.VOUT_UV_FAULT_RESPONS);
            OnceExecutedCommands.Add(PMBusCode.IOUT_OC_FAULT_LIMIT);
            OnceExecutedCommands.Add(PMBusCode.IOUT_OC_FAULT_RESPONSE);
            OnceExecutedCommands.Add(PMBusCode.IOUT_OC_WARN_LIMIT);
            OnceExecutedCommands.Add(PMBusCode.POUT_OP_FAULT_LIMIT);
            OnceExecutedCommands.Add(PMBusCode.POUT_OP_WARN_LIMIT);
            OnceExecutedCommands.Add(PMBusCode.OT_FAULT_LIMIT);
            OnceExecutedCommands.Add(PMBusCode.OT_FAULT_RESPONSE);
            OnceExecutedCommands.Add(PMBusCode.OT_WARN_LIMIT);
            OnceExecutedCommands.Add(PMBusCode.UT_WARN_LIMIT);
            OnceExecutedCommands.Add(PMBusCode.UT_FAULT_LIMIT);
            OnceExecutedCommands.Add(PMBusCode.UT_FAULT_RESPONSE);
        }
        private void InitVINLimits()
        {
            VIN_Limits = new ObservableDictionary<PMBusCode, Limit>();
            VIN_Limits.Add(PMBusCode.VIN_OV_FAULT_LIMIT, new Limit(PMBusCode.VIN_OV_FAULT_LIMIT, PMBusLimitType.Fault));
            VIN_Limits.Add(PMBusCode.VIN_OV_FAULT_RESPONSE, new Limit(PMBusCode.VIN_OV_FAULT_RESPONSE, PMBusLimitType.Fault));
            VIN_Limits.Add(PMBusCode.VIN_OV_WARN_LIMIT, new Limit(PMBusCode.VIN_OV_WARN_LIMIT, PMBusLimitType.Warning));
            VIN_Limits.Add(PMBusCode.VIN_UV_WARN_LIMIT, new Limit(PMBusCode.VIN_UV_WARN_LIMIT, PMBusLimitType.Warning));
            VIN_Limits.Add(PMBusCode.VIN_UV_FAULT_LIMIT, new Limit(PMBusCode.VIN_UV_FAULT_LIMIT, PMBusLimitType.Fault));
            VIN_Limits.Add(PMBusCode.VIN_UV_FAULT_RESPONSE, new Limit(PMBusCode.VIN_UV_FAULT_RESPONSE, PMBusLimitType.Fault));
        }
        private void InitVOUTLimits()
        {
            VOUT_Limits = new ObservableDictionary<PMBusCode, Limit>();
            VOUT_Limits.Add(PMBusCode.VOUT_OV_FAULT_LIMIT, new Limit(PMBusCode.VOUT_OV_FAULT_LIMIT, PMBusLimitType.Fault));
            VOUT_Limits.Add(PMBusCode.VOUT_OV_FAULT_RESPONSE, new Limit(PMBusCode.VOUT_OV_FAULT_RESPONSE, PMBusLimitType.Fault));
            VOUT_Limits.Add(PMBusCode.VOUT_OV_WARN_LIMIT, new Limit(PMBusCode.VOUT_OV_WARN_LIMIT, PMBusLimitType.Warning));
            VOUT_Limits.Add(PMBusCode.VOUT_UV_WARN_LIMIT, new Limit(PMBusCode.VOUT_UV_WARN_LIMIT, PMBusLimitType.Warning));
            VOUT_Limits.Add(PMBusCode.VOUT_UV_FAULT_LIMIT, new Limit(PMBusCode.VOUT_UV_FAULT_LIMIT, PMBusLimitType.Fault));
            VOUT_Limits.Add(PMBusCode.VOUT_UV_FAULT_RESPONS, new Limit(PMBusCode.VOUT_UV_FAULT_RESPONS, PMBusLimitType.Fault));
        }
        private void InitIOUTLimits()
        {
            IOUT_Limits = new ObservableDictionary<PMBusCode, Limit>();
            IOUT_Limits.Add(PMBusCode.IOUT_OC_FAULT_LIMIT, new Limit(PMBusCode.IOUT_OC_FAULT_LIMIT, PMBusLimitType.Fault));
            IOUT_Limits.Add(PMBusCode.IOUT_OC_FAULT_RESPONSE, new Limit(PMBusCode.IOUT_OC_FAULT_RESPONSE, PMBusLimitType.Fault));
            IOUT_Limits.Add(PMBusCode.IOUT_OC_WARN_LIMIT, new Limit(PMBusCode.IOUT_OC_WARN_LIMIT, PMBusLimitType.Warning)); 
        }
        private void InitPOUTLimits()
        {
            POUT_Limits = new ObservableDictionary<PMBusCode, Limit>();
            POUT_Limits.Add(PMBusCode.POUT_OP_FAULT_LIMIT, new Limit(PMBusCode.POUT_OP_FAULT_LIMIT, PMBusLimitType.Fault));
            POUT_Limits.Add(PMBusCode.POUT_OP_WARN_LIMIT, new Limit(PMBusCode.POUT_OP_WARN_LIMIT, PMBusLimitType.Warning));
        }
        private void InitTEMPLimits()
        {
            TEMP_Limits = new ObservableDictionary<PMBusCode, Limit>();
            TEMP_Limits.Add(PMBusCode.OT_FAULT_LIMIT, new Limit(PMBusCode.OT_FAULT_LIMIT, PMBusLimitType.Fault));
            TEMP_Limits.Add(PMBusCode.OT_FAULT_RESPONSE, new Limit(PMBusCode.OT_FAULT_RESPONSE, PMBusLimitType.Fault));
            TEMP_Limits.Add(PMBusCode.OT_WARN_LIMIT, new Limit(PMBusCode.OT_WARN_LIMIT, PMBusLimitType.Warning));
            TEMP_Limits.Add(PMBusCode.UT_WARN_LIMIT, new Limit(PMBusCode.UT_WARN_LIMIT, PMBusLimitType.Warning));
            TEMP_Limits.Add(PMBusCode.UT_FAULT_LIMIT, new Limit(PMBusCode.UT_FAULT_LIMIT, PMBusLimitType.Fault));
            TEMP_Limits.Add(PMBusCode.UT_FAULT_RESPONSE, new Limit(PMBusCode.UT_FAULT_RESPONSE, PMBusLimitType.Fault));
        }
        private void InitLimits()
        {
            InitVINLimits();
            InitVOUTLimits();
            InitIOUTLimits();
            InitPOUTLimits();
            InitTEMPLimits();
        }

        public ParamerusPMBusDevice(PMBusDevice device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device), "Device cannot be null");
            Device = device;
            InitLimits();
            InitRegs();
            InitReadCommands();
            InitOnceExecutedCommands();
            InitPMBusCommands();
            UpdateAllCommands();
        }
        #endregion

        #region Update methods

        void UpdateAllCommands()
        {
            if (pmbus_commands != null && pmbus_commands.Count != 0)
            {
                foreach (var cmd in pmbus_commands)
                {
                    cmd.Value.ReadCmd();
                }
            }
        }
        void UpdateCommands(List<PMBusCode> cmds)
        {
            if (cmds != null && cmds.Count != 0 && pmbus_commands != null && pmbus_commands.Count != 0)
            {
                foreach (var cmd in cmds)
                {
                    object res = pmbus_commands[cmd].ReadCmd();
                }
            }
        }
        /// <summary>
        /// Обновление параметров устройства
        /// </summary>
        private void Update()
        {
            while (!stopUpdateTask.IsCancellationRequested)
            {
                ControlLineStatusUpdate();
                UpdateCommands(ConstantlyUpdatetCommands);
                Thread.Sleep(TimeSpan.FromTicks(1000));
            }

        }
        
        public void StartListener()
        {
            stopUpdateTask = new CancellationTokenSource();
            updateTask = Task.Factory.StartNew(new Action(Update));
        }

        public void StopListener()
        {
            stopUpdateTask?.Cancel();
            updateTask?.Wait();
        }
        #endregion

        #region Control line methods
        /// <summary>
        /// Подтяжка контрольной линии к 1
        /// </summary>
        /// <returns>true - если линия выставлена в 1, false - если не удалось подтянуть линию</returns>
        public bool ControlLineSetHigh()
        {
            if (Device == null)
                return false;
            LogicLevelResult control_status = ControlLineStatusUpdate();
            if (!control_status.Success)
                return false;
            if (control_status.Level == LogicLevel.Low)
                Device.Adapter.Set_Control(LogicLevel.High);
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
            LogicLevelResult control_status = ControlLineStatusUpdate();
            if (!control_status.Success)
                return false;
            if (control_status.Level == LogicLevel.High)
                Device.Adapter.Set_Control(LogicLevel.Low);
            return true;
        }
        /// <summary>
        /// Получение состояния контрольной линии, запрашивает состояние линии у устройства, и обновляет свойство ControlLine
        /// </summary>
        /// <returns></returns>
        public LogicLevelResult ControlLineStatusUpdate()
        {
            if (Device == null)
                return null;
            ControlLine = Device.Adapter.Get_Control();
            return ControlLine;
        }
        #endregion

        #region Release INotifyPropertyChanged and override ToString()
        public override string ToString()
        {
            if (Device == null)
                return "Error";
            return Device.ToString();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] String name = "")
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
            catch (Exception) { }
        }
        #endregion

    }


}
