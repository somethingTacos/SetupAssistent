﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SetupAssistent.Model;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.IO;
using System.Xml.Serialization;
using System.Windows;

namespace SetupAssistent.ViewModel
{
    class AddTasksViewModel
    {
        public ModuleTasks AllModuleTasks { get; set; }
        public MyICommand ToggleTaskSelectedCommand { get; set; }
        public MyICommand DoneCommand { get; set; }
        public MyICommand CreateNewTaskCommand { get; set; }
        private readonly NavigationViewModel _navigationViewModel;
        public string ModuleName;

        #region Default Constructor
        public AddTasksViewModel(NavigationViewModel navigationViewModel, string moduleName)
        {
            _navigationViewModel = navigationViewModel;
            ModuleName = moduleName;
            
            DoneCommand = new MyICommand(onDoneCommand, canDoneCommand);
            CreateNewTaskCommand = new MyICommand(onCreateNewTaskCommand, canCreateNewTaskCommand);
            ToggleTaskSelectedCommand = new MyICommand(onToggleTaskSelectedCommand, canToggleTaskSelectedCommand);

            InitCurrentModuleTasks();
            LoadTasks();
        }

        #endregion

        #region Commands Code

        public void onCreateNewTaskCommand(object parameter)
        {
            _navigationViewModel.SelectedViewModel = new CreateNewTaskViewModel(_navigationViewModel, ModuleName);
        }
        public bool canCreateNewTaskCommand()
        {
            return true;
        }

        public void onDoneCommand(object parameter)
        {
            if (AllModuleTasks.TasksList.Count > 0)
            {
                ModuleTasks currentSelectedTasks = new ModuleTasks();
                currentSelectedTasks.ScriptTasks = new ObservableCollection<RunScript>();
                currentSelectedTasks.InstallProgramTasks = new ObservableCollection<InstallProgram>();
                currentSelectedTasks.AddLocalAdminTasks = new ObservableCollection<AddLocalAdmin>();

                foreach (object taskCollection in AllModuleTasks.TasksList)
                {
                    if (taskCollection is CollectionContainer CC)
                    {
                        foreach (object task in CC.Collection)
                        {
                            if (task is RunScript RS)
                            {
                                if (RS.IsIncluded)
                                {
                                    currentSelectedTasks.ScriptTasks.Add(RS);
                                }
                            }
                            if (task is InstallProgram IP)
                            {
                                if (IP.IsIncluded)
                                {
                                    currentSelectedTasks.InstallProgramTasks.Add(IP);
                                }
                            }
                            if (task is AddLocalAdmin ALA)
                            {
                                if (ALA.IsIncluded)
                                {
                                    currentSelectedTasks.AddLocalAdminTasks.Add(ALA);
                                }
                            }
                        }
                    }
                }

                foreach (Module module in AllModules.modulesList)
                {
                    if (module.Name.ToString() == ModuleName)
                    {
                        module.TasksList.Clear();
                        module.TasksList.Add(currentSelectedTasks);
                    }
                }

                try
                {
                    bool saved = false;
                    string ModulesOutputPath = AllSettings.settings[0].outputFilePath + "\\Modules.xml";

                    using (TextWriter writer = new StreamWriter(ModulesOutputPath))
                    {
                        XmlSerializer xmlS = new XmlSerializer(typeof(ObservableCollection<Module>));
                        xmlS.Serialize(writer, AllModules.modulesList);

                        saved = true;
                    }

                    if (saved)
                    {

                    }
                    else
                    {

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }


            _navigationViewModel.SelectedViewModel = new TasksViewModel(_navigationViewModel, ModuleName);
        }
        public bool canDoneCommand()
        {
            return true;
        }

        public void onToggleTaskSelectedCommand(object parameter)
        {
            if(parameter is RunScript RS)
            {
                if(RS.IsIncluded)
                {
                    RS.IsIncluded = false;
                }
                else
                {
                    RS.IsIncluded = true;
                }
            }
            if (parameter is InstallProgram IP)
            {
                if(IP.IsIncluded)
                {
                    IP.IsIncluded = false;
                }
                else
                {
                    IP.IsIncluded = true;
                }
            }
            if (parameter is AddLocalAdmin ALA)
            {
                if(ALA.IsIncluded)
                {
                    ALA.IsIncluded = false;
                }
                else
                {
                    ALA.IsIncluded = true;
                }
            }
        }
        public bool canToggleTaskSelectedCommand()
        {
            return true;
        }

        #endregion

        #region Additional Methods

        public void InitCurrentModuleTasks()
        {
            ModuleTasks tempMT = new ModuleTasks();
            tempMT.ScriptTasks = new ObservableCollection<RunScript>();
            tempMT.InstallProgramTasks = new ObservableCollection<InstallProgram>();
            tempMT.AddLocalAdminTasks = new ObservableCollection<AddLocalAdmin>();

            AllModuleTasks = tempMT;
        }

        public void LoadTasks()
        {
            AllModuleTasks.ScriptTasks.Clear();
            AllModuleTasks.InstallProgramTasks.Clear();
            AllModuleTasks.AddLocalAdminTasks.Clear();

            if (AllTasks.tasksList.Count > 0)
            {
                foreach (RunScript runScript in AllTasks.tasksList[0].ScriptTasks)
                {
                    runScript.IsIncluded = false;
                    foreach (Module module in AllModules.modulesList)
                    {
                        if (module.Name == ModuleName)
                        {
                            foreach(RunScript _runScript in module.TasksList[0].ScriptTasks)
                            {
                                if(_runScript.Name == runScript.Name)
                                {
                                    runScript.IsIncluded = true;
                                }
                            }
                        }
                    }

                    AllModuleTasks.ScriptTasks.Add(runScript);
                }

                foreach(InstallProgram installProgram in AllTasks.tasksList[0].InstallProgramTasks)
                {
                    installProgram.IsIncluded = false;
                    foreach (Module module in AllModules.modulesList)
                    {
                        if (module.Name == ModuleName)
                        {
                            foreach (InstallProgram _installProgram in module.TasksList[0].InstallProgramTasks)
                            {
                                if (_installProgram.Name == installProgram.Name)
                                {
                                    installProgram.IsIncluded = true;
                                }
                            }
                        }
                    }

                    AllModuleTasks.InstallProgramTasks.Add(installProgram);
                }

                foreach(AddLocalAdmin addLocalAdmin in AllTasks.tasksList[0].AddLocalAdminTasks)
                {
                    addLocalAdmin.IsIncluded = false;
                    foreach (Module module in AllModules.modulesList)
                    {
                        if (module.Name == ModuleName)
                        {
                            foreach (AddLocalAdmin _addLocalAdmin in module.TasksList[0].AddLocalAdminTasks)
                            {
                                if (_addLocalAdmin.UserName == addLocalAdmin.UserName)
                                {
                                    addLocalAdmin.IsIncluded = true;
                                }
                            }
                        }
                    }

                    AllModuleTasks.AddLocalAdminTasks.Add(addLocalAdmin);
                }
            }
        }
        #endregion
    }
}
