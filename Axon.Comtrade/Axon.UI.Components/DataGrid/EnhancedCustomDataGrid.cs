using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using System;
using System.Linq;
using System.Windows.Media;

namespace Axon.UI.Components.Datagrid
{
    public class EnhancedCustomDataGrid : DataGrid
    {
        static EnhancedCustomDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EnhancedCustomDataGrid),
                new FrameworkPropertyMetadata(typeof(EnhancedCustomDataGrid)));
        }

        public EnhancedCustomDataGrid() : base()
        {
            InitializeEnhancedFeatures();
        }

        #region Additional Dependency Properties

        public static readonly DependencyProperty ShowFiltersProperty =
            DependencyProperty.Register(nameof(ShowFilters), typeof(bool), typeof(EnhancedCustomDataGrid),
                new PropertyMetadata(true, OnShowFiltersChanged));

        public bool ShowFilters
        {
            get => (bool)GetValue(ShowFiltersProperty);
            set => SetValue(ShowFiltersProperty, value);
        }

        public static readonly DependencyProperty ShowGroupingProperty =
            DependencyProperty.Register(nameof(ShowGrouping), typeof(bool), typeof(EnhancedCustomDataGrid),
                new PropertyMetadata(false, OnShowGroupingChanged));

        public bool ShowGrouping
        {
            get => (bool)GetValue(ShowGroupingProperty);
            set => SetValue(ShowGroupingProperty, value);
        }

        #endregion

        #region Enhanced Properties

        private readonly Dictionary<string, string> _columnFilters = new Dictionary<string, string>();

        #endregion

        #region Initialization

        private void InitializeEnhancedFeatures()
        {
            // ✅ SOLUCIÓN: Aplicar estilos básicos pero sin interferir con GroupStyle
            try
            {
                // Solo aplicar estilos si existen, de forma segura
                ApplyStyleSafely("EnhancedDataGridStyle", style => Style = style);
                ApplyStyleSafely("EnhancedDataGridCellStyle", style => CellStyle = style);
                ApplyStyleSafely("EnhancedDataGridRowStyle", style => RowStyle = style);
                ApplyStyleSafely("EnhancedDataGridColumnHeaderStyle", style => ColumnHeaderStyle = style);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Could not apply styles: {ex.Message}");
                // Aplicar estilos básicos manualmente si no encuentra los recursos
                ApplyBasicStyles();
            }

            // Configurar eventos
            Loaded += OnEnhancedLoaded;
            DataContextChanged += OnEnhancedDataContextChanged;
        }

        private void ApplyStyleSafely(string styleName, Action<Style> applyAction)
        {
            try
            {
                if (TryFindResource(styleName) is Style style)
                {
                    applyAction(style);
                    System.Diagnostics.Debug.WriteLine($"Applied style: {styleName}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Style not found: {styleName}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying style {styleName}: {ex.Message}");
            }
        }

        private void ApplyBasicStyles()
        {
            // Estilos básicos para que se vea el DataGrid
            Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)); // #1E1E1E
            Foreground = new SolidColorBrush(Colors.White);
            BorderBrush = new SolidColorBrush(Color.FromRgb(70, 70, 70));
            BorderThickness = new Thickness(1);
            GridLinesVisibility = DataGridGridLinesVisibility.Horizontal;
            HorizontalGridLinesBrush = new SolidColorBrush(Color.FromRgb(50, 50, 50));
            CanUserAddRows = false;
            CanUserDeleteRows = false;
            AutoGenerateColumns = false;
            AlternationCount = 2;

            System.Diagnostics.Debug.WriteLine("Applied basic styles manually");
        }

        private void OnEnhancedLoaded(object sender, RoutedEventArgs e)
        {
            // ✅ NO llamar UpdateGroupingVisibility aquí
            // El GroupStyle ya está definido en XAML
            System.Diagnostics.Debug.WriteLine("EnhancedCustomDataGrid loaded");
        }

        private void OnEnhancedDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // ✅ NO interferir con la configuración del CollectionView
            System.Diagnostics.Debug.WriteLine("EnhancedCustomDataGrid DataContext changed");
        }

        #endregion

        #region Event Handlers

        private static void OnShowFiltersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EnhancedCustomDataGrid dataGrid)
            {
                // Solo manejar filtros, no agrupación
                dataGrid.UpdateFilterVisibility();
            }
        }

        private static void OnShowGroupingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EnhancedCustomDataGrid dataGrid)
            {
                // ✅ CRÍTICO: NO interferir con GroupStyle si está definido en XAML
                System.Diagnostics.Debug.WriteLine($"ShowGrouping changed to: {e.NewValue}");
                // NO llamar UpdateGroupingVisibility()
            }
        }

        #endregion

        #region Filtering Implementation

        public void SetColumnFilter(string columnName, string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                _columnFilters.Remove(columnName);
            }
            else
            {
                _columnFilters[columnName] = filter;
            }
            ApplyFilters();
        }

        public void ClearAllFilters()
        {
            _columnFilters.Clear();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var collectionView = CollectionViewSource.GetDefaultView(ItemsSource);
            if (collectionView != null)
            {
                collectionView.Filter = item =>
                {
                    foreach (var filter in _columnFilters)
                    {
                        if (!ApplyColumnFilter(item, filter.Key, filter.Value))
                            return false;
                    }
                    return true;
                };

                collectionView.Refresh();
            }
        }

        private bool ApplyColumnFilter(object item, string columnName, string filter)
        {
            try
            {
                return FilterByProperty(item, columnName, filter);
            }
            catch
            {
                return true;
            }
        }

        private bool FilterByProperty(object item, string propertyName, string filter)
        {
            try
            {
                var property = item.GetType().GetProperty(propertyName);
                if (property == null) return true;

                var value = property.GetValue(item)?.ToString() ?? "";
                return value.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            catch
            {
                return true;
            }
        }

        #endregion

        #region UI Update Methods

        private void UpdateFilterVisibility()
        {
            // Implementar lógica para mostrar/ocultar filtros en headers
            foreach (var column in Columns)
            {
                if (column.Header is FrameworkElement header)
                {
                    var filterBox = FindChild<TextBox>(header, "FilterTextBox");
                    if (filterBox != null)
                    {
                        filterBox.Visibility = ShowFilters ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
            }
        }

        #endregion

        #region Utility Methods

        public static T FindChild<T>(DependencyObject parent, string childName = null) where T : DependencyObject
        {
            if (parent == null) return null;

            T foundChild = null;
            int childrenCount = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);

                if (child is T t && (string.IsNullOrEmpty(childName) ||
                    (child is FrameworkElement fe && fe.Name == childName)))
                {
                    foundChild = t;
                    break;
                }

                foundChild = FindChild<T>(child, childName);
                if (foundChild != null) break;
            }

            return foundChild;
        }

        #endregion

        #region Public API Methods

        /// <summary>
        /// Exporta los datos a CSV
        /// </summary>
        public string ExportToCsv()
        {
            var csv = new StringBuilder();

            // Headers
            var headers = Columns.Where(c => c.Visibility == Visibility.Visible)
                                 .Select(c => c.Header?.ToString() ?? "")
                                 .ToArray();
            csv.AppendLine(string.Join(",", headers));

            // Data
            foreach (var item in Items)
            {
                var values = new List<string>();
                foreach (var column in Columns.Where(c => c.Visibility == Visibility.Visible))
                {
                    var value = GetCellValue(item, column) ?? "";
                    values.Add($"\"{value.ToString().Replace("\"", "\"\"")}\"");
                }
                csv.AppendLine(string.Join(",", values));
            }

            return csv.ToString();
        }

        private object GetCellValue(object item, DataGridColumn column)
        {
            try
            {
                if (column is DataGridBoundColumn boundColumn &&
                    boundColumn.Binding is Binding binding)
                {
                    var property = item.GetType().GetProperty(binding.Path.Path);
                    return property?.GetValue(item);
                }
            }
            catch
            {
                // Fallback silencioso
            }
            return null;
        }

        #endregion
    }
}