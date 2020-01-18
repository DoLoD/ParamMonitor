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
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
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

        public static readonly DependencyProperty VisibleValuesSeriesLabelProperty = DependencyProperty.Register("VisibleValuesSeriesLabel", typeof(bool), typeof(ParamerusChartPanel), new PropertyMetadata(true));
        public static readonly DependencyProperty VisibleLimitsPanelProperty = DependencyProperty.RegisterAttached("VisibleLimitsPanel",
                                                                                                                    typeof(bool?),
                                                                                                                    typeof(ParamerusChartPanel),
                                                                                                                    new FrameworkPropertyMetadata(
                                                                                                                        false,
                                                                                                                        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                                                                        (o,args) => ((ParamerusChartPanel)o).SetVisibleLimitsPanel((bool)args.NewValue)));
        public static readonly DependencyProperty LastValueProperty = DependencyProperty.Register("LastValue", typeof(double?), typeof(ParamerusChartPanel), new PropertyMetadata(null));
        
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

        public bool VisibleValuesSeriesLabel
        {
            get => (bool)GetValue(VisibleValuesSeriesLabelProperty);
            set
            {
                SetValue(VisibleValuesSeriesLabelProperty, value);
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

        private List<double> _values = new List<double>();
        private List<long> _times = new List<long>();

        public List<double> Values
        {
            get => _values;
            set
            {
                _values = value;
                OnPropertyChanged();
            }
        }
        public List<long> Times
        {
            get => _times;
            set
            {
                _times = value;
                OnPropertyChanged();
            }
        }

        private PlotModel _model = new PlotModel();
        public PlotModel MyModel
        {
            get => _model;
            set
            {
                _model = value;
                OnPropertyChanged();
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
            MyModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = 0.0,
                Maximum = 0.5,
            });
            MyModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = 0.0,
                Maximum = 600000.0,
                LabelFormatter=TimeLabelFormatter
                
            });
            LineSeries ser = new LineSeries() { LineStyle = LineStyle.Solid , Color = OxyColor.FromArgb(0xFF,0xC2,0x00,0x00), StrokeThickness=2};
            LineSeries ser_lab = new LineSeries() { LineStyle = LineStyle.Solid, Color = OxyColor.FromArgb(0xFF, 0xC2, 0x00, 0x00), StrokeThickness = 1, LabelMargin = 0, LabelFormatString = "{1:F3}" };
            MyModel.Series.Add(ser);
            MyModel.Series.Add(ser_lab);
        }
        
        private String TimeLabelFormatter(double val)
        {
            TimeSpan time = TimeSpan.FromMilliseconds(val);
            if (time.Hours != 0)
            {
                return time.ToString("hh\\.mm\\.ss");
            }
            else if (time.Minutes != 0)
            {
                return time.ToString("mm\\.ss");
            }
            else if (time.Seconds != 0)
            {
                return time.ToString("ss");
            }
            else if(time.Ticks == 0)
            {
                return "0";
            }
            else
            {
                return time.ToString("fff");
            }
                
        }
        private void ParamerusChartPanel_CurrentDeviceChanged(ParamerusPMBusDevice obj)
        {
            ((LineSeries)MyModel.Series[0]).Points.Clear();
            timer = new Timer((e) => Dispatcher.Invoke(Update), null, 0, TimerPeriod);
        }
        private void Update()
        {
            if (Device != null && LastValue != null && Visibility == Visibility.Visible)
            {
                var series = (LineSeries)MyModel.Series[0];
                var ser_labs = (LineSeries)MyModel.Series[1];
                double val = (double)LastValue;
                double time = series.Points.Count == 0 ? 0 : series.Points.Last().X + TimerPeriod;


                if (MyModel.Axes[0].Maximum < val + val * 0.1)
                    MyModel.Axes[0].Maximum = val + val * 0.1;
                if (MyModel.Axes[1].Maximum < time + time * 0.15)
                    MyModel.Axes[1].Maximum = time + time * 0.15;

                
                series.Points.Add(new DataPoint(time, val));
                ser_labs.Points.Clear();
                if (VisibleValuesSeriesLabel)
                {
                    ser_labs.Points.Add(new DataPoint(time, val));
                }

                

                if (series.Points.Count > 500)
                    series.Points.RemoveAt(0);
                MyModel.InvalidatePlot(true);
            }
            if(LastValue == null && timer != null)
            {
                timer.Change(Timeout.Infinite, 0);
                timer.Dispose();
                timer = null;
                VisiblePanel = false;
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
