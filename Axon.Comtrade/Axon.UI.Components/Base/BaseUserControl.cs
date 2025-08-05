using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Axon.UI.Components.Base
{
    public class BaseUserControl:UserControl
    {
        public static readonly DependencyProperty ThemeProperty =
           DependencyProperty.Register("Theme", typeof(string), typeof(BaseUserControl),
               new PropertyMetadata("Light", OnThemeChanged));

        public string Theme
        {
            get { return (string)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }

        private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BaseUserControl control)
            {
                control.UpdateTheme();
            }
        }

        protected virtual void UpdateTheme()
        {
            // Lógica para cambiar tema
        }
    }
}
