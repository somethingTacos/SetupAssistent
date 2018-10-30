using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SetupAssistent.CustomControls
{
    public class SaveSettingsStackPanel : System.Windows.Controls.StackPanel
    {
        public static readonly DependencyProperty SettingsChangedProperty =
            DependencyProperty.Register("SettingsChanged", typeof(bool), typeof(SaveSettingsStackPanel), new PropertyMetadata(false));

        public bool SettingsChanged
        {
            get { return (bool)GetValue(SettingsChangedProperty); }
            set { SetValue(SettingsChangedProperty, value); }
        }
    }
}
