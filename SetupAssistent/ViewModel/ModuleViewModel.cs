﻿using System;
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
        public MyICommand RemoveModule { get; set; }
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
            LoadModuleCommand = new MyICommand(onLoadModule, canLoadModule);
            CreateNewModuleCommand = new MyICommand(onCreateNewModule, canCreateNewModule);
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

        public void onCreateNewModule(object parameter)
        {
            _navigationViewModel.SelectedViewModel = new CreateNewModuleViewModel(_navigationViewModel);
        }

        public bool canCreateNewModule()
        {
            return true;
        }

        public void onLoadModule(object parameter)
        {
            if(parameter is Module item)
            {
                _navigationViewModel.SelectedViewModel = new TasksViewModel(_navigationViewModel, item.Name);
            }
        }

        public bool canLoadModule()
        {
            return true;
        }

        public void onRemoveModule(object parameter)
        {

        }
        public bool canRemoveModule()
        {
            return true;
        }

        #endregion
    }
}
