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
        public String NameValue { get; set; }
        public double Value { get; set; }
        public String UnitsName { get; set; }
        public ParamerusChartSpinEdit()
        {
            InitializeComponent();
        }
    }
}
