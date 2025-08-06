using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Diagnostics;

namespace Axon.UI.Components.TreeNode
{
    public static class TreeViewHelper
    {
        // Propiedad attached para el nivel
        public static readonly DependencyProperty LevelProperty =
            DependencyProperty.RegisterAttached(
                "Level",
                typeof(int),
                typeof(TreeViewHelper),
                new PropertyMetadata(0));

        public static int GetLevel(DependencyObject obj)
        {
            return (int)obj.GetValue(LevelProperty);
        }

        public static void SetLevel(DependencyObject obj, int value)
        {
            obj.SetValue(LevelProperty, value);
        }

        // Método para calcular el nivel automáticamente
        public static int CalculateLevel(TreeViewItem item)
        {
            int level = 0;
            var parent = ItemsControl.ItemsControlFromItemContainer(item) as TreeViewItem;

            while (parent != null)
            {
                level++;
                parent = ItemsControl.ItemsControlFromItemContainer(parent) as TreeViewItem;
            }

            return level;
        }
    }

    // Converter para la indentación basada en el nivel
    public class LevelToIndentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.WriteLine($"LevelToIndentConverter - Input: {value}, Type: {value?.GetType()}");

            if (value is int level)
            {
                var result = level * 9; // 9 pixels por nivel
                Debug.WriteLine($"LevelToIndentConverter - Output: {result}");
                return result;
            }

            Debug.WriteLine("LevelToIndentConverter - Returning 0 (default)");
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

   
}
