using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Data;
using System.Xml.Serialization;

namespace SetupAssistent.Model
{
    public class TasksModel { }

    [AddINotifyPropertyChangedInterface]
    public class ModuleTasks : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        [XmlIgnore]
        public IList TasksList
        {
            get
            {
                return new CompositeCollection
                {
                    new CollectionContainer() {Collection = ScriptTasks},
                    new CollectionContainer() {Collection = InstallProgramTasks},
                    new CollectionContainer() {Collection = AddLocalAdminTasks}
                };
            }
        }

        public ObservableCollection<RunScript> ScriptTasks { get; set; }
        public ObservableCollection<InstallProgram> InstallProgramTasks { get; set; }
        public ObservableCollection<AddLocalAdmin> AddLocalAdminTasks { get; set; }
    }

    #region Task Types

    [AddINotifyPropertyChangedInterface]
    public class RunScript : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public string Name { get; set; }
        public string Description { get; set; }
        public string ScriptSource { get; set; }
        public string ScriptParameters { get; set; }
        public bool IsIncluded { get; set; }
    }

    [AddINotifyPropertyChangedInterface]
    public class InstallProgram : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public string Name { get; set; }
        public string Description { get; set; }
        public string ProgramSource { get; set; }
        public bool IsIncluded { get; set; }
    }

    [AddINotifyPropertyChangedInterface]
    public class AddLocalAdmin : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public string UserName { get; set; }
        public bool IsIncluded { get; set; }
    }
    #endregion

    public static class AllTasks
    {
        public static bool LoadedAtStartup = false;
        public static ObservableCollection<ModuleTasks> tasksList = new ObservableCollection<ModuleTasks>();
    }
}
