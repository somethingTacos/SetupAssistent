using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SetupAssistent.Model;
using System.Windows.Data;
using System.Windows;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;

namespace SetupAssistent.ViewModel
{
    class TasksViewModel
    {
        #region Public Propertys

        public CompositeCollection TaskList { get; set; }
        public MyICommand BackCommand { get; set; }
        public MyICommand EditTasksCommand { get; set; }
        public MyICommand RunTaskCommand { get; set; }
        public string CurrentModuleName { get; set; }
        private readonly NavigationViewModel _navigationViewModel;
        #endregion

        #region Default Constructor

        public TasksViewModel(NavigationViewModel navigationViewModel, string ModuleName)
        {
            CurrentModuleName = ModuleName;
            _navigationViewModel = navigationViewModel;
            BackCommand = new MyICommand(onBackCommand, canBackCommand);
            EditTasksCommand = new MyICommand(onEditTasksCommand, canEditTasksCommand);
            RunTaskCommand = new MyICommand(onRunTaskCommand, CanRunTaskCommand);
            LoadTasks();
        }
        #endregion

        #region Commands Code
        public void onBackCommand(object parameter)
        {
            _navigationViewModel.SelectedViewModel = new ModuleViewModel(_navigationViewModel);
        }
        public bool canBackCommand()
        {
            return true;
        }

        public void onEditTasksCommand(object parameter)
        {
            _navigationViewModel.SelectedViewModel = new AddTasksViewModel(_navigationViewModel, CurrentModuleName);
        }
        public bool canEditTasksCommand()
        {
            return true;
        }

        public void onRunTaskCommand(object parameter)
        {
            if(parameter is RunScript runScript)
            {
                try
                {
                    ProcessStartInfo scriptTask = new ProcessStartInfo();
                    scriptTask.FileName = runScript.ScriptSource;
                    string[] tempInfo = runScript.ScriptSource.Split('\\');
                    tempInfo = tempInfo.Where(x => x != tempInfo.Last<string>()).ToArray();
                    string workingDir = String.Join("\\", tempInfo);
                    scriptTask.WorkingDirectory = workingDir;
                    scriptTask.Arguments = runScript.ScriptParameters;
                    Process.Start(scriptTask);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if(parameter is InstallProgram installProgram)
            {
                try
                {
                    ProcessStartInfo programTask = new ProcessStartInfo();
                    programTask.FileName = installProgram.ProgramSource;
                    string[] tempInfo = installProgram.ProgramSource.Split('\\');
                    tempInfo = tempInfo.Where(x => x != tempInfo.Last<string>()).ToArray();
                    string workingDir = String.Join("\\", tempInfo);
                    programTask.WorkingDirectory = workingDir;
                    Process.Start(programTask);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if(parameter is AddLocalAdmin addLocalAdmin)
            {
                try
                {
                    using (var pcLocal = new PrincipalContext(ContextType.Machine))
                    {
                        var group = GroupPrincipal.FindByIdentity(pcLocal, "Administrators");

                        using (var pcDomain = new PrincipalContext(ContextType.Domain, Environment.UserDomainName))
                        {
                            group.Members.Add(pcDomain, IdentityType.SamAccountName, addLocalAdmin.UserName);
                            group.Save();
                        };
                    };
                    MessageBox.Show($"New local admin added: {Environment.UserDomainName}\\{addLocalAdmin.UserName}", "Local Admin Added", MessageBoxButton.OK);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };
            }
        }
        public bool CanRunTaskCommand()
        {
            return true;
        }
        #endregion

        #region LoadTask

        public void LoadTasks()
        {
            if(AllModules.modulesList.Count > 0)
            {
                foreach(Module module in AllModules.modulesList)
                {
                    if(module.Name == CurrentModuleName)
                    {
                        if(module.TasksList.Count == 0)
                        {
                            ModuleTasks tempMT = new ModuleTasks();
                            tempMT.ScriptTasks = new ObservableCollection<RunScript>();
                            tempMT.InstallProgramTasks = new ObservableCollection<InstallProgram>();
                            tempMT.AddLocalAdminTasks = new ObservableCollection<AddLocalAdmin>();

                            module.TasksList.Add(tempMT);
                        }

                        CompositeCollection tempCC = new CompositeCollection();
                        tempCC.Add(new CollectionContainer() { Collection = module.TasksList[0].ScriptTasks });
                        tempCC.Add(new CollectionContainer() { Collection = module.TasksList[0].InstallProgramTasks });
                        tempCC.Add(new CollectionContainer() { Collection = module.TasksList[0].AddLocalAdminTasks });

                        TaskList = tempCC;
                    }
                }
            }
            else
            {
                MessageBox.Show("How the fuck did you get here??!??!?");
            }
        }
        #endregion
    }
}
