
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ParamerusStudio.PMBus
{

    #region Converters
    /// <summary>
    /// Конвертер значений, отвечающий за преобразование состояния регистра статуса в цвет фона заголовка таблицы на форме
    /// </summary>
    public class StatusRegisterToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Brushes.Transparent;
            BitStatus bs = (BitStatus)value;
            switch(bs)
            {
                case BitStatus.Fault:
                    return new SolidColorBrush(Color.FromArgb(0xFF, 0xFE, 0x00, 0x00));
                case BitStatus.Warning:
                    return new SolidColorBrush(Color.FromArgb(0xFF, 0xFE, 0xA4, 0x00));
                default:
                    return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
    /// <summary>
    /// Конвертер значений, отвечающий за преобразование состояния регистра статуса в цвет текста заголовка таблицы на форме
    /// </summary>
    public class StatusRegisterToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Brushes.Black;
            BitStatus bs = (BitStatus)value;
            switch (bs)
            {
                case BitStatus.Fault:
                    return Brushes.White;
                case BitStatus.Warning:
                    return Brushes.Black;
                default:
                    return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
    #endregion

    public enum BitStatus
    {
        Fault,
        Warning,
        BitNotSet,
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
                _currentBitStatus = value;
                OnPropertyChanged();
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
        /// <summary>
        /// Текущее состояние регистра, Fault - если хотя бы один из бит выставлен как Fault, Warning - если хотя бы один из бит выставлен как Warning
        /// </summary>
        public BitStatus StatusRegister
        {
            get => _statusRegister;
            set
            {
                _statusRegister = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Коллекция бит регистра
        /// </summary>
        public List<ParamerusRegisterBit> RegisterBits { get; set; } = new List<ParamerusRegisterBit>();
        public ParamerusRegisterStatus(List<ParamerusRegisterBit> _registerBits)
        {
            RegisterBits = _registerBits;
            if (RegisterBits == null)
                return;
            foreach(ParamerusRegisterBit bit in RegisterBits)
            {
                bit.ParrentRegister = this;
                bit.PropertyChanged += Bit_PropertyChanged;
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
