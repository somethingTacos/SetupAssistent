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

namespace SetupAssistent.CustomControls
{
    /// <summary>
    /// Interaction logic for ModuleUserControl.xaml
    /// </summary>
    public partial class ModuleUserControl : UserControl
    {
        public ModuleUserControl()
        {
            InitializeComponent();
        }


        #region Dependency Properties

        public static readonly DependencyProperty ModuleNameProperty =
            DependencyProperty.Register("ModuleName", typeof(string), typeof(ModuleUserControl), new PropertyMetadata(""));

        public string ModuleName
        {
            get { return (string)GetValue(ModuleNameProperty); }
            set { SetValue(ModuleNameProperty, value); }
        }

        public static readonly DependencyProperty ModuleDescriptionProperty =
            DependencyProperty.Register("ModuleDesc", typeof(string), typeof(ModuleUserControl), new PropertyMetadata(""));

        public string ModuleDesc
        {
            get { return (string)GetValue(ModuleDescriptionProperty); }
            set { SetValue(ModuleDescriptionProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ModuleUserControl), new PropertyMetadata(null));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty ModuleBorderColorProperty =
            DependencyProperty.Register("ModuleBorderColor", typeof(Brush), typeof(ModuleUserControl), new FrameworkPropertyMetadata(Brushes.LightSlateGray));

        public Brush ModuleBorderColor
        {
            get { return (Brush)GetValue(ModuleBorderColorProperty); }
            set { SetValue(ModuleBorderColorProperty, value); }
        }

        public static readonly DependencyProperty ModuleBorderThicknessProperty =
            DependencyProperty.Register("ModuleBorderThickness", typeof(int), typeof(ModuleUserControl), new PropertyMetadata(2));

        public int ModuleBorderThickness
        {
            get { return (int)GetValue(ModuleBorderThicknessProperty); }
            set { SetValue(ModuleBorderThicknessProperty, value); }
        }


        //public static readonly DependencyProperty IsHoveredModuleProperty =
        //   DependencyProperty.Register("IsHoveredModule", typeof(bool), typeof(ModuleUserControl), new PropertyMetadata(false));
        //public bool IsHoveredModule
        //{
        //    get { return (bool)GetValue(IsHoveredModuleProperty); }
        //    set { SetValue(IsHoveredModuleProperty, value); }
        //}
        #endregion

        private object _SelectedModule;

        public object SelectedModule
        {
            get { return _SelectedModule; }
            set { _SelectedModule = value; }
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectedModule = sender;
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(sender != null && SelectedModule != null)
            {
                if(sender == SelectedModule)
                {
                    MessageBox.Show("Do the thing.");
                }
            }
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            SelectedModule = null;
        }
    }
}
