using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SetupAssistent.Model;
using System.Windows.Data;
using System.Windows;

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
            //createDummytasks();
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
            MessageBox.Show("Testing");
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


        #region DummyData
        public void createDummytasks()
        {
            //CompositeCollection tempCC = new CompositeCollection();
            //ObservableCollection<RunScript> tempRS = new ObservableCollection<RunScript>();
            //ObservableCollection<InstallProgram> tempIP = new ObservableCollection<InstallProgram>();

            //tempRS.Add(new RunScript { Name = "Some Script", Description = "This is a script.", ScriptSource = "SomeURI_Here", ScriptParameters = "SomeParams" });
            //tempIP.Add(new InstallProgram { Name = "Some Program", Description = "This is a program with a really long description so I can figure out how these will be handled.", ProgramSource = "SomeURI_Here" });

            //tempCC.Add(new CollectionContainer() { Collection = tempRS });
            //tempCC.Add(new CollectionContainer() { Collection = tempIP });
            
            //TaskList = tempCC;
        }
        #endregion
    }
}
