using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SetupAssistent.Model;
using System.IO;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace SetupAssistent.ViewModel
{
    public class CreateNewTaskViewModel
    {
        #region Public Propertys
        public RunScript NewRunScript { get; set; }
        public InstallProgram NewInstallProgram { get; set; }
        public AddLocalAdmin NewLocalAdmin { get; set; }

        public TaskTypeIndex CurrentTaskType { get; set; }

        public MyICommand SaveCommand { get; set; }
        public MyICommand CancelCommand { get; set; }
        public MyICommand FindCommand { get; set; }

        private readonly NavigationViewModel _navigationViewModel;
        public string CurrentModuleName { get; set; }
        public string userName = Environment.UserName;
        public string outputPath = string.Empty;

        #endregion

        #region Default Constructor

        public CreateNewTaskViewModel(NavigationViewModel navigationViewModel, string currentModuleName)
        {
            CurrentModuleName = currentModuleName;

            CancelCommand = new MyICommand(onCancelCommand, canCancelCommand);
            SaveCommand = new MyICommand(onSaveCommand, canSaveCommand);
            FindCommand = new MyICommand(onFindCommand, canFindCommand);

            _navigationViewModel = navigationViewModel;
            CurrentTaskType = new TaskTypeIndex();
            CurrentTaskType.SelectedTaskTypeIndex = 0;
            //This will be a setting later
            outputPath = String.Format("C:\\Users\\{0}\\Desktop\\TestFolder\\Tasks.xml", userName);

            InitTaskTypes();
        }

        #endregion

        #region Initialize All Task Types

        public void InitTaskTypes()
        {
            //There has to be a better way to do this...
            RunScript tempRS = new RunScript();
            tempRS.Name = "";
            tempRS.Description = "";
            tempRS.ScriptParameters = "";
            tempRS.ScriptSource = "";
            tempRS.IsIncluded = false;

            InstallProgram tempIP = new InstallProgram();
            tempIP.Name = "";
            tempIP.Description = "";
            tempIP.ProgramSource = "";
            tempIP.IsIncluded = false;

            AddLocalAdmin tempALA = new AddLocalAdmin();
            tempALA.UserName = "";
            tempALA.IsIncluded = false;


            NewRunScript = tempRS;
            NewInstallProgram = tempIP;
            NewLocalAdmin = tempALA;
        }
        #endregion

        #region Commands Code

        public void onFindCommand(object parameter)
        {
            Microsoft.Win32.OpenFileDialog filePicker = new Microsoft.Win32.OpenFileDialog();

            switch(parameter.ToString())
            {
                case "RunScript":
                    {

                        filePicker.Filter = "Batch|*.bat|PowerShell|*.ps1|"
                                          + "Accepted Types|*.bat;*.ps1";
                        filePicker.FilterIndex = 3;
                        break;
                    }

                case "InstallProgram":
                    {
                        filePicker.Filter = "Windows Installer|*.msi|Executable|*.exe|"
                                         + "Accepted Types|*.msi;*.exe";
                        filePicker.FilterIndex = 3;
                        break;
                    }
            }

            Nullable<bool> result = filePicker.ShowDialog();
            if (result == true)
            {
                switch (parameter.ToString())
                {
                    case "RunScript":
                        {
                            NewRunScript.ScriptSource = filePicker.FileName.ToString();

                            break;
                        }

                    case "InstallProgram":
                        {
                            NewInstallProgram.ProgramSource = filePicker.FileName.ToString();
                            break;
                        }
                }
            }
        }
        public bool canFindCommand()
        {
            return true;
        }

        public void onSaveCommand(object parameter) //there is probably a better way to do this, but I couldn't figure out how to do it at the time of making this.   :(
        {
            bool saved = false;
            bool nameUsed = false;
            switch (CurrentTaskType.SelectedTaskTypeIndex)
            {
                case 0: //RunScript
                    {
                        if (NewRunScript.Name.ToString() != "" && NewRunScript.ScriptSource.ToString() != "" && File.Exists(NewRunScript.ScriptSource.ToString()))
                        {

                            using (TextWriter writer = new StreamWriter(outputPath))
                            {
                                ObservableCollection<ModuleTasks> tempOC = new ObservableCollection<ModuleTasks>();

                                if(AllTasks.tasksList.Count == 0)
                                {
                                    ModuleTasks tasks = new ModuleTasks();
                                    tasks.ScriptTasks = new ObservableCollection<RunScript>();
                                    tasks.InstallProgramTasks = new ObservableCollection<InstallProgram>();
                                    tasks.AddLocalAdminTasks = new ObservableCollection<AddLocalAdmin>();

                                    AllTasks.tasksList.Add(tasks);
                                }

                                tempOC = AllTasks.tasksList;

                                foreach (RunScript RS in tempOC[0].ScriptTasks)
                                {
                                    if (RS.Name.ToString() == NewRunScript.Name.ToString())
                                    {
                                        nameUsed = true;
                                    }
                                }

                                if (!nameUsed)
                                {
                                    XmlSerializer xmlS = new XmlSerializer(typeof(ObservableCollection<ModuleTasks>));
                                    tempOC[0].ScriptTasks.Add(NewRunScript);
                                    xmlS.Serialize(writer, tempOC);
                                    saved = true;

                                    if (saved)
                                    {
                                        MessageBox.Show(String.Format("Task '{0}' was saved.", NewRunScript.Name.ToString()), "Task Saved");  // This will be replaced with a delayed animation in the future. For now this works well enough for confirmation.  <<<------ IMPORTANT
                                        _navigationViewModel.SelectedViewModel = new AddTasksViewModel(_navigationViewModel, CurrentModuleName);
                                    }
                                    else
                                    {
                                        MessageBox.Show(String.Format("Something went wroung.{0}Could not save Task.", Environment.NewLine), "Oops  :(", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(String.Format("The name '{0}' is currently in use by another task.{1}Please choose another name.", NewRunScript.Name.ToString(), Environment.NewLine), "Name Already In Use", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show(String.Format("The name '{0}' is invalid (null) or the script source does not exist.{1}Please confirm the name and script source are valid.", NewRunScript.Name.ToString(), Environment.NewLine), "Invalid Properties", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                    }
                case 1: //InstallProgram
                    {
                        if (NewInstallProgram.Name.ToString() != "" && NewInstallProgram.ProgramSource.ToString() != "" && File.Exists(NewInstallProgram.ProgramSource.ToString()))
                        {

                            using (TextWriter writer = new StreamWriter(outputPath))
                            {
                                ObservableCollection<ModuleTasks> tempOC = new ObservableCollection<ModuleTasks>();

                                if (AllTasks.tasksList.Count == 0)
                                {
                                    ModuleTasks tasks = new ModuleTasks();
                                    tasks.ScriptTasks = new ObservableCollection<RunScript>();
                                    tasks.InstallProgramTasks = new ObservableCollection<InstallProgram>();

                                    AllTasks.tasksList.Add(tasks);
                                }

                                tempOC = AllTasks.tasksList;

                                foreach (InstallProgram IP in tempOC[0].InstallProgramTasks)
                                {
                                    if (IP.Name.ToString() == NewInstallProgram.Name.ToString())
                                    {
                                        nameUsed = true;
                                    }
                                }

                                if (!nameUsed)
                                {
                                    XmlSerializer xmlS = new XmlSerializer(typeof(ObservableCollection<ModuleTasks>));
                                    tempOC[0].InstallProgramTasks.Add(NewInstallProgram);
                                    xmlS.Serialize(writer, tempOC);
                                    saved = true;

                                    if (saved)
                                    {
                                        MessageBox.Show(String.Format("Task '{0}' was saved.", NewInstallProgram.Name.ToString()), "Task Saved");  // This will be replaced with a delayed animation in the future. For now this works well enough for confirmation.  <<<------ IMPORTANT
                                        _navigationViewModel.SelectedViewModel = new AddTasksViewModel(_navigationViewModel, CurrentModuleName);
                                    }
                                    else
                                    {
                                        MessageBox.Show(String.Format("Something went wroung.{0}Could not save Task.", Environment.NewLine), "Oops  :(", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(String.Format("The name '{0}' is currently in use by another task.{1}Please choose another name.", NewInstallProgram.Name.ToString(), Environment.NewLine), "Name Already In Use", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show(String.Format("The name '{0}' is invalid (null) or the script source does not exist.{1}Please confirm the name and script source are valid.", NewInstallProgram.Name.ToString(), Environment.NewLine), "Invalid Properties", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                    }
                case 2: //Add Local Admin
                    {

                        break;
                    }
            }
        }

        public bool canSaveCommand()
        {
            return true;
        }

        public void onCancelCommand(object parameter)
        {
            _navigationViewModel.SelectedViewModel = new AddTasksViewModel(_navigationViewModel, CurrentModuleName);
        }
        public bool canCancelCommand()
        {
            return true;
        }

        #endregion
    }

    #region TaskTypeIndex Class
    public class TaskTypeIndex : INotifyPropertyChanged
    {
        private int _selectedTaskTypeIndex;
        public int SelectedTaskTypeIndex
        {
            get { return _selectedTaskTypeIndex; }
            set
            {
                _selectedTaskTypeIndex = value;
                RaisePropertyChanged("SelectedTaskTypeIndex");
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
    #endregion
}
