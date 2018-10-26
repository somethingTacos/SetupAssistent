using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SetupAssistent.Model;
using System.Windows;
using System.Xml.Serialization;
using System.IO;

namespace SetupAssistent.ViewModel
{
    public class ModuleViewModel
    {
        #region Public Propertys
        public ObservableCollection<Module> ModuleList { get; set; }
        public MyICommand LoadModuleCommand { get; set; }
        public MyICommand CreateNewModuleCommand { get; set; }
        public MyICommand RemoveModuleCommand { get; set; }
        public MyICommand EditModuleCommand { get; set; }
        public string workingModuleName { get; set; }
        public string userName = Environment.UserName.ToString();
        public string ModulesFile = string.Empty;
        public string TasksFile = string.Empty;
        public string SettingsFolder = string.Empty;

        private readonly NavigationViewModel _navigationViewModel;
        #endregion

        #region Default Constructor
        public ModuleViewModel(NavigationViewModel navigationViewModel)
        {
            _navigationViewModel = navigationViewModel;
            LoadModuleCommand = new MyICommand(onLoadModuleCommand, canLoadModuleCommand);
            CreateNewModuleCommand = new MyICommand(onCreateNewModuleCommand, canCreateNewModuleCommand);
            RemoveModuleCommand = new MyICommand(onRemoveModuleCommand, canRemoveModuleCommand);
            EditModuleCommand = new MyICommand(onEditModuleCommand, canEditModuleCommand);
            //These paths are just for testing. I'll have these be settable later in a settings view.
            SettingsFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SetupAssistant";
            LoadXMLData();
        }
        #endregion

        #region LoadData
        public void LoadXMLData()
        {
            string SettingsFile = SettingsFolder + "\\Settings.xml";
            ObservableCollection<Module> tempOC_Modules = new ObservableCollection<Module>();
            ObservableCollection<ModuleTasks> tempOC_Tasks = new ObservableCollection<ModuleTasks>();
            ObservableCollection<Settings> tempOC_Settings = new ObservableCollection<Settings>();

            if (!Directory.Exists(SettingsFolder))
            {
                Directory.CreateDirectory(SettingsFolder);
            }

            if (File.Exists(SettingsFile))
            {
                XmlSerializer xmlS = new XmlSerializer(typeof(ObservableCollection<Settings>));

                using (TextReader reader = new StreamReader(SettingsFile))
                {
                    tempOC_Settings = (ObservableCollection<Settings>)xmlS.Deserialize(reader);
                }
            }
            else
            {
                Settings defaultSettings = new Settings();
                defaultSettings.outputFilePath = SettingsFolder;
                tempOC_Settings.Add(defaultSettings);
                ModulesFile = SettingsFolder.ToString() + "\\Modules.xml";
                TasksFile = SettingsFolder.ToString() + "\\Tasks.xml";

                XmlSerializer xmlS = new XmlSerializer(typeof(ObservableCollection<Settings>));

                using (TextWriter writer = new StreamWriter(SettingsFile))
                {
                    xmlS.Serialize(writer, tempOC_Settings);
                }

                using (TextReader reader = new StreamReader(SettingsFile))
                {
                    tempOC_Settings = (ObservableCollection<Settings>)xmlS.Deserialize(reader);
                }
            }


            if (tempOC_Settings[0].outputFilePath.ToString() != "")
            {
                ModulesFile = tempOC_Settings[0].outputFilePath.ToString() + "\\Modules.xml";
                TasksFile = tempOC_Settings[0].outputFilePath.ToString() + "\\Tasks.xml";
            }
        

            if (File.Exists(ModulesFile))
            {
                XmlSerializer xmlS = new XmlSerializer(typeof(ObservableCollection<Module>));

                using (TextReader reader = new StreamReader(ModulesFile))
                {
                    tempOC_Modules = (ObservableCollection<Module>)xmlS.Deserialize(reader);
                }
            }

            if(File.Exists(TasksFile))
            {
                XmlSerializer xmlS = new XmlSerializer(typeof(ObservableCollection<ModuleTasks>));

                using (TextReader reader = new StreamReader(TasksFile))
                {
                    tempOC_Tasks = (ObservableCollection<ModuleTasks>)xmlS.Deserialize(reader);
                }
            }

            foreach (Module tempModule in tempOC_Modules)
            {
                if (tempModule.Description.Length > 100)
                {
                    tempModule.DescriptionPreview = tempModule.Description.Substring(0, 100) + "...";
                }
                else
                {
                    tempModule.DescriptionPreview = tempModule.Description;
                }
            }

            ModuleList = tempOC_Modules;

            if (!AllModules.LoadedAtStartup)
            {
                foreach (Module module in ModuleList)
                {
                    AllModules.modulesList.Add(module);
                }
                AllModules.LoadedAtStartup = true;
            }

            if(!AllTasks.LoadedAtStartup)
            {
                foreach(ModuleTasks tasks in tempOC_Tasks)
                {
                    AllTasks.tasksList.Add(tasks);
                }
                AllTasks.LoadedAtStartup = true;
            }

            if (!AllSettings.LoadedAtStartup)
            {
                foreach (Settings setting in tempOC_Settings)
                {
                    AllSettings.settings.Add(setting);
                }
                AllSettings.LoadedAtStartup = true;
            }
        }
        #endregion

        #region Commands Code

        public void onCreateNewModuleCommand(object parameter)
        {
            _navigationViewModel.SelectedViewModel = new CreateNewModuleViewModel(_navigationViewModel);
        }

        public bool canCreateNewModuleCommand()
        {
            return true;
        }

        public void onLoadModuleCommand(object parameter)
        {
            if(parameter is Module item)
            {
                _navigationViewModel.SelectedViewModel = new TasksViewModel(_navigationViewModel, item.Name);
            }
        }

        public bool canLoadModuleCommand()
        {
            return true;
        }

        public void onRemoveModuleCommand(object parameter)
        {
            if(parameter is Module module)
            {
                MessageBoxResult result = MessageBox.Show(String.Format("Are you sure you want to remove '{0}'",module.Name.ToString()), "Remove Module", MessageBoxButton.YesNo);
                if(result == MessageBoxResult.Yes)
                {
                    foreach(Module searchedModule in ModuleList)
                    {
                        if(searchedModule.Name.ToString() == module.Name.ToString())
                        {
                            ModuleList.Remove(searchedModule);

                            AllModules.modulesList.Clear();
                            foreach (Module remainingModule in ModuleList)
                            {
                                AllModules.modulesList.Add(remainingModule);
                            }

                            WriteAllModuleDataToXML();
                            break;
                        }
                    }
                }
            }
        }
        public bool canRemoveModuleCommand()
        {
            return true;
        }

        public void onEditModuleCommand(object parameter)
        {
            if(parameter is Module module)
            {
                MessageBox.Show(String.Format("'{0}' would have been edited", module.Name.ToString()));
                //I might try to reuse the CreateNewModuleView, but I'm not sure if it would be easier to just create a new view.
            }
        }
        public bool canEditModuleCommand()
        {
            return true;
        }

        #endregion

        #region Other Methods

        public void WriteAllModuleDataToXML()
        {
            bool saved = false;
            string ModulesOutputPath = AllSettings.settings[0].outputFilePath + "\\Modules.xml";
            using (TextWriter writer = new StreamWriter(ModulesOutputPath))
            {
                XmlSerializer xmlS = new XmlSerializer(typeof(ObservableCollection<Module>));
                xmlS.Serialize(writer, AllModules.modulesList);

                saved = true;
            }
            if(!saved)
            {
                MessageBox.Show("Something went wrong :(  Modules have not been saved!");
            }
        }
        #endregion
    }
}
