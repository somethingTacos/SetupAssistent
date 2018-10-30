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
        public static readonly DependencyProperty SettingsUpdatedProperty =
            DependencyProperty.Register("SettingsUpdated", typeof(bool), typeof(SaveSettingsStackPanel), new PropertyMetadata(false));

        public bool SettingsUpdated
        {
            get { return (bool)GetValue(SettingsUpdatedProperty); }
            set { SetValue(SettingsUpdatedProperty, value); }
        }
    }
}
