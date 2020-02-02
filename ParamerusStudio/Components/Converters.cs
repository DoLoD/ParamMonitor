using ParamerusStudio.PMBus;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TIDP.SAA;

namespace ParamerusStudio.Components
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
    /// Конвертер значений, возвращающий текст текущего состояния кнопки из коллекции состояний и индекса нового состояния.
    /// </summary>
    public class StatesButtonConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values[0] == null || values[1] == null)
                throw new ArgumentNullException(nameof(values), "Argument null");
            if (values[0] is int && values[1] is List<String>)
            {
                int ind = (int)values[0];
                List<String> states = values[1] as List<String>;
                if (states.Count <= ind || (ind < -1))
                    throw new ArgumentOutOfRangeException(nameof(values), "Argument out of boundary states indicator button.");
                else if (ind == -1)
                    return "";
                else
                    return states[ind];
            }
            throw new ArgumentException("Argument type error.", nameof(values));
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return (object[])Binding.DoNothing;
        }
    }

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
            switch (bs)
            {
                case BitStatus.Fault:
                    return new SolidColorBrush(Color.FromArgb(0xFF, 0xFE, 0x00, 0x00));
                case BitStatus.Warning:
                    return new SolidColorBrush(Color.FromArgb(0xFF, 0xFE, 0xA4, 0x00));
                case BitStatus.BitNotImplemented:
                    return new SolidColorBrush(Color.FromArgb(0xFF, 0xF4, 0xF4, 0xF4));
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
                case BitStatus.BitNotImplemented:
                    return new SolidColorBrush(Color.FromArgb(0xFF, 0x83, 0x83, 0x83));
                default:
                    return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

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

    public class LimitItemSourceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Contains(null) || values.Contains(Binding.DoNothing) || !(values[1] is ParamerusPMBusDevice))
                return Binding.DoNothing;
            ParamerusPMBusDevice dev = values[1] as ParamerusPMBusDevice;
            switch(System.Convert.ToInt32(parameter))
            {
                case 0:
                    return dev.VIN_Limits;
                case 1:
                    return dev.VOUT_Limits;
                case 2:
                    return dev.IOUT_Limits;
                case 3:
                    return dev.POUT_Limits;
                case 4:
                    return dev.TEMP_Limits;
                default:
                    return Binding.DoNothing;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return (object[])Binding.DoNothing;
        }
    }
}
