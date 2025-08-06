using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;

namespace Axon.UI.Components.TreeNode
{
    public class NodeTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TreeNodeType nodeType)
            {
                return CreateIcon(nodeType);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private UIElement CreateIcon(TreeNodeType nodeType)
        {
            var path = new Path
            {
                Width = 16,
                Height = 16,
                Fill = new SolidColorBrush(Colors.White),
                Stretch = Stretch.Uniform
            };

            switch (nodeType)
            {
                case TreeNodeType.Folder:
                    // Icono de carpeta
                    path.Data = Geometry.Parse("M10,4H4C2.89,4 2,4.89 2,6V18A2,2 0 0,0 4,20H20A2,2 0 0,0 22,18V8C22,6.89 21.1,6 20,6H12L10,4Z");
                    path.Fill = new SolidColorBrush(Color.FromRgb(255, 193, 7)); // Amarillo
                    break;

                case TreeNodeType.Device:
                    // Icono de dispositivo
                    path.Data = Geometry.Parse("M4,2H20A2,2 0 0,1 22,4V16A2,2 0 0,1 20,18H16V20A1,1 0 0,1 15,21H9A1,1 0 0,1 8,20V18H4A2,2 0 0,1 2,16V4A2,2 0 0,1 4,2M4,4V16H20V4H4Z");
                    path.Fill = new SolidColorBrush(Color.FromRgb(0, 123, 255)); // Azul
                    break;

                case TreeNodeType.Protocol:
                    // Icono de protocolo/conexión
                    path.Data = Geometry.Parse("M12,1L21.5,6.5V17.5L12,23L2.5,17.5V6.5L12,1M12,3.311L5,7.65311V16.3469L12,20.689L19,16.3469V7.65311L12,3.311Z");
                    path.Fill = new SolidColorBrush(Color.FromRgb(40, 167, 69)); // Verde
                    break;

                default:
                    // Icono por defecto
                    path.Data = Geometry.Parse("M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2Z");
                    break;
            }

            return path;
        }
    }
}
