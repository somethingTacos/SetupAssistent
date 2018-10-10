﻿using System;
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
using System.Windows.Media.Animation;
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

            Module_Animation(8, Colors.Red, TimeSpan.FromMilliseconds(150));
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
            //run some animations for fancy here on mouse leave
            Module_Animation(2, Colors.LightSlateGray, TimeSpan.FromMilliseconds(250));
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            //run some animations for fancy here on mouse enter
            Module_Animation(5, Colors.DodgerBlue, TimeSpan.FromMilliseconds(250));
        }

        public void Module_Animation(int NewBorderThickness, Color NewBorderColor, TimeSpan Duration)
        {
            DoubleAnimation animation1 = new DoubleAnimation(NewBorderThickness, Duration);
            ColorAnimation animation2 = new ColorAnimation(NewBorderColor, Duration);

            ModuleBody_rectangle.Stroke = new SolidColorBrush(NewBorderColor);

            ModuleBody_rectangle.BeginAnimation(Rectangle.StrokeThicknessProperty, animation1);
            ModuleBody_rectangle.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, animation2);
        }
    }
}
