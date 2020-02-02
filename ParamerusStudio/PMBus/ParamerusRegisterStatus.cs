using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Media;

namespace ParamerusStudio.PMBus
{

    public enum BitStatus
    {
        BitNotSet,
        Fault,
        Warning,
        BitNotImplemented
    }

    /// <summary>
    /// Класс, описывающий бит регистра статуса
    /// </summary>
    public class ParamerusRegisterBit : INotifyPropertyChanged
    {
        
        private BitStatus _currentBitStatus = BitStatus.BitNotSet;
        /// <summary>
        /// Текущее состояние бита
        /// </summary>
        public BitStatus CurrentStatusBit 
        { 
            get => _currentBitStatus;
            set
            {
                if(_currentBitStatus != value)
                {
                    _currentBitStatus = value;
                    OnPropertyChanged();
                }
            } 
        }

        /// <summary>
        /// Тип бита(Fault или Warning)
        /// </summary>
        public BitStatus TypeBit { get; private set; }
        /// <summary>
        /// Имя бита
        /// </summary>
        public String NameBit { get; set; }
        /// <summary>
        /// Номер бита
        /// </summary>
        public int BitNum { get; set; }
        /// <summary>
        /// Регистр, к которому относится бит
        /// </summary>
        public ParamerusRegisterStatus ParrentRegister { get; set; }
        public void SetBit(int bitVal)
        {
            CurrentStatusBit = (bitVal == 0) ? BitStatus.BitNotSet : TypeBit; 
        }

        public ParamerusRegisterBit()
        {

        }
        public ParamerusRegisterBit(String _nameBit, int _bitNum, BitStatus _type, BitStatus _status = BitStatus.BitNotSet)
        {
            NameBit = _nameBit;
            CurrentStatusBit = _status;
            BitNum = _bitNum;
            TypeBit = _type;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]String nameProperty = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameProperty));
        }
    }

    /// <summary>
    /// Класс, описывающий регистр статуса
    /// </summary>
    public class ParamerusRegisterStatus : INotifyPropertyChanged
    {
        private BitStatus _statusRegister = BitStatus.BitNotSet;
        private byte? _register_value;
        /// <summary>
        /// Текущее состояние регистра, Fault - если хотя бы один из бит выставлен как Fault, Warning - если хотя бы один из бит выставлен как Warning
        /// </summary>
        public BitStatus StatusRegister
        {
            get => _statusRegister;
            set
            {
                _statusRegister = value;
                if(_statusRegister == BitStatus.BitNotImplemented)
                {
                    foreach (var bit in RegisterBits)
                        bit.CurrentStatusBit = BitStatus.BitNotImplemented;
                }
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Коллекция бит регистра
        /// </summary>
        public List<ParamerusRegisterBit> RegisterBits { get; set; } = new List<ParamerusRegisterBit>();
        public byte? RegisterValue
        {
            get => _register_value;
            set
            {
                if(value == null)
                {
                    _register_value = value;
                    StatusRegister = BitStatus.BitNotImplemented;
                    OnPropertyChanged();
                }  
                else if(_register_value != value)
                {
                    _register_value = value;
                    bool isFaultSet = false;
                    bool isWarningSet = false;
                    for (int i=0;i<8;i++)
                    {
                        RegisterBits[i].SetBit(((int)_register_value >> i) & 0x1);
                        switch(RegisterBits[i].CurrentStatusBit)
                        {
                            case BitStatus.Fault:
                                isFaultSet = true;
                                break;
                            case BitStatus.Warning:
                                isWarningSet = true;
                                break;
                            default:
                                break;
                        }
                    }
                    if (isFaultSet)
                        StatusRegister = BitStatus.Fault;
                    else if (isWarningSet)
                        StatusRegister = BitStatus.Warning;
                    else
                        StatusRegister = BitStatus.BitNotSet;
                    OnPropertyChanged();
                }
            }
        }
        public ParamerusRegisterStatus(List<ParamerusRegisterBit> _registerBits)
        {
            RegisterBits = _registerBits;
            if (RegisterBits == null)
                return;
            foreach(ParamerusRegisterBit bit in RegisterBits)
            {
                bit.ParrentRegister = this;
                //bit.PropertyChanged += Bit_PropertyChanged;
            }
        }

        public ParamerusRegisterStatus()
        {

        }

        /// <summary>
        /// Событие, возникающее при изменнении состояния одного из бит регистра
        /// </summary>
        /// <param name="sender">Бит, поменявший состояние</param>
        /// <param name="e"></param>
        public void Bit_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            bool isWarningSet = false;
            foreach (ParamerusRegisterBit bit in RegisterBits)
            {
                switch(bit.CurrentStatusBit)
                {
                    case BitStatus.Fault:
                        StatusRegister = BitStatus.Fault;
                        return;
                    case BitStatus.Warning:
                        isWarningSet = true;
                        break;
                    default:
                        break;
                }
            }
            if (isWarningSet)
                StatusRegister = BitStatus.Warning;
            else
                StatusRegister = BitStatus.BitNotSet;

        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]String nameProperty = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameProperty));
        }
    }
}
