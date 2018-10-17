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
            ModulesFile = String.Format("C:\\Users\\{0}\\Desktop\\TestFolder\\Modules.xml", userName);
            TasksFile = String.Format("C:\\Users\\{0}\\Desktop\\TestFolder\\Tasks.xml", userName);
            LoadXMLData();
        }
        #endregion

        #region LoadData
        public void LoadXMLData()
        {
            ObservableCollection<Module> tempOC_Modules = new ObservableCollection<Module>();
            ObservableCollection<ModuleTasks> tempOC_Tasks = new ObservableCollection<ModuleTasks>();

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
            string outputPath = String.Format("C:\\Users\\{0}\\Desktop\\TestFolder\\Modules.xml", Environment.UserName);
            using (TextWriter writer = new StreamWriter(outputPath))
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
