using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using PropertyChanged;

namespace SetupAssistent.Model
{
    public class SettingsModel { }

    [AddINotifyPropertyChangedInterface]
    public class Settings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public Color Theme_MainColor { get; set; }
        public Color Theme_AccentColor { get; set; }

    }

}
