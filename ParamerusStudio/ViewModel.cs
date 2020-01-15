using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TIDP.SAA;

namespace ParamerusStudio
{
    public class ViewModel : DependencyObject, INotifyPropertyChanged
    {
        private LogicLevelResult _control_line;
        public LogicLevelResult Control_Line
        {
            get => _control_line;
            set
            {
                _control_line = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] String name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
