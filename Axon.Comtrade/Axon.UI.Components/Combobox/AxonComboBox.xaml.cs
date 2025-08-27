using Axon.UI.Components.Base;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Axon.UI.Components
{
    /// <summary>
    /// Control personalizado ComboBox con estilo Microsoft Fluent 2
    /// </summary>
    [ContentProperty("Items")]
    public partial class AxonComboBox : UserControl, INotifyPropertyChanged
    {
        private bool _isDropDownOpen = false;

        public AxonComboBox()
        {
            InitializeComponent();
            Items = new ObservableCollection<AxonComboBoxItem>();
            Groups = new ObservableCollection<AxonComboBoxGroup>();

            Items.CollectionChanged += Items_CollectionChanged;
            Groups.CollectionChanged += Groups_CollectionChanged;

            // Inicializar comando inmediatamente
            ToggleDropDownCommand = new DelegateCommand(ToggleDropDown);

            Loaded += OnLoaded;
        }

        #region Dependency Properties

        /// <summary>
        /// ItemsSource para binding con colecciones del ViewModel (como ComboBox nativo)
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(System.Collections.IEnumerable),
                typeof(AxonComboBox), new PropertyMetadata(null, OnItemsSourceChanged));

        public System.Collections.IEnumerable ItemsSource
        {
            get => (System.Collections.IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        /// <summary>
        /// Propiedad que se usa para mostrar el texto de cada item
        /// </summary>
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(nameof(DisplayMemberPath), typeof(string),
                typeof(AxonComboBox), new PropertyMetadata("", OnDisplayMemberPathChanged));

        public string DisplayMemberPath
        {
            get => (string)GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }

        /// <summary>
        /// Propiedad que se usa para obtener el valor de cada item
        /// </summary>
        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register(nameof(SelectedValuePath), typeof(string),
                typeof(AxonComboBox), new PropertyMetadata(""));

        public string SelectedValuePath
        {
            get => (string)GetValue(SelectedValuePathProperty);
            set => SetValue(SelectedValuePathProperty, value);
        }

        /// <summary>
        /// Item seleccionado del ItemsSource (como ComboBox nativo)
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object),
                typeof(AxonComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemChanged));

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        /// <summary>
        /// Texto del label superior
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(AxonComboBox),
                new PropertyMetadata(""));

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        /// <summary>
        /// Texto placeholder cuando no hay selección
        /// </summary>
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(AxonComboBox),
                new PropertyMetadata("Seleccionar..."));

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        /// <summary>
        /// Valor seleccionado (bindeable)
        /// </summary>
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(nameof(SelectedValue), typeof(object), typeof(AxonComboBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedValueChanged));

        public object SelectedValue
        {
            get => GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value);
        }

        /// <summary>
        /// Índice del item seleccionado
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(AxonComboBox),
                new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedIndexChanged));

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        /// <summary>
        /// Determina si se muestran los grupos
        /// </summary>
        public static readonly DependencyProperty ShowGroupsProperty =
            DependencyProperty.Register(nameof(ShowGroups), typeof(bool), typeof(AxonComboBox),
                new PropertyMetadata(false));

        public bool ShowGroups
        {
            get => (bool)GetValue(ShowGroupsProperty);
            set => SetValue(ShowGroupsProperty, value);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Colección de items del combobox (para declaración manual)
        /// </summary>
        public ObservableCollection<AxonComboBoxItem> Items { get; private set; }

        /// <summary>
        /// Colección de grupos del combobox (para declaración manual)
        /// </summary>
        public ObservableCollection<AxonComboBoxGroup> Groups { get; private set; }

        /// <summary>
        /// Controla si el dropdown está abierto
        /// </summary>
        public bool IsDropDownOpen
        {
            get => _isDropDownOpen;
            set
            {
                if (_isDropDownOpen != value)
                {
                    _isDropDownOpen = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DropDownVisibility));
                }
            }
        }

        /// <summary>
        /// Visibilidad del dropdown para binding
        /// </summary>
        public Visibility DropDownVisibility => IsDropDownOpen ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Texto que se muestra en el combobox
        /// </summary>
        public string DisplayText
        {
            get
            {
                if (SelectedItem != null)
                {
                    return GetDisplayText(SelectedItem);
                }
                return Placeholder;
            }
        }

        /// <summary>
        /// Items para mostrar en el dropdown
        /// </summary>
        public object DisplayItems => ShowGroups ? (object)Groups : (object)Items;

        /// <summary>
        /// Comando para toggle del dropdown
        /// </summary>
        public ICommand ToggleDropDownCommand { get; private set; }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<ComboSelectionChangedEventArgs> SelectionChanged;

        #endregion

        #region Event Handlers

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Click fuera para cerrar dropdown
            if (Application.Current?.MainWindow != null)
            {
                Application.Current.MainWindow.PreviewMouseDown += OnWindowPreviewMouseDown;
            }
        }

        private void OnWindowPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsDropDownOpen)
            {
                var hitTest = e.OriginalSource as DependencyObject;
                if (!IsDescendantOf(hitTest, this))
                {
                    IsDropDownOpen = false;
                }
            }
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AxonComboBox control)
            {
                control.RefreshItemsFromSource();
            }
        }

        private static void OnDisplayMemberPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AxonComboBox control)
            {
                control.RefreshItemsFromSource();
            }
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AxonComboBox control)
            {
                control.UpdateFromSelectedItem(e.NewValue);
            }
        }

        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AxonComboBox control)
            {
                control.UpdateFromSelectedValue(e.NewValue);
            }
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AxonComboBox control)
            {
                control.UpdateFromSelectedIndex((int)e.NewValue);
            }
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (AxonComboBoxItem item in e.NewItems)
                {
                    item.SelectCommand = new DelegateCommand<AxonComboBoxItem>(SelectWrapperItem);
                }
            }
        }

        private void Groups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (AxonComboBoxGroup group in e.NewItems)
                {
                    foreach (var item in group.Items)
                    {
                        item.SelectCommand = new DelegateCommand<AxonComboBoxItem>(SelectWrapperItem);
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private void ToggleDropDown()
        {
            IsDropDownOpen = !IsDropDownOpen;
        }

        private void RefreshItemsFromSource()
        {
            Items.Clear();

            if (ItemsSource != null)
            {
                foreach (var sourceItem in ItemsSource)
                {
                    var comboItem = new AxonComboBoxItem
                    {
                        Text = GetDisplayText(sourceItem),
                        Value = GetItemValue(sourceItem),
                        SourceItem = sourceItem,
                        SelectCommand = new DelegateCommand<AxonComboBoxItem>(SelectWrapperItem)
                    };

                    Items.Add(comboItem);
                }

                // Restaurar selección si había una
                if (SelectedItem != null)
                {
                    UpdateFromSelectedItem(SelectedItem);
                }
            }

            OnPropertyChanged(nameof(DisplayText));
        }

        private string GetDisplayText(object item)
        {
            if (item == null) return "";

            if (string.IsNullOrEmpty(DisplayMemberPath))
                return item.ToString();

            try
            {
                var property = item.GetType().GetProperty(DisplayMemberPath);
                return property?.GetValue(item)?.ToString() ?? item.ToString();
            }
            catch
            {
                return item.ToString();
            }
        }

        private object GetItemValue(object item)
        {
            if (item == null) return null;

            if (string.IsNullOrEmpty(SelectedValuePath))
                return item;

            try
            {
                var property = item.GetType().GetProperty(SelectedValuePath);
                return property?.GetValue(item) ?? item;
            }
            catch
            {
                return item;
            }
        }

        private void UpdateFromSelectedItem(object selectedItem)
        {
            if (selectedItem == null)
            {
                UpdateAllItemsSelection(null);
                SelectedValue = null;
                OnPropertyChanged(nameof(DisplayText));
                return;
            }

            // Buscar el wrapper item que corresponde al item seleccionado
            var wrapperItem = Items.FirstOrDefault(i => Equals(i.SourceItem ?? i, selectedItem));
            if (wrapperItem != null)
            {
                UpdateAllItemsSelection(wrapperItem);
                SelectedValue = wrapperItem.Value;
            }

            OnPropertyChanged(nameof(DisplayText));
        }

        private void UpdateFromSelectedValue(object value)
        {
            if (ItemsSource != null && value != null)
            {
                // Buscar en ItemsSource
                foreach (var sourceItem in ItemsSource)
                {
                    var itemValue = GetItemValue(sourceItem);
                    if (Equals(itemValue, value))
                    {
                        SelectedItem = sourceItem;
                        return;
                    }
                }
            }
            else
            {
                // Buscar en Items manuales
                var item = Items.FirstOrDefault(i => Equals(i.Value, value));
                if (item != null)
                {
                    UpdateAllItemsSelection(item);
                    SelectedItem = item.SourceItem ?? item;
                    OnPropertyChanged(nameof(DisplayText));
                }
            }
        }

        private void UpdateFromSelectedIndex(int index)
        {
            if (index >= 0 && index < Items.Count)
            {
                var item = Items[index];
                SelectedItem = item.SourceItem ?? item;
                SelectedValue = item.Value;
            }
        }

        private void SelectWrapperItem(AxonComboBoxItem wrapperItem)
        {
            if (wrapperItem == null) return;

            var oldValue = SelectedValue;
            var oldItem = SelectedItem;

            UpdateAllItemsSelection(wrapperItem);

            SelectedItem = wrapperItem.SourceItem ?? wrapperItem;
            SelectedValue = wrapperItem.Value;
            SelectedIndex = Items.IndexOf(wrapperItem);

            OnPropertyChanged(nameof(DisplayText));

            IsDropDownOpen = false;

            SelectionChanged?.Invoke(this, new ComboSelectionChangedEventArgs
            {
                OldValue = oldValue,
                NewValue = SelectedValue,
                OldItem = oldItem,
                NewItem = SelectedItem
            });
        }

        private void UpdateAllItemsSelection(AxonComboBoxItem selectedItem)
        {
            // Actualizar items sin agrupar
            foreach (var item in Items)
            {
                item.IsSelected = item == selectedItem;
            }

            // Actualizar items en grupos
            foreach (var group in Groups)
            {
                foreach (var item in group.Items)
                {
                    item.IsSelected = item == selectedItem;
                }
            }
        }

        private bool IsDescendantOf(DependencyObject child, DependencyObject parent)
        {
            if (child == null || parent == null) return false;

            var current = child;
            while (current != null)
            {
                if (current == parent) return true;
                current = VisualTreeHelper.GetParent(current) ?? System.Windows.LogicalTreeHelper.GetParent(current);
            }
            return false;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Public Methods

        public void AddItem(string text, object value = null)
        {
            var item = new AxonComboBoxItem
            {
                Text = text,
                Value = value,
                SelectCommand = new DelegateCommand<AxonComboBoxItem>(SelectWrapperItem)
            };
            Items.Add(item);
        }

        public void AddGroup(string header, params AxonComboBoxItem[] items)
        {
            var group = new AxonComboBoxGroup { Header = header };
            foreach (var item in items)
            {
                item.SelectCommand = new DelegateCommand<AxonComboBoxItem>(SelectWrapperItem);
                group.Items.Add(item);
            }
            Groups.Add(group);
        }

        public void Clear()
        {
            Items.Clear();
            Groups.Clear();
            SelectedIndex = -1;
            SelectedItem = null;
            SelectedValue = null;
        }

        #endregion

        private void ToggleDropDown_click(object sender, RoutedEventArgs e)
        {
            ToggleDropDown();
        }
    }

    #region Helper Classes

    public class AxonComboBoxItem : INotifyPropertyChanged
    {
        private string _text;
        private object _value;
        private bool _isEnabled = true;
        private bool _isSelected = false;
        private object _sourceItem;

        public string Text
        {
            get => _text;
            set { _text = value; OnPropertyChanged(); }
        }

        public object Value
        {
            get => _value;
            set { _value = value; OnPropertyChanged(); }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set { _isEnabled = value; OnPropertyChanged(); }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Referencia al item original del ItemsSource
        /// </summary>
        public object SourceItem
        {
            get => _sourceItem;
            set { _sourceItem = value; OnPropertyChanged(); }
        }

        public ICommand SelectCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return Text;
        }
    }

    [ContentProperty("Items")]
    public class AxonComboBoxGroup : INotifyPropertyChanged
    {
        private string _header;

        public AxonComboBoxGroup()
        {
            Items = new ObservableCollection<AxonComboBoxItem>();
        }

        public string Header
        {
            get => _header;
            set { _header = value; OnPropertyChanged(); }
        }

        public ObservableCollection<AxonComboBoxItem> Items { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ComboSelectionChangedEventArgs : EventArgs
    {
        public object OldValue { get; set; }
        public object NewValue { get; set; }
        public object OldItem { get; set; }
        public object NewItem { get; set; }
    }

    #endregion
}