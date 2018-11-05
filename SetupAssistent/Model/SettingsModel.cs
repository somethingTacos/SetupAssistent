using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SetupAssistent.Model
{
    public class SettingsModel { }

    public class Settings : INotifyPropertyChanged
    {
        private bool _darkTheme;
        public bool DarkTheme
        {
            get { return _darkTheme; }
            set
            {
                _darkTheme = value;
                RaisePropertyChanged("DarkTheme");
                SettingsChanged = true;
            }
        }

        private bool _limitModuleNameSize;
        public bool LimitModuleNameSize
        {
            get { return _limitModuleNameSize; }
            set
            {
                _limitModuleNameSize = value;
                RaisePropertyChanged("LimitModuleNameSize");
                SettingsChanged = true;
            }
        }

        private string _outputFilePath;
        public string OutputFilePath
        {
            get { return _outputFilePath; }
            set
            {
                if (_outputFilePath != null)
                {

                    if (value.ToLower() != _outputFilePath.ToLower())
                    {
                        _outputFilePath = value;
                        RaisePropertyChanged("outputFilePath");
                        SettingsChanged = true;
                    }
                }
                else
                {
                    _outputFilePath = value;
                    RaisePropertyChanged("outputFilePath");
                    SettingsChanged = true;
                }
            }
        }

        private bool _settingsChanged;
        public bool SettingsChanged
        {
            get { return _settingsChanged; }
            set
            {
                _settingsChanged = value;
                RaisePropertyChanged("SettingsChanged");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }

    public static class AllSettings
    {
        public static bool LoadedAtStartup = false;
        public static ObservableCollection<Settings> settings = new ObservableCollection<Settings>();
        public static string SettingsFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SetupAssistant";
        public static string SettingsFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SetupAssistant\\Settings.xml";

        private static string oldfilepath;

        public static string OldSettingsFilePath
        {
            get { return oldfilepath; }
            set { oldfilepath = value; }
        }
    }
}
