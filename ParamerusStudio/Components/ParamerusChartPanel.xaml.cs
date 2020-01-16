using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
using InteractiveDataDisplay.WPF;
using ParamerusStudio.PMBus;

namespace ParamerusStudio
{
    public class VisibilityToCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Visibility)value) == Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
    /// <summary>
    /// Логика взаимодействия для ParamerusChartPanel.xaml
    /// </summary>
    public partial class ParamerusChartPanel : UserControl, INotifyPropertyChanged
    {
        

        public static readonly DependencyProperty VisibleLimitsPanelProperty = DependencyProperty.RegisterAttached("VisibleLimitsPanel",
                                                                                                                    typeof(bool?),
                                                                                                                    typeof(ParamerusChartPanel),
                                                                                                                    new FrameworkPropertyMetadata(
                                                                                                                        false,
                                                                                                                        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                                                                        (o,args) => ((ParamerusChartPanel)o).SetVisibleLimitsPanel((bool)args.NewValue)));
        public static readonly DependencyProperty LastValueProperty = DependencyProperty.Register("LastValue", typeof(double?), typeof(ParamerusChartPanel));
        
        public static readonly DependencyProperty DeviceProperty = DependencyProperty.Register("Device", typeof(ParamerusPMBusDevice), typeof(ParamerusChartPanel),
                                                                                                                    new FrameworkPropertyMetadata(
                                                                                                                        null,
                                                                                                                        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                                                                        (o, args) => ((ParamerusChartPanel)o).CurrentDeviceChanged?.Invoke((ParamerusPMBusDevice)args.NewValue)));
        public static readonly DependencyProperty TimerPeriodProperty = DependencyProperty.Register("TimerPeriod", typeof(int), typeof(ParamerusChartPanel));

        public static void VisibleLimitPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d == null)
                return;
            (d as ParamerusChartPanel).SetVisibleLimitsPanel((bool)args.NewValue);
        }


        public double? LastValue
        {
            get => (double?)GetValue(LastValueProperty);
            set
            {
                SetValue(LastValueProperty, value);
            }
        }
        public event Action<ParamerusPMBusDevice> CurrentDeviceChanged;

        public ParamerusPMBusDevice Device
        {
            get => Dispatcher.Invoke<ParamerusPMBusDevice>(() => (ParamerusPMBusDevice)GetValue(DeviceProperty));
            set
            {
                SetValue(DeviceProperty, value);
            }
        }

        public int TimerPeriod
        {
            get => Dispatcher.Invoke<int>(()=>(int)GetValue(TimerPeriodProperty));
            set
            {
                SetValue(TimerPeriodProperty, value);
            }
        }
        public String NamePanel { get; set; }
        private Timer timer { get; set; }
        private bool? _visible_panel;
        private LineGraph plot;
        public bool? VisiblePanel
        {
            get { return _visible_panel; }
            set
            {
                if (value == false)
                    this.Visibility = Visibility.Collapsed;
                else
                    this.Visibility = Visibility.Visible;
                _visible_panel = value;
                
                OnPropertyChanged();
            }
        }

        public void SetVisibleLimitsPanel(bool newValue)
        {
            if (newValue == false)
                LimitsPanel.Visibility = Visibility.Collapsed;
            else
                LimitsPanel.Visibility = Visibility.Visible;
            SetValue(VisibleLimitsPanelProperty, newValue);
            OnPropertyChanged("VisibleLimitsPanel");
        }
        public bool? VisibleLimitsPanel
        {
            get => (bool?)this.GetValue(VisibleLimitsPanelProperty);
            
            set
            {
                this.SetValue(VisibleLimitsPanelProperty, value);
                OnPropertyChanged();
            }
        }

        
        public ParamerusChartPanel()
        {
            InitializeComponent();
            VisiblePanel = true;
            SetVisibleLimitsPanel(false);
            HeaderCloseButton.Click += HeaderCloseButton_Click;
            CurrentDeviceChanged += ParamerusChartPanel_CurrentDeviceChanged;
        }
        List<long> x;
        List<double> y;
        int i;
        private void ParamerusChartPanel_CurrentDeviceChanged(ParamerusPMBusDevice obj)
        {
            plot = new LineGraph();
            x = new List<long>();
            y = new List<double>();
            plot.Plot(x, y);
            
            lines.Children.Add(plot);
            timer = new Timer((e) => Dispatcher.Invoke(TimerCb), null, 0, TimerPeriod);
        }
        
        private void TimerCb()
        {
            if (LastValue != null && Visibility == Visibility.Visible)
            {
                if (x.Count == 0)
                    x.Add(0);
                else
                    x.Add(x.Last() + TimerPeriod);
                y.Add((double)LastValue);
                plot.Plot(x, y);
            }
        }

        private void TimerTick(object state)
        {
            
        }

        private void HeaderCloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.VisiblePanel = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]String name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

}
