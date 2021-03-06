﻿using DevExpress.Xpf.Core;
using ParamerusStudio.Components;
using ParamerusStudio.PMBus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using TIDP.SAA;

namespace ParamerusStudio
{
    #region Converters

    public class AlignmentPlotsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;
            bool check = (bool)value;
            if (targetType == typeof(HorizontalAlignment))
            {
                if (check)
                    return HorizontalAlignment.Stretch;
                else
                    return HorizontalAlignment.Left;
            }
            else if (targetType == typeof(VerticalAlignment))
            {
                if (check)
                    return VerticalAlignment.Stretch;
                else
                    return VerticalAlignment.Top;
            }
            else
                return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
    /// <summary>
    /// Конвертер значений, преобразовывающий регистр статуса в коллекцию бит
    /// </summary>
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
            return (object[])Binding.DoNothing;
        }
    }
    /// <summary>
    /// Конвертер значений, преобразовывающий состояние контрольной линии, в состояние индикаторной кнопки
    /// </summary>
    public class StatusControlLineToStateIndicButtonConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Contains(null) || values.Contains(DependencyProperty.UnsetValue))
                return 0;
            if (!(values[0] is ParamerusPMBusDevice) || !(values[1] is LogicLevelResult))
                return 0;

            ParamerusPMBusDevice dev = values[0] as ParamerusPMBusDevice;
            LogicLevelResult stat_control_line = dev.ControlLine;
            if (!stat_control_line.Success)
                return 0;
            if (stat_control_line.Level == LogicLevel.Low)
                return 0;
            else
                return 1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return (object[])Binding.DoNothing;
        }
    }
    /// <summary>
    /// Конвертер значений, преобразовывающий состояние статусного регистра в цвет индикатора на кнопке
    /// </summary>
    public class StatusRegsToIndicatorButtonBacgroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Contains(null) || values.Contains(DependencyProperty.UnsetValue))
                return Brushes.Red;

            LogicLevelResult res = values[1] as LogicLevelResult;
            if (!res.Success || res.Level == LogicLevel.Low)
                return Brushes.Red;

            bool isWarningBitSet = false;
            for (int i = 2; i < values.Length; i++)
            {
                BitStatus bs = (BitStatus)values[i];
                switch (bs)
                {
                    case BitStatus.Fault:
                        return Brushes.Red;
                    case BitStatus.Warning:
                        isWarningBitSet = true;
                        break;
                    default:
                        break;
                }
            }
            if (isWarningBitSet)
                return Brushes.Orange;
            return Brushes.Green;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return (object[])Binding.DoNothing;
        }
    }
    /// <summary>
    /// Конвертер значений, преобразовывающий считанное значение в отображение элемента
    /// </summary>
    public class ReadValueToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
    /// <summary>
    /// Конвертер значений, преобразовывающий количество отображаемых графиков в количество строк контейнера
    /// </summary>
    public class RowCountPanelChartsConverter : IMultiValueConverter
    {
        private bool IsVisiblePanel(object visibilityPanel)
        {
            if (!(visibilityPanel is Visibility))
                return false;
            return ((Visibility)visibilityPanel) == Visibility.Visible;
        }
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int count_visible = values.Count(IsVisiblePanel);
            if (count_visible <= 4)
                return count_visible;
            if (count_visible <= 6)
                return 3;
            return 4;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return (object[])Binding.DoNothing;
        }
    }
    /// <summary>
    /// Конвертер значений, преобразовывающий количество отображаемых графиков в количество столбцов
    /// </summary>
    public class ColumnCountPanelChartsConverter : IMultiValueConverter
    {
        private bool IsVisiblePanel(object visibilityPanel)
        {
            if (!(visibilityPanel is Visibility))
                return false;
            return ((Visibility)visibilityPanel) == Visibility.Visible;
        }
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int count_visible = values.Count(IsVisiblePanel);
            return (count_visible <= 4) ? 1 : 2;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return (object[])Binding.DoNothing;
        }
    }

    /// <summary>
    /// Конвертер значениий, преобразовывающий ширину заданную пользователем в ширину графика
    /// </summary>
    public class SizePlotsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (((bool)values[0]) == false)
                return Double.NaN;
            else
                return Double.Parse(values[1].ToString());
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return (object[])Binding.DoNothing;
        }
    }

    /// <summary>
    /// Конвертер значениий, преобразовывающий высоту заданную пользователем в ширину графика
    /// </summary>
    public class HeightPlotsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return (object[])Binding.DoNothing;
        }
    }

    #endregion

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : DXWindow, INotifyPropertyChanged
    {

        /// <summary>
        /// Найденный SMBus адаптер
        /// </summary>
        private SMBusAdapter _smBusAdapter;
        private List<ParamerusPMBusDevice> _pm_busDevices = new List<ParamerusPMBusDevice>();
        private ParamerusPMBusDevice _сurrentPmBusDevice;
        /// <summary>
        /// Найденные PMBus устройства
        /// </summary>
        public List<ParamerusPMBusDevice> PMBusDevices
        {
            get => _pm_busDevices;
            set
            {
                _pm_busDevices = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Выбранное PMBus устройство, при изменении выбранного устройства, производит считывание всех параметров устройства
        /// </summary>
        public ParamerusPMBusDevice CurrentPmBusDevice
        {
            get => _сurrentPmBusDevice;
            set
            {
                _сurrentPmBusDevice = value;
                _сurrentPmBusDevice.StartListener();
                OnPropertyChanged();
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            DXGridDataController.DisableThreadingProblemsDetection = true;
        }


        /// <summary>
        /// Реализация возможно перемещение окна приложения, при зажатии кнопки на логотипе
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();

        }

        /// <summary>
        /// Событие выбора ячейки в таблицах статусов регистров, запрещает выделение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (sender == null || !(sender is DataGrid))
                return;
            DataGrid dataGrid = sender as DataGrid;
            dataGrid.SelectedItem = null;
        }

        /// <summary>
        /// Запуск потока поиска подключенных устройств
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DXWindow_Loaded(object sender, RoutedEventArgs e) => Task.Factory.StartNew(PMBusDevicesDiscover);


        /// <summary>
        /// Изменение текста в статус баре внизу окна
        /// </summary>
        /// <param name="newStatus"></param>
        void SetStatus(String newStatus)
        {
            StatusBar.Dispatcher.Invoke(() => StatusBar.Content = newStatus);
        }

        /// <summary>
        /// Поиск подключенных PMBus устройств
        /// </summary>
        void PMBusDevicesDiscover()
        {

            SetStatus("Searching SMBUS-Adapter...");
            _smBusAdapter = ParamerusPMBusDevice.SMBusAdaptherSearch();
            if (_smBusAdapter == null)
            {
                SetStatus("SMBUS-Adapter not found.");
                return;
            }
            SetStatus("SMBus-Adapter found! Scanning PMBus-devices...");
            PMBusDevices = ParamerusPMBusDevice.PMBusInit();
            if (PMBusDevices == null || PMBusDevices.Count == 0)
            {
                SetStatus("PMBus-devices not found!");
                return;
            }
            cbDeviceList.Dispatcher.Invoke(() =>
            {
                cbDeviceList.SelectedIndex = 0;
                CurrentPmBusDevice = PMBusDevices[0];
            });

            SetStatus("PMBus-devices found!");
        }

        /// <summary>
        /// Событие изменения выбранного устройства
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbDeviceList_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (PMBusDevices == null)
                return;
            if (cbDeviceList.SelectedIndex == -1)
                cbDeviceList.SelectedIndex = 0;
            CurrentPmBusDevice?.StopListener();
            CurrentPmBusDevice = PMBusDevices[cbDeviceList.SelectedIndex];
        }
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] String name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        /// <summary>
        /// Событие изменения состояния индикаторной кнопки
        /// </summary>
        /// <param name="sender">Кнопка, вызвавшая событие</param>
        /// <param name="oldState">Старое состояние кнопки</param>
        /// <param name="newState">Новое состояние кнопки</param>
        private void ParamerusIndicButton_StateIndexChanged(object sender, int oldState, int newState)
        {

            if (newState == 1)
            {
                if (PMBusDevices == null || CurrentPmBusDevice == null)
                {
                    ParamerusIndicButton pib = sender as ParamerusIndicButton;
                    pib.CurrentStateIndex = 0;
                    return;
                }
                if (CurrentPmBusDevice.ControlLineSetHigh())
                {
                    SetStatus("CONTROL LINE is High");
                }
                else
                {
                    ParamerusIndicButton pib = sender as ParamerusIndicButton;
                    pib.CurrentStateIndex = 0;
                    SetStatus("Error set CONTROL LINE to High");
                }
            }
            else
            {
                if (PMBusDevices == null || CurrentPmBusDevice == null)
                    return;
                if (CurrentPmBusDevice.ControlLineSetLow())
                {
                    SetStatus("CONTROL LINE is Low");
                }
                else
                {
                    SetStatus("Error set CONTROL LINE to Low");
                }

            }
        }

        private void VinChartPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender == null || !(sender is ParamerusChartPanel))
                return;

            ParamerusChartPanel panel = sender as ParamerusChartPanel;
            panel.Focus();
        }
    }
}

