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

namespace ParamerusStudio.Components
{
    public class EventList<T> : List<T>
    {
        public event EventHandler OnAdd;

        public new void Add(T item) // "new" to avoid compiler-warnings, because we're hiding a method from base-class
        {
            base.Add(item);
            OnAdd?.Invoke(this, null);
        }
    }

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
            throw new NotImplementedException();
        }
    }

    public enum BitStatus
    {
        Fault,
        Warning,
        BitNotSet,
        BitNotImplemented
    }

    public class ParamerusRegisterBit : INotifyPropertyChanged
    {
        
        private BitStatus _currentBitStatus = BitStatus.BitNotSet;
        public BitStatus CurrentStatusBit 
        { 
            get => _currentBitStatus;
            set
            {
                _currentBitStatus = value;
                OnPropertyChanged();
            } 
        }
        public String NameBit { get; set; }
        public int BitNum { get; set; }
        public ParamerusRegisterStatus ParrentRegister { get; set; }

        public ParamerusRegisterBit()
        {

        }

        public ParamerusRegisterBit(String _nameBit,int _bitNum ,BitStatus _status = BitStatus.BitNotSet)
        {
            NameBit = _nameBit;
            CurrentStatusBit = _status;
            BitNum = _bitNum;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]String nameProperty = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameProperty));
        }
    }


    public class ParamerusRegisterStatus : INotifyPropertyChanged
    {
        public EventList<ParamerusRegisterBit> RegisterBits { get; set; } = new EventList<ParamerusRegisterBit>();
        private BitStatus _statusRegister = BitStatus.BitNotSet;
        public BitStatus StatusRegister
        {
            get => _statusRegister;
            set
            {
                _statusRegister = value;
                OnPropertyChanged();
            }
        }

        public ParamerusRegisterStatus(EventList<ParamerusRegisterBit> _registerBits)
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
            RegisterBits.OnAdd += RegisterBits_OnAdd;
        }

        private void RegisterBits_OnAdd(object sender, EventArgs e)
        {
            if(RegisterBits.Last() != null)
                RegisterBits.Last().ParrentRegister = this;
        }

        public void Bit_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            bool isWarningSet = false;
            bool isFaultSet = false;
            foreach (ParamerusRegisterBit bit in RegisterBits)
            {
                switch(bit.CurrentStatusBit)
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

        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]String nameProperty = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameProperty));
        }
    }
}
