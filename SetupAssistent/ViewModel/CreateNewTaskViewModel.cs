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
        #region Public Properties
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
        public string TasksOutputPath = string.Empty;

        private string _UserDomain;
        public string UserDomain
        {
            get { return _UserDomain; }
            set { _UserDomain = value; }
        }

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
            TasksOutputPath = AllSettings.settings[0].OutputFilePath + "\\Tasks.xml";

            InitTaskTypes();
        }

        #endregion

        #region Initialize All Task Types

        public void InitTaskTypes()
        {
            UserDomain = $"Current Logon Domain: {Environment.UserDomainName}";

            RunScript tempRS = new RunScript();
            InstallProgram tempIP = new InstallProgram();
            AddLocalAdmin tempALA = new AddLocalAdmin();

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

                            using (TextWriter writer = new StreamWriter(TasksOutputPath))
                            {
                                ObservableCollection<ModuleTasks> tempOC = new ObservableCollection<ModuleTasks>();

                                if(AllTasks.tasksList.Count == 0)
                                {
                                    ModuleTasks tasks = new ModuleTasks();

                                    AllTasks.tasksList.Add(tasks);
                                }

                                tempOC = AllTasks.tasksList;

                                nameUsed = AllTasks.tasksList[0].ScriptTasks.Where(x => x.Name == NewRunScript.Name).Count() > 0;

                                if (!nameUsed)
                                {
                                    try
                                    {
                                        XmlSerializer xmlS = new XmlSerializer(typeof(ObservableCollection<ModuleTasks>));
                                        tempOC[0].ScriptTasks.Add(NewRunScript);
                                        xmlS.Serialize(writer, tempOC);
                                        saved = true;
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message);
                                    }

                                    if (saved)
                                    {
                                        MessageBox.Show($"Task '{NewRunScript.Name.ToString()}' was saved.", "Task Saved");  // This will be replaced with a delayed animation in the future. For now this works well enough for confirmation.  <<<------ IMPORTANT
                                        _navigationViewModel.SelectedViewModel = new AddTasksViewModel(_navigationViewModel, CurrentModuleName);
                                    }
                                    else
                                    {
                                        MessageBox.Show($"Something went wroung.{Environment.NewLine}Could not save Task.", "Oops  :(", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show($"The name '{NewRunScript.Name.ToString()}' is currently in use by another task.{Environment.NewLine}Please choose another name.", "Name Already In Use", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show($"The name '{NewRunScript.Name.ToString()}' is invalid (null) or the script source does not exist.{Environment.NewLine}Please confirm the name and script source are valid.", "Invalid Properties", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                    }
                case 1: //InstallProgram
                    {
                        if (NewInstallProgram.Name.ToString() != "" && NewInstallProgram.ProgramSource.ToString() != "" && File.Exists(NewInstallProgram.ProgramSource.ToString()))
                        {
                            using (TextWriter writer = new StreamWriter(TasksOutputPath))
                            {
                                ObservableCollection<ModuleTasks> tempOC = new ObservableCollection<ModuleTasks>();

                                if (AllTasks.tasksList.Count == 0)
                                {
                                    ModuleTasks tasks = new ModuleTasks();

                                    AllTasks.tasksList.Add(tasks);
                                }

                                tempOC = AllTasks.tasksList;

                                nameUsed = AllTasks.tasksList[0].InstallProgramTasks.Where(x => x.Name == NewInstallProgram.Name).Count() > 0;

                                if (!nameUsed)
                                {
                                    try
                                    {
                                        XmlSerializer xmlS = new XmlSerializer(typeof(ObservableCollection<ModuleTasks>));
                                        tempOC[0].InstallProgramTasks.Add(NewInstallProgram);
                                        xmlS.Serialize(writer, tempOC);
                                        saved = true;
                                    }
                                    catch(Exception ex)
                                    {
                                        MessageBox.Show(ex.Message);
                                    }

                                    if (saved)
                                    {
                                        MessageBox.Show($"Task '{NewInstallProgram.Name.ToString()}' was saved.", "Task Saved");  // This will be replaced with a delayed animation in the future. For now this works well enough for confirmation.  <<<------ IMPORTANT
                                        _navigationViewModel.SelectedViewModel = new AddTasksViewModel(_navigationViewModel, CurrentModuleName);
                                    }
                                    else
                                    {
                                        MessageBox.Show($"Something went wroung.{Environment.NewLine}Could not save Task.", "Oops  :(", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show($"The name '{NewInstallProgram.Name.ToString()}' is currently in use by another task.{Environment.NewLine}Please choose another name.", "Name Already In Use", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show($"The name '{NewInstallProgram.Name.ToString()}' is invalid (null) or the script source does not exist.{Environment.NewLine}Please confirm the name and script source are valid.", "Invalid Properties", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                    }
                case 2: //Add Local Admin
                    {
                        if (NewLocalAdmin.UserName.ToString() != "")
                        {
                            NewLocalAdmin.Name = $"Local Admin: {NewLocalAdmin.UserName}";

                            using (TextWriter writer = new StreamWriter(TasksOutputPath))
                            {
                                ObservableCollection<ModuleTasks> tempOC = new ObservableCollection<ModuleTasks>();

                                if (AllTasks.tasksList.Count == 0)
                                {
                                    ModuleTasks tasks = new ModuleTasks();

                                    AllTasks.tasksList.Add(tasks);
                                }

                                tempOC = AllTasks.tasksList;

                                nameUsed = AllTasks.tasksList[0].AddLocalAdminTasks.Where(x => x.UserName == NewLocalAdmin.UserName).Count() > 0;

                                if (!nameUsed)
                                {
                                    try
                                    {
                                        XmlSerializer xmlS = new XmlSerializer(typeof(ObservableCollection<ModuleTasks>));
                                        tempOC[0].AddLocalAdminTasks.Add(NewLocalAdmin);
                                        xmlS.Serialize(writer, tempOC);
                                        saved = true;
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message);
                                    }

                                    if (saved)
                                    {
                                        MessageBox.Show($"Task '{NewLocalAdmin.UserName.ToString()}' was saved.", "Task Saved");  // This will be replaced with a delayed animation in the future. For now this works well enough for confirmation.  <<<------ IMPORTANT
                                        _navigationViewModel.SelectedViewModel = new AddTasksViewModel(_navigationViewModel, CurrentModuleName);
                                    }
                                    else
                                    {
                                        MessageBox.Show($"Something went wroung.{Environment.NewLine}Could not save Task.", "Oops  :(", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show($"The name '{NewLocalAdmin.UserName.ToString()}' is currently in use by another task.{Environment.NewLine}Please choose another name.", "Name Already In Use", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show($"The name '{NewLocalAdmin.UserName.ToString()}' is invalid (null).{Environment.NewLine}Please confirm the name and script source are valid.", "Invalid Properties", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
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
