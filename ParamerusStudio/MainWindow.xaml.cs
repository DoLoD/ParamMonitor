using DevExpress.Xpf.Accordion;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.WindowsUI;
using ParamerusStudio.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
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
using TIDP.PMBus;
using TIDP.SAA;

namespace ParamerusStudio
{

    public class CenterBorderGapMaskConverter : IMultiValueConverter
    {
        // Methods
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            
            Type type = typeof(double);
            if (values == null
                || values.Length != 3
                || values[0] == null
                || values[1] == null
                || values[2] == null
                || !type.IsAssignableFrom(values[0].GetType())
                || !type.IsAssignableFrom(values[1].GetType())
                || !type.IsAssignableFrom(values[2].GetType()))
            {
                return DependencyProperty.UnsetValue;
            }

            double pixels = (double)values[0];
            double width = (double)values[1];
            double height = (double)values[2];
            if ((width == 0.0) || (height == 0.0))
            {
                return null;
            }
            Grid visual = new Grid();
            visual.Width = width;
            visual.Height = height;
            ColumnDefinition colDefinition1 = new ColumnDefinition();
            ColumnDefinition colDefinition2 = new ColumnDefinition();
            ColumnDefinition colDefinition3 = new ColumnDefinition();
            colDefinition1.Width = new GridLength(1.0, GridUnitType.Star);
            colDefinition2.Width = new GridLength(pixels);
            colDefinition3.Width = new GridLength(1.0, GridUnitType.Star);
            visual.ColumnDefinitions.Add(colDefinition1);
            visual.ColumnDefinitions.Add(colDefinition2);
            visual.ColumnDefinitions.Add(colDefinition3);
            RowDefinition rowDefinition1 = new RowDefinition();
            RowDefinition rowDefinition2 = new RowDefinition();
            rowDefinition1.Height = new GridLength(height / 2.0);
            rowDefinition2.Height = new GridLength(1.0, GridUnitType.Star);
            visual.RowDefinitions.Add(rowDefinition1);
            visual.RowDefinitions.Add(rowDefinition2);
            Rectangle rectangle1 = new Rectangle();
            Rectangle rectangle2 = new Rectangle();
            Rectangle rectangle3 = new Rectangle();
            rectangle1.Fill = Brushes.Black;
            rectangle2.Fill = Brushes.Black;
            rectangle3.Fill = Brushes.Black;
            Grid.SetRowSpan(rectangle1, 2);
            Grid.SetRow(rectangle1, 0);
            Grid.SetColumn(rectangle1, 0);
            Grid.SetRow(rectangle2, 1);
            Grid.SetColumn(rectangle2, 1);
            Grid.SetRowSpan(rectangle3, 2);
            Grid.SetRow(rectangle3, 0);
            Grid.SetColumn(rectangle3, 2);
            visual.Children.Add(rectangle1);
            visual.Children.Add(rectangle2);
            visual.Children.Add(rectangle3);
            return new VisualBrush(visual);
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { Binding.DoNothing };
        }
    }


    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : DXWindow
    {
        ParamerusRegisterStatus Register_STATUS_VOUT;
        SMBusAdapter _smBusAdapter;
        List<PMBusDevice> _devices;

        public MainWindow()
        {
            InitializeComponent();
            Register_STATUS_VOUT = (ParamerusRegisterStatus)FindResource("Register_STATUS_VOUT");

        }

        private void NavigMenu_SelectedItemChanged(object sender, AccordionSelectedItemChangedEventArgs e)
        {
            AccordionItem item = e.NewItem as AccordionItem;
            if ((String)item.Header == "Система")
                TabControlNav.SelectedIndex = 0;
            if ((String)item.Header == "Конфигурация")
                TabControlNav.SelectedIndex = 1;
            if ((String)item.Header == "Панель управления")
                TabControlNav.SelectedIndex = 2;
            if ((String)item.Header == "Мониторинг")
                TabControlNav.SelectedIndex = 3;

        }

        private void NavigBtn_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (sender == null || !(sender is DataGrid))
                return;
            DataGrid dataGrid = sender as DataGrid;
            dataGrid.SelectedItem = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Register_STATUS_VOUT.RegisterBits[3].CurrentStatusBit = BitStatus.Fault;
        }

        private async void DXWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Factory.StartNew(PMBusDevicesDiscover).ConfigureAwait(false);
           
        }

        private bool PMBusInit()
        {
            if (SMBusAdapter.Discover() == 0)
                return false;
            _smBusAdapter = SMBusAdapter.Adapter;
            //LbSMBusVer.Text = _smBusAdapter.Version.ToString();
            //LbSMBusAdapt.Text = _smBusAdapter.ToString().Substring(0, _smBusAdapter.ToString().IndexOf("pid_") + 9) + "]";
            var opts = new PMBusDevice.DiscoverOptions();
            opts.Scan_Mode = PMBusDevice.ScanMode.TPS53951;
            if (PMBusDevice.Discover(opts) == 0)
                return false;
            _devices = PMBusDevice.Devices;
            foreach (PMBusDevice dev in PMBusDevice.Devices)
            {
                //CbListPMBusDevice.Items.Add(dev.ToString());
            }

            //CbListPMBusDevice.SelectedIndex = 0;
            return true;
        }

        void PMBusDevicesDiscover()
        {
            StatusBar.Content = "Start finding SMBUS-Adapter";
            
            if(!PMBusInit())
            {

            }
        }
    }
}
