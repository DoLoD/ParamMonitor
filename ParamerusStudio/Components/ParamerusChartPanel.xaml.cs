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

        public static readonly DependencyProperty DeviceProperty = DependencyProperty.Register("Device", typeof(ParamerusPMBusDevice), typeof(ParamerusChartPanel));
        public static readonly DependencyProperty TimerPeriodProperty = DependencyProperty.Register("TimerPeriod", typeof(int), typeof(ParamerusChartPanel));
        public static void VisibleLimitPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d == null)
                return;
            (d as ParamerusChartPanel).SetVisibleLimitsPanel((bool)args.NewValue);
        }

        public ParamerusPMBusDevice Device
        {
            get => (ParamerusPMBusDevice)GetValue(DeviceProperty);
            set
            {
                SetValue(DeviceProperty, value);
            }
        }

        public int TimerPeriod
        {
            get => (int)GetValue(TimerPeriodProperty);
            set
            {
                SetValue(TimerPeriodProperty, value);
            }
        }
        public String NamePanel { get; set; }
        private Timer timer { get; set; }
        private bool? _visible_panel;
        private bool? _visible_limits_panel;
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
            _visible_limits_panel = newValue;
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

            //double[] x = new double[200];
            //for (int i = 0; i < x.Length; i++)
            //    x[i] = 3.1415 * i / (x.Length - 1);

            
            
            //for (int i = 0; i < 8; i++)
            //{
            //    var lg = new LineGraph();
            //    var lg_legend = new LineGraph();

            //    lg_legend.SetBinding(LineGraph.VisibilityProperty, new Binding() {Path=new PropertyPath("Visibility"),  Source=lg, Mode=BindingMode.TwoWay});
            //    lg_legend.Description = String.Format(CultureInfo.CurrentCulture, "Data series {0}", i + 1);
            //    lg_legend.StrokeThickness = 2;
            //    lg_legend.Stroke = new SolidColorBrush(Color.FromArgb(255, (byte)((i * 20) + 90), 0, 0));
            //    lg.StrokeThickness = 2;
            //    lg.Stroke = new SolidColorBrush(Color.FromArgb(255, (byte)((i * 20) + 90), 0, 0));
            //    lg.Plot(x, x.Select(v => Math.Sin(v + i / 10.0)).ToArray());
            //    lines.Children.Add(lg);
            //    lines_legend.Items.Add(lg_legend);
            //}

            
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
