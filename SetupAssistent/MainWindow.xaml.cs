using SetupAssistent.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SetupAssistent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            var viewmodel = new NavigationViewModel();
            viewmodel.SelectedViewModel = new ModuleViewModel(viewmodel);
            this.DataContext = viewmodel;
        }

        private void Settings_Button_Click(object sender, RoutedEventArgs e)
        {
            Window settingsWindow = new Window();
            SettingsViewModel settingsVM = new SettingsViewModel();
            settingsWindow.Content = settingsVM;
            settingsWindow.MinWidth = 500;
            settingsWindow.MinHeight = 400;
            settingsWindow.Width = 500;
            settingsWindow.Height = 400;
            settingsWindow.Title = "Settings";
            settingsWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            settingsWindow.ShowDialog();
        }
    }
}
