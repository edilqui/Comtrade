using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Documents;
using System.Text;
using System.Windows.Controls.Primitives;

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
        private ICollectionView _collectionView;

        #endregion

        #region Initialization

        private void InitializeEnhancedFeatures()
        {
            // Aplicar estilos mejorados si están disponibles
            try
            {
                if (FindResource("EnhancedDataGridStyle") is Style dataGridStyle)
                    Style = dataGridStyle;

                if (FindResource("EnhancedDataGridCellStyle") is Style cellStyle)
                    CellStyle = cellStyle;

                if (FindResource("EnhancedDataGridRowStyle") is Style rowStyle)
                    RowStyle = rowStyle;

                // Usar el estilo sin filtros por defecto
                if (FindResource("EnhancedDataGridColumnHeaderStyle") is Style headerStyle)
                    ColumnHeaderStyle = headerStyle;

                if (FindResource("EnhancedDataGridRowHeaderStyle") is Style rowHeaderStyle)
                    RowHeaderStyle = rowHeaderStyle;
            }
            catch
            {
                // Si no encuentra los estilos, usar los por defecto
            }

            // Configuración adicional
            Loaded += OnEnhancedLoaded;
            DataContextChanged += OnEnhancedDataContextChanged;
        }

        public void EnableFilters(bool enable)
        {
            try
            {
                var styleName = enable ? "EnhancedDataGridColumnHeaderWithFilterStyle" : "EnhancedDataGridColumnHeaderStyle";
                if (FindResource(styleName) is Style headerStyle)
                {
                    ColumnHeaderStyle = headerStyle;
                }
            }
            catch
            {
                // Manejar error
            }
        }

        private void OnEnhancedLoaded(object sender, RoutedEventArgs e)
        {
            UpdateFilterVisibility();
            UpdateGroupingVisibility();
            SetupCollectionView();
        }

        private void OnEnhancedDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SetupCollectionView();
        }

        #endregion

        #region Event Handlers

        private static void OnShowFiltersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EnhancedCustomDataGrid dataGrid)
            {
                dataGrid.UpdateFilterVisibility();
            }
        }

        private static void OnShowGroupingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EnhancedCustomDataGrid dataGrid)
            {
                dataGrid.UpdateGroupingVisibility();
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            // Manejar atajos de teclado adicionales
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.F:
                        ToggleFilters();
                        e.Handled = true;
                        return;
                    case Key.G:
                        ToggleGrouping();
                        e.Handled = true;
                        return;
                }
            }

            // Llamar al manejo base
            base.OnPreviewKeyDown(e);
        }

        #endregion

        #region Filtering Implementation

        private void SetupCollectionView()
        {
            if (ItemsSource != null)
            {
                _collectionView = CollectionViewSource.GetDefaultView(ItemsSource);
                ApplyFilters();
                UpdateGrouping();
            }
        }

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

            // También limpiar filtros visuales
            ClearFilterBoxes();
        }

        private void ApplyFilters()
        {
            if (_collectionView != null)
            {
                _collectionView.Filter = item =>
                {
                    // Aplicar filtro personalizado primero
                    //if (OnFilter != null && !OnFilter(item))
                    //    return false;

                    // Aplicar filtros de columnas
                    foreach (var filter in _columnFilters)
                    {
                        if (!ApplyColumnFilter(item, filter.Key, filter.Value))
                            return false;
                    }

                    return true;
                };

                _collectionView.Refresh();
            }
        }

        private bool ApplyColumnFilter(object item, string columnName, string filter)
        {
            try
            {
                // Buscar la columna correspondiente
                var column = Columns.FirstOrDefault(c =>
                    c.SortMemberPath == columnName ||
                    c.Header?.ToString() == columnName);

                if (column == null) return true;

                // Si existe el método FilterColumn de la clase base, usarlo
                //if (this.OnFilter != null)
                //{
                //    return this.OnFilter(item);
                //}

                // Fallback: filtrado básico por propiedad
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

        private void ClearFilterBoxes()
        {
            // Usar el método heredado de CustomDataGrid si existe
            try
            {
                //this.ResetFilters();
            }
            catch
            {
                // Si no existe el método, limpiar manualmente
                _columnFilters.Clear();
                ApplyFilters();
            }
        }

        #endregion

        #region Grouping Implementation

        public void SetGrouping(string propertyName)
        {
            if (_collectionView != null)
            {
                _collectionView.GroupDescriptions.Clear();
                if (!string.IsNullOrEmpty(propertyName))
                {
                    _collectionView.GroupDescriptions.Add(new PropertyGroupDescription(propertyName));
                }
            }
        }

        public void ClearGrouping()
        {
            _collectionView?.GroupDescriptions.Clear();
        }

        private void UpdateGrouping()
        {
            if (!ShowGrouping)
            {
                ClearGrouping();
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

        private void UpdateGroupingVisibility()
        {
            if (ShowGrouping)
            {
                // Aplicar GroupStyle via código
                try
                {
                    GroupStyle.Clear();
                    if (FindResource("DefaultGroupStyle") is GroupStyle groupStyle)
                    {
                        GroupStyle.Add(groupStyle);
                    }
                    else
                    {
                        // Crear GroupStyle por defecto si no encuentra el recurso
                        var defaultGroupStyle = new GroupStyle();
                        defaultGroupStyle.Panel = new ItemsPanelTemplate();
                        defaultGroupStyle.Panel.VisualTree = new FrameworkElementFactory(typeof(DataGridRowsPresenter));
                        GroupStyle.Add(defaultGroupStyle);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error aplicando GroupStyle: {ex.Message}");
                }
            }
            else
            {
                GroupStyle.Clear();
            }

            // Aplicar estilo con agrupación si está habilitada
            try
            {
                if (ShowGrouping && FindResource("GroupingDataGridStyle") is Style groupStyle)
                {
                    Style = groupStyle;
                }
                else if (FindResource("EnhancedDataGridStyle") is Style normalStyle)
                {
                    Style = normalStyle;
                }
            }
            catch
            {
                // Si no encuentra los estilos, mantener el actual
            }
        }

        #endregion

        #region Copy/Paste Implementation

        public new void CopySelectedCells()
        {
            //if ((Permission & Permission.Copy) == 0) return;

            try
            {
                // Usar la implementación existente de CustomDataGrid
                //this.CopyingFromSignalDataGrid();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en CopySelectedCells: {ex.Message}");
            }
        }

        public new void PasteToSelectedCells()
        {
            //if ((Permission & Permission.Paste) == 0) return;

            try
            {
                var clipboardText = Clipboard.GetText();
                if (!string.IsNullOrEmpty(clipboardText))
                {
                    // Usar la implementación existente de CustomDataGrid
                    //this.PastingToSignalDataGrid(clipboardText);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en PasteToSelectedCells: {ex.Message}");
            }
        }

        #endregion

        #region Command Methods

        private void ToggleFilters()
        {
            ShowFilters = !ShowFilters;
        }

        private void ToggleGrouping()
        {
            ShowGrouping = !ShowGrouping;
        }

        #endregion

        #region Context Menu Override

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            // Usar el manejo base de CustomDataGrid que ya tiene toda la lógica
            base.OnContextMenuOpening(e);

            // Agregar elementos adicionales si es necesario
            if (ContextMenu != null)
            {
                // Agregar separador y opciones adicionales
                ContextMenu.Items.Add(new Separator());

                var filterMenuItem = new MenuItem
                {
                    Header = "Filtros",
                    IsCheckable = true,
                    IsChecked = ShowFilters
                };
                filterMenuItem.Click += (s, args) => ShowFilters = !ShowFilters;
                ContextMenu.Items.Add(filterMenuItem);

                var groupMenuItem = new MenuItem
                {
                    Header = "Agrupación",
                    IsCheckable = true,
                    IsChecked = ShowGrouping
                };
                groupMenuItem.Click += (s, args) => ShowGrouping = !ShowGrouping;
                ContextMenu.Items.Add(groupMenuItem);
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
        /// Configura agrupación por una propiedad específica
        /// </summary>
        public void GroupBy(string propertyName)
        {
            ShowGrouping = true;
            SetGrouping(propertyName);
        }

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