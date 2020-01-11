using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ParamerusStudio.Components
{
    public class StatesButtonConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values[0] == null || values[1] == null)
                throw new ArgumentNullException(nameof(values),"Argument null");
            if(values[0] is int && values[1] is List<String>)
            {
                int ind = (int)values[0];
                List<String> states = values[1] as List<String>;
                if (states.Count <= ind || (ind < -1))
                    throw new ArgumentOutOfRangeException(nameof(values),"Argument out of boundary states indicator button.");
                else if (ind == -1)
                    return "";
                else
                    return states[ind];
            }
            throw new ArgumentException("Argument type error.", nameof(values));
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Логика взаимодействия для ParamerusIndicButton.xaml
    /// </summary>
    public partial class ParamerusIndicButton : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty IndicatorBackgroundProperty = DependencyProperty.Register("IndicatorBackground", typeof(Brush), typeof(ParamerusIndicButton));
        public static readonly DependencyProperty StatesButtonProperty = DependencyProperty.Register("StatesButton", typeof(List<String>), typeof(ParamerusIndicButton), new PropertyMetadata(new List<String>()));
        public static readonly DependencyProperty CurrentStateIndexProperty = DependencyProperty.Register("CurrentStateIndex", typeof(int), typeof(ParamerusIndicButton), new PropertyMetadata(0, OnStateIndexPropertyChanged));
        public static readonly DependencyProperty HorizontalIndicatorAlignmentProperty = DependencyProperty.Register("HorizontalIndicatorAlignment", typeof(HorizontalAlignment), typeof(ParamerusIndicButton));
        public static readonly DependencyProperty VerticalIndicatorAlignmentProperty = DependencyProperty.Register("VerticalIndicatorAlignment", typeof(VerticalAlignment), typeof(ParamerusIndicButton));
        public static readonly DependencyProperty MarginIndicatorProperty = DependencyProperty.Register("MarginIndicator", typeof(Thickness), typeof(ParamerusIndicButton));
        public static readonly DependencyProperty MarginContentProperty = DependencyProperty.Register("MarginContent", typeof(Thickness), typeof(ParamerusIndicButton));
        public static readonly DependencyProperty IndicatorWidthProperty = DependencyProperty.Register("IndicatorWidth", typeof(double), typeof(ParamerusIndicButton),new PropertyMetadata(5.0));
        public static readonly DependencyProperty IndicatorHeightProperty = DependencyProperty.Register("IndicatorHeight", typeof(double), typeof(ParamerusIndicButton), new PropertyMetadata(5.0));

        private static void OnStateIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ParamerusIndicButton sender = d as ParamerusIndicButton;
            sender.StateIndexChanged?.Invoke(sender, (int)e.OldValue, (int)e.NewValue);
        }


        
        public delegate void StateIndexChangedDelegate(object sender, int oldState, int newState);

        public event StateIndexChangedDelegate StateIndexChanged;
        public event RoutedEventHandler Click;

        public HorizontalAlignment HorizontalIndicatorAlignment
        {
            get
            {
                return (HorizontalAlignment)GetValue(HorizontalIndicatorAlignmentProperty);
            }
            set
            {
                SetValue(HorizontalIndicatorAlignmentProperty, value);
            }
        }
        public VerticalAlignment VerticalIndicatorAlignment
        {
            get
            {
                return (VerticalAlignment)GetValue(VerticalIndicatorAlignmentProperty);
            }
            set
            {
                SetValue(VerticalIndicatorAlignmentProperty, value);
            }
        }
        public Thickness MarginIndicator
        {
            get
            {
                return (Thickness)GetValue(MarginIndicatorProperty);
            }
            set
            {
                SetValue(MarginIndicatorProperty, value);
            }
        }
        public Thickness MarginContent
        {
            get
            {
                return (Thickness)GetValue(MarginContentProperty);
            }
            set
            {
                SetValue(MarginContentProperty, value);
            }
        }
        public double IndicatorWidth
        {
            get
            {
                return (double)GetValue(IndicatorWidthProperty);
            }
            set
            {
                SetValue(IndicatorWidthProperty, value);
            }
        }
        public double IndicatorHeight
        {
            get
            {
                return (double)GetValue(IndicatorHeightProperty);
            }
            set
            {
                SetValue(IndicatorHeightProperty, value);
            }
        }
        public int CurrentStateIndex
        {
            get
            {
                return (int)GetValue(CurrentStateIndexProperty);
            }
            set
            {
                SetValue(CurrentStateIndexProperty, value);
                OnPropertyChanged();
            }
        }
        public List<String> StatesButton
        {
            get
            {
                return (List<String>)GetValue(StatesButtonProperty);
            }
            set
            {
                SetValue(StatesButtonProperty, value);
            }
        }
        public Brush IndicatorBackground
        {
            get
            {
                return (Brush)GetValue(IndicatorBackgroundProperty);
            }

            set
            {
                SetValue(IndicatorBackgroundProperty, value);
            }
        }


        public ParamerusIndicButton()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] String name="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            CurrentStateIndex = (CurrentStateIndex < (StatesButton.Count - 1)) ? CurrentStateIndex + 1 : 0;
            Click?.Invoke(this, null);

        }
    }
}
