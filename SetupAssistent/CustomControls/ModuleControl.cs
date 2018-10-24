using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SetupAssistent.CustomControls
{
    public class ModuleButton : System.Windows.Controls.Button
    {
        //Is Expanded DP and rounted event
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ModuleButton), new PropertyMetadata(false));

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public static readonly DependencyProperty IsCollaspedProperty =
            DependencyProperty.Register("IsCollapsed", typeof(bool), typeof(ModuleButton), new PropertyMetadata(false));

        public bool IsCollapsed
        {
            get { return (bool)GetValue(IsCollaspedProperty); }
            set { SetValue(IsCollaspedProperty, value); }
        }
    }
}
