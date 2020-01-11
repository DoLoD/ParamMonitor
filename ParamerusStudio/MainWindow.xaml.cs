using DevExpress.Xpf.Accordion;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.WindowsUI;
using ParamerusStudio.Components;
using ParamerusStudio.PMBus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using TIDP.PMBus;
using TIDP.PMBus.Commands;
using TIDP.SAA;
using TIDP;

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
    public class RegisterStatusTableConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == null || !(values[1] is ParamerusRegisterStatus))
                return null;
            ParamerusRegisterStatus reg = values[1] as ParamerusRegisterStatus;
            return reg.RegisterBits;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : DXWindow, INotifyPropertyChanged
    {
        
        SMBusAdapter _smBusAdapter;
        private List<ParamerusPMBusDevice> _pm_busDevices = new List<ParamerusPMBusDevice>();
        private ParamerusPMBusDevice _сurrentPmBusDevice;
        public List<ParamerusPMBusDevice> PMBusDevices
        {
            get => _pm_busDevices;
            set
            {
                _pm_busDevices = value;
                OnPropertyChanged();
            }
        }
        public ParamerusPMBusDevice CurrentPmBusDevice
        {
            get => _сurrentPmBusDevice;
            set
            {
                _сurrentPmBusDevice = value;
                CurrentPmBusDevice.Update();
                OnPropertyChanged();
            }
        }


        public MainWindow()
        {
            InitializeComponent();

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
            
        }

        private void DXWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(PMBusDevicesDiscover);

        }


        private bool SMBusAdaptherSearch()
        {
            if (SMBusAdapter.Discover() == 0)
                return false;
            _smBusAdapter = SMBusAdapter.Adapter;
            return true;
        }

        private bool PMBusSearch()
        {
            var opts = new PMBusDevice.DiscoverOptions();
            opts.Scan_Mode = PMBusDevice.ScanMode.DeviceID;
            if (PMBusDevice.Discover(opts) == 0)
            {
                opts.Scan_Mode = PMBusDevice.ScanMode.TPS53951;
                if (PMBusDevice.Discover(opts) == 0)
                    return false;
            }
            List<ParamerusPMBusDevice> listDevices = new List<ParamerusPMBusDevice>();
            foreach(var dev in PMBusDevice.Devices)
            {
                listDevices.Add(new ParamerusPMBusDevice(dev));
            }
            PMBusDevices = listDevices;
            cbDeviceList.Dispatcher.Invoke(() =>
            {
                cbDeviceList.SelectedIndex = 0;
            });
            CurrentPmBusDevice = PMBusDevices[0];
            return true;
        }

        // 0451/5f00
        private bool PMBusInit()
        {
            if (!PMBusSearch())
                return false;
            return true;
        }

        void SetStatus(String newStatus)
        {
            StatusBar.Dispatcher.Invoke(() => StatusBar.Content = newStatus);
        }

        ParamerusPMBusDevice GetSelectDevice()
        {
            if (PMBusDevices == null)
                return null;
            return PMBusDevices[cbDeviceList.SelectedIndex];
        }

        void PMBusDevicesDiscover()
        {

            SetStatus("Searching SMBUS-Adapter...");
            if (!SMBusAdaptherSearch())
            {
                SetStatus("SMBUS-Adapter not found.");
                return;
            }
            SetStatus("SMBus-Adapter found! Scanning PMBus-devices...");
            if (!PMBusInit())
            {
                SetStatus("PMBus-devices not found!");
                return;
            }
            SetStatus("PMBus-devices found!");
        }

        private void cbDeviceList_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (PMBusDevices == null)
                return;
            if (cbDeviceList.SelectedIndex == -1)
                cbDeviceList.SelectedIndex = 0;
            CurrentPmBusDevice = PMBusDevices[cbDeviceList.SelectedIndex];
        }
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] String name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        
        private void ParamerusIndicButton_Click(object sender, RoutedEventArgs e)
        {
            if (PMBusDevices == null)
                return;
            
        }

        void OutListParamDevs(PMBusDevice dev)
        {
            Debug.WriteLine("ID\tCode\tRail\tRead Only?\tStatus\tDecoded\tDecoded Formated\tEncoded");

            foreach (ParameterBase param in dev.Commands.Parameters)
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
    }
    }

