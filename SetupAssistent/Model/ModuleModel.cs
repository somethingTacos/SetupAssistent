using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using System.Collections.ObjectModel;
using SetupAssistent.ViewModel;

namespace SetupAssistent.Model
{
    [AddINotifyPropertyChangedInterface]
    public class ModuleModel { }

    public class Module : INotifyPropertyChanged
    {
        /* Let's think...
         * 
         * Model Structure:
         *     -Name
         *     -Customizable Colors? -- Maybe only themed colors to keep the settings simple. Going to need a Settings screen too though, which might be quite baren. Possible Issolated themes per module?
         *     -Description
         *     -Images? (should you be able to add images? Background and in-line images?) -- I'm thinking only background images for this to keep the UI looking cleaner, but we'll see what happens.
         *     -Necessary Drives (I can use these to lock out modules if the current computer is missing drives needed to access certain programs/scripts.)
         *     -options (remove/edit module)
         * 
         * Other Thoughts:
         *     -Should I sync modules to the local network?
         *     -Should I have all the tasks linked to the moduleModel or create their own model?  hmmm...
         */

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string DescriptionPreview { get; set; } = "";
        public string ImageSource { get; set; } = "";
        public bool allowRemove { get; set; } = true;
        public bool allowEdit { get; set; } = true;

        public ObservableCollection<ModuleTasks> TasksList { get; set; } = new ObservableCollection<ModuleTasks>();
    }

    public static class AllModules
    {
        public static bool LoadedAtStartup = false;
        public static ObservableCollection<Module> modulesList = new ObservableCollection<Module>();
    }

}
