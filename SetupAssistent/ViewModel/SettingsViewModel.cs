using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SetupAssistent.Model;

namespace SetupAssistent.ViewModel
{
    class SettingsViewModel
    {
        public Settings settings { get; set; }
        public MyICommand SaveCommand { get; set; }

        #region Default Constructor
        public SettingsViewModel()
        {
            SaveCommand = new MyICommand(onSaveCommand, canSaveCommand);
            settings = AllSettings.settings[0];
        }
        #endregion

        #region Commands Code
        public void onSaveCommand(object parameter)
        {
            MessageBox.Show("Testing Save Command");
        }
        public bool canSaveCommand()
        {
            return true;
        }
        #endregion
    }
}
