using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        //I'll make this later, setting up custom colors will be too much work.
        public bool DarkTheme { get; set; } = false;
        public bool LimitModuleNameSize { get; set; } = true;
        public string outputFilePath { get; set; } = "";
    }

    public static class AllSettings
    {
        public static bool LoadedAtStartup = false;
        public static ObservableCollection<Settings> settings = new ObservableCollection<Settings>();
    }
}
