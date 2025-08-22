using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.UI.Components.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    namespace Axon.UI.Components.Converters
    {
        /// <summary>
        /// Convierte valores Boolean a Visibility y viceversa
        /// Soporta conversión invertida y diferentes modos de colapso
        /// </summary>
        public class BooleanToVisibilityConverter : IValueConverter
        {
            /// <summary>
            /// Si es true, invierte la lógica de conversión
            /// True -> Hidden/Collapsed, False -> Visible
            /// </summary>
            public bool IsInverted { get; set; } = false;

            /// <summary>
            /// Si es true, usa Collapsed en lugar de Hidden para valores false
            /// </summary>
            public bool UseCollapsed { get; set; } = true;

            /// <summary>
            /// Convierte de Boolean a Visibility
            /// </summary>
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                bool boolValue = GetBooleanValue(value);

                // Aplicar inversión si está configurada
                if (IsInverted)
                    boolValue = !boolValue;

                if (boolValue)
                    return Visibility.Visible;
                else
                    return UseCollapsed ? Visibility.Collapsed : Visibility.Hidden;
            }

            /// <summary>
            /// Convierte de Visibility a Boolean
            /// </summary>
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is Visibility visibility)
                {
                    bool result = visibility == Visibility.Visible;

                    // Aplicar inversión si está configurada
                    if (IsInverted)
                        result = !result;

                    return result;
                }

                return false;
            }

            /// <summary>
            /// Obtiene el valor booleano de diferentes tipos de entrada
            /// </summary>
            private bool GetBooleanValue(object value)
            {
                if (value == null)
                    return false;

                if (value is bool boolVal)
                    return boolVal;

                if (value is string stringVal)
                {
                    if (bool.TryParse(stringVal, out bool parsedBool))
                        return parsedBool;

                    // Considera strings no vacías como true
                    return !string.IsNullOrWhiteSpace(stringVal);
                }

                if (value is int intVal)
                    return intVal != 0;

                if (value is double doubleVal)
                    return doubleVal != 0.0;

                if (value is decimal decimalVal)
                    return decimalVal != 0m;

                // Para otros tipos, considera null como false y no-null como true
                return value != null;
            }
        }

        /// <summary>
        /// Versión invertida del converter para uso directo en XAML
        /// True -> Hidden/Collapsed, False -> Visible
        /// </summary>
        public class InvertedBooleanToVisibilityConverter : BooleanToVisibilityConverter
        {
            public InvertedBooleanToVisibilityConverter()
            {
                IsInverted = true;
            }
        }

        /// <summary>
        /// Versión que usa Hidden en lugar de Collapsed
        /// </summary>
        public class BooleanToVisibilityHiddenConverter : BooleanToVisibilityConverter
        {
            public BooleanToVisibilityHiddenConverter()
            {
                UseCollapsed = false;
            }
        }
    }
}
