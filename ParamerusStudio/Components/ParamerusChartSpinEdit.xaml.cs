using System;
using System.Collections.Generic;
using System.Linq;
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

namespace ParamerusStudio
{
    /// <summary>
    /// Логика взаимодействия для ParamerusChartSpinEdit.xaml
    /// </summary>
    public partial class ParamerusChartSpinEdit : UserControl
    {
        public static readonly DependencyProperty NameValueProperty = DependencyProperty.Register(nameof(NameValue), typeof(String), typeof(ParamerusChartSpinEdit));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(ParamerusChartSpinEdit));

        public String NameValue 
        {
            get => (String)GetValue(NameValueProperty);
            set
            {
                SetValue(NameValueProperty, value);
            }
        }
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set
            {
                SetValue(ValueProperty, value);
            }
        }
        public String UnitsName { get; set; }
        public ParamerusChartSpinEdit()
        {
            InitializeComponent();
        }
    }
}
