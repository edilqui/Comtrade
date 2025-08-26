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
using System.Windows.Media.Animation;

namespace Axon.UI.Components
{
    /// <summary>
    /// Control personalizado ComboBox con estilo Microsoft Fluent 2
    /// </summary>
    [ContentProperty("Items")]
    public partial class AxonComboBox : UserControl, INotifyPropertyChanged
    {
        private bool _isDropDownOpen = false;
        private Storyboard _openAnimation;
        private Storyboard _closeAnimation;

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
        /// Colección de items del combobox
        /// </summary>
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(nameof(Items), typeof(ObservableCollection<AxonComboBoxItem>),
                typeof(AxonComboBox), new PropertyMetadata(null));

        public ObservableCollection<AxonComboBoxItem> Items
        {
            get => (ObservableCollection<AxonComboBoxItem>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        /// <summary>
        /// Colección de grupos del combobox
        /// </summary>
        public static readonly DependencyProperty GroupsProperty =
            DependencyProperty.Register(nameof(Groups), typeof(ObservableCollection<AxonComboBoxGroup>),
                typeof(AxonComboBox), new PropertyMetadata(null));

        public ObservableCollection<AxonComboBoxGroup> Groups
        {
            get => (ObservableCollection<AxonComboBoxGroup>)GetValue(GroupsProperty);
            set => SetValue(GroupsProperty, value);
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
        /// Item seleccionado (readonly)
        /// </summary>
        private static readonly DependencyPropertyKey SelectedItemPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(SelectedItem), typeof(AxonComboBoxItem), typeof(AxonComboBox),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedItemProperty = SelectedItemPropertyKey.DependencyProperty;

        public AxonComboBoxItem SelectedItem
        {
            get => (AxonComboBoxItem)GetValue(SelectedItemProperty);
            private set => SetValue(SelectedItemPropertyKey, value);
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
                new PropertyMetadata(false, OnShowGroupsChanged));

        public bool ShowGroups
        {
            get => (bool)GetValue(ShowGroupsProperty);
            set => SetValue(ShowGroupsProperty, value);
        }

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

                    if (value)
                        ShowDropDown();
                    else
                        HideDropDown();
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
        public string DisplayText => SelectedItem?.Text ?? Placeholder;

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
            InitializeAnimations();
            ToggleDropDownCommand = new DelegateCommand(ToggleDropDown);

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

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (AxonComboBoxItem item in e.NewItems)
                {
                    item.SelectCommand = new DelegateCommand<AxonComboBoxItem>(SelectItem);
                }
            }

            UpdateDisplayItems();
        }

        private void Groups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (AxonComboBoxGroup group in e.NewItems)
                {
                    group.Items.CollectionChanged += GroupItems_CollectionChanged;
                    foreach (var item in group.Items)
                    {
                        item.SelectCommand = new DelegateCommand<AxonComboBoxItem>(SelectItem);
                    }
                }
            }

            UpdateDisplayItems();
        }

        private void GroupItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (AxonComboBoxItem item in e.NewItems)
                {
                    item.SelectCommand = new DelegateCommand<AxonComboBoxItem>(SelectItem);
                }
            }

            UpdateDisplayItems();
        }

        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AxonComboBox control)
            {
                control.UpdateSelectionFromValue(e.NewValue);
            }
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AxonComboBox control)
            {
                control.UpdateSelectionFromIndex((int)e.NewValue);
            }
        }

        private static void OnShowGroupsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AxonComboBox control)
            {
                control.UpdateDisplayItems();
            }
        }

        #endregion

        #region Private Methods

        private void InitializeAnimations()
        {
            // Animación de apertura
            _openAnimation = new Storyboard();
            var openOpacity = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(150));
            var openTransform = new DoubleAnimation(-10, 0, TimeSpan.FromMilliseconds(200))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(openOpacity, FindName("DropDownBorder") as DependencyObject);
            Storyboard.SetTargetProperty(openOpacity, new PropertyPath("Opacity"));
            Storyboard.SetTarget(openTransform, FindName("DropDownTransform") as DependencyObject);
            Storyboard.SetTargetProperty(openTransform, new PropertyPath("Y"));

            _openAnimation.Children.Add(openOpacity);
            _openAnimation.Children.Add(openTransform);

            // Animación de cierre
            _closeAnimation = new Storyboard();
            var closeOpacity = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(100));
            var closeTransform = new DoubleAnimation(0, -5, TimeSpan.FromMilliseconds(100));

            Storyboard.SetTarget(closeOpacity, FindName("DropDownBorder") as DependencyObject);
            Storyboard.SetTargetProperty(closeOpacity, new PropertyPath("Opacity"));
            Storyboard.SetTarget(closeTransform, FindName("DropDownTransform") as DependencyObject);
            Storyboard.SetTargetProperty(closeTransform, new PropertyPath("Y"));

            _closeAnimation.Children.Add(closeOpacity);
            _closeAnimation.Children.Add(closeTransform);

            _closeAnimation.Completed += (s, e) =>
            {
                OnPropertyChanged(nameof(DropDownVisibility));
            };
        }

        private void ToggleDropDown()
        {
            IsDropDownOpen = !IsDropDownOpen;
        }

        private void ShowDropDown()
        {
            OnPropertyChanged(nameof(DropDownVisibility));
            _openAnimation?.Begin(this);
        }

        private void HideDropDown()
        {
            _closeAnimation?.Begin(this);
        }

        private void SelectItem(AxonComboBoxItem item)
        {
            if (item == null) return;

            var oldValue = SelectedValue;
            var oldItem = SelectedItem;
            var oldIndex = SelectedIndex;

            SelectedItem = item;
            SelectedValue = item.Value ?? item.Text;

            // Encontrar el índice en la lista plana
            var allItems = GetAllItems();
            SelectedIndex = allItems.IndexOf(item);

            OnPropertyChanged(nameof(DisplayText));

            IsDropDownOpen = false;

            SelectionChanged?.Invoke(this, new ComboSelectionChangedEventArgs
            {
                OldValue = oldValue,
                NewValue = SelectedValue,
                OldItem = oldItem,
                NewItem = item,
                OldIndex = oldIndex,
                NewIndex = SelectedIndex
            });
        }

        private void UpdateSelectionFromValue(object value)
        {
            var allItems = GetAllItems();
            var item = allItems.FirstOrDefault(i => Equals(i.Value ?? i.Text, value));

            if (item != null)
            {
                SelectedItem = item;
                SelectedIndex = allItems.IndexOf(item);
                OnPropertyChanged(nameof(DisplayText));
            }
        }

        private void UpdateSelectionFromIndex(int index)
        {
            var allItems = GetAllItems();
            if (index >= 0 && index < allItems.Count)
            {
                var item = allItems[index];
                SelectedItem = item;
                SelectedValue = item.Value ?? item.Text;
                OnPropertyChanged(nameof(DisplayText));
            }
        }

        private void UpdateDisplayItems()
        {
            OnPropertyChanged(nameof(DisplayItems));
        }

        private ObservableCollection<AxonComboBoxItem> GetAllItems()
        {
            var result = new ObservableCollection<AxonComboBoxItem>();

            if (ShowGroups)
            {
                foreach (var group in Groups)
                {
                    foreach (var item in group.Items)
                    {
                        result.Add(item);
                    }
                }
            }
            else
            {
                foreach (var item in Items)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        private bool IsDescendantOf(DependencyObject child, DependencyObject parent)
        {
            if (child == null || parent == null) return false;

            var current = child;
            while (current != null)
            {
                if (current == parent) return true;
                current = VisualTreeHelper.GetParent(current) ?? LogicalTreeHelper.GetParent(current);
            }
            return false;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Public Properties for Binding

        public object DisplayItems
        {
            get
            {
                return ShowGroups ? (object)Groups : (object)Items;
            }
        }

        #endregion

        #region Public Methods

        public void AddItem(string text, object value = null)
        {
            Items.Add(new AxonComboBoxItem { Text = text, Value = value });
        }

        public void AddGroup(string header, params AxonComboBoxItem[] items)
        {
            var group = new AxonComboBoxGroup { Header = header };
            foreach (var item in items)
            {
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

        public ICommand SelectCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
        public AxonComboBoxItem OldItem { get; set; }
        public AxonComboBoxItem NewItem { get; set; }
        public int OldIndex { get; set; }
        public int NewIndex { get; set; }
    }

    #endregion
}