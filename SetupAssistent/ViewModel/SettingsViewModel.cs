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
            InitSettings();
        }
        #endregion

        #region Commands Code
        public void onSaveCommand(object parameter)
        {
            /* -Set the new save location to AllSettings
             * -Save AllSettings to xml file in appdata
             * -Copy modules and tasks xmls if they exist to new location
             * -Confirm move succeeded, then remove old files (the old directory can be removed if it is emtpy after the old xml files are removed)
             * 
             * In the future, I'll need to move packed files if any exist.
             */



            MessageBox.Show(String.Format("S output: {1}{0}AS Ouput: {2}", Environment.NewLine, settings.outputFilePath, AllSettings.settings[0].outputFilePath));
        }
        public bool canSaveCommand()
        {
            return true;
        }
        #endregion

        #region Other Methods
        public void InitSettings()
        {
            Settings tempSettings = new Settings();
            tempSettings.outputFilePath = AllSettings.settings[0].outputFilePath.ToString();

            settings = tempSettings;
        }
        #endregion
    }
}
