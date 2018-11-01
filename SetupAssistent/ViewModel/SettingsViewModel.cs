using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using SetupAssistent.Model;

namespace SetupAssistent.ViewModel
{
    class SettingsViewModel
    {
        #region Public Properties
        public Settings settings { get; set; }
        public MyICommand SaveCommand { get; set; }
        #endregion

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
             * -Confirm copy succeeded, then remove old files (the old directory can be removed if it is emtpy after the old xml files are removed)
             * 
             * In the future, I'll need to move packed files if any exist.
             */
            
            //Code for if Outputpath was changed -- Start
            void CheckAndMoveFile(string FileToMove, string MoveToHere)
            {
                if (File.Exists(FileToMove))
                {
                    try
                    {
                        File.Copy(FileToMove, MoveToHere);
                        File.Delete(FileToMove);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }

            AllSettings.settings[0] = settings;

            XmlSerializer xmlS = new XmlSerializer(typeof(ObservableCollection<Settings>));

            using (TextWriter writer = new StreamWriter(AllSettings.SettingsFile))
            {
                xmlS.Serialize(writer, AllSettings.settings);
            }

            string Old_ModulesFile = AllSettings.OldSettingsFilePath + "\\Modules.xml";
            string Old_TasksFile = AllSettings.OldSettingsFilePath + "\\Tasks.xml";

            CheckAndMoveFile(Old_ModulesFile, settings.OutputFilePath + "\\Modules.xml");
            CheckAndMoveFile(Old_TasksFile, settings.OutputFilePath + "\\Tasks.xml");

            //Packed files will have to be moved as well, whenever I decide to work on those. Probably not for awhile.

            AllSettings.OldSettingsFilePath = settings.OutputFilePath;
            //Code for if Outputpath was changed -- End



            settings.SettingsChanged = false;
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
            tempSettings.OutputFilePath = AllSettings.settings[0].OutputFilePath.ToString();

            settings = tempSettings;

            settings.SettingsChanged = false;
            AllSettings.OldSettingsFilePath = settings.OutputFilePath;
        }
        #endregion
    }
}
