using Axon.UI.Components.Base;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Axon.UI.Components
{
    /// <summary>
    /// Control personalizado de RadioButton group
    /// </summary>
    [ContentProperty("Items")]
    public partial class AxonRadioButton : UserControl, INotifyPropertyChanged
    {
        public AxonRadioButton()
        {
            InitializeComponent();
            Items = new ObservableCollection<AxonRadioItem>();
            Items.CollectionChanged += Items_CollectionChanged;
        }

        #region Dependency Properties

        /// <summary>
        /// Colección de items del radio button
        /// </summary>
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(nameof(Items), typeof(ObservableCollection<AxonRadioItem>),
                typeof(AxonRadioButton), new PropertyMetadata(null));

        public ObservableCollection<AxonRadioItem> Items
        {
            get => (ObservableCollection<AxonRadioItem>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        /// <summary>
        /// Texto del label superior
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(AxonRadioButton),
                new PropertyMetadata("Opciones"));

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        /// <summary>
        /// Valor seleccionado (bindeable)
        /// </summary>
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(nameof(SelectedValue), typeof(object), typeof(AxonRadioButton),
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
            DependencyProperty.RegisterReadOnly(nameof(SelectedItem), typeof(AxonRadioItem), typeof(AxonRadioButton),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedItemProperty = SelectedItemPropertyKey.DependencyProperty;

        public AxonRadioItem SelectedItem
        {
            get => (AxonRadioItem)GetValue(SelectedItemProperty);
            private set => SetValue(SelectedItemPropertyKey, value);
        }

        /// <summary>
        /// Índice del item seleccionado
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(AxonRadioButton),
                new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedIndexChanged));

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        /// <summary>
        /// Nombre del grupo (para agrupar radio buttons)
        /// </summary>
        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register(nameof(GroupName), typeof(string), typeof(AxonRadioButton),
                new PropertyMetadata(null));

        public string GroupName
        {
            get => (string)GetValue(GroupNameProperty);
            set => SetValue(GroupNameProperty, value);
        }

        /// <summary>
        /// Orientación de los items (Vertical u Horizontal)
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(AxonRadioButton),
                new PropertyMetadata(Orientation.Vertical));

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        /// <summary>
        /// Controla si el control está habilitado
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(AxonRadioButton),
                new PropertyMetadata(true));

        public new bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<RadioSelectionChangedEventArgs> SelectionChanged;

        #endregion

        #region Event Handlers

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Cuando se agregan items, asignar el comando de selección
            if (e.NewItems != null)
            {
                foreach (AxonRadioItem item in e.NewItems)
                {
                    item.SelectCommand = new DelegateCommand<AxonRadioItem>(SelectItem);
                    item.GroupName = GroupName ?? GetHashCode().ToString();
                }
            }

            // Si no hay selección y hay items, seleccionar el primero
            if (Items.Count > 0 && SelectedIndex < 0)
            {
                SelectedIndex = 0;
            }
        }

        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AxonRadioButton control)
            {
                control.UpdateSelectionFromValue(e.NewValue);
            }
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AxonRadioButton control)
            {
                control.UpdateSelectionFromIndex((int)e.NewValue);
            }
        }

        #endregion

        #region Private Methods

        private void SelectItem(AxonRadioItem item)
        {
            if (item == null || !IsEnabled) return;

            var oldIndex = SelectedIndex;
            var oldItem = SelectedItem;
            var oldValue = SelectedValue;

            var newIndex = Items.IndexOf(item);
            if (newIndex >= 0)
            {
                // Actualizar selecciones
                UpdateItemSelections(item);

                SelectedIndex = newIndex;
                SelectedItem = item;
                SelectedValue = item.Value ?? item.Text;

                // Disparar evento
                SelectionChanged?.Invoke(this, new RadioSelectionChangedEventArgs
                {
                    OldIndex = oldIndex,
                    NewIndex = newIndex,
                    OldItem = oldItem,
                    NewItem = item,
                    OldValue = oldValue,
                    NewValue = SelectedValue
                });
            }
        }

        private void UpdateSelectionFromValue(object value)
        {
            if (Items == null) return;

            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                var itemValue = item.Value ?? item.Text;

                if (Equals(itemValue, value))
                {
                    if (SelectedIndex != i)
                    {
                        SelectedIndex = i;
                    }
                    return;
                }
            }
        }

        private void UpdateSelectionFromIndex(int index)
        {
            if (Items == null || index < 0 || index >= Items.Count) return;

            var item = Items[index];
            UpdateItemSelections(item);

            SelectedItem = item;
            SelectedValue = item.Value ?? item.Text;
        }

        private void UpdateItemSelections(AxonRadioItem selectedItem)
        {
            foreach (var item in Items)
            {
                item.IsSelected = item == selectedItem;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Agrega un item al radio button group
        /// </summary>
        public void AddItem(string text, object value = null)
        {
            Items.Add(new AxonRadioItem { Text = text, Value = value });
        }

        /// <summary>
        /// Remueve un item por índice
        /// </summary>
        public void RemoveItem(int index)
        {
            if (index >= 0 && index < Items.Count)
            {
                Items.RemoveAt(index);
            }
        }

        /// <summary>
        /// Limpia todos los items
        /// </summary>
        public void Clear()
        {
            Items.Clear();
        }

        #endregion
    }

    #region Helper Classes

    /// <summary>
    /// Representa un item individual del radio button
    /// </summary>
    public class AxonRadioItem : INotifyPropertyChanged
    {
        private string _text;
        private object _value;
        private bool _isSelected;
        private bool _isEnabled = true;
        private string _groupName;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        public object Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public string GroupName
        {
            get => _groupName;
            set
            {
                _groupName = value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Argumentos para el evento SelectionChanged
    /// </summary>
    public class RadioSelectionChangedEventArgs : EventArgs
    {
        public int OldIndex { get; set; }
        public int NewIndex { get; set; }
        public AxonRadioItem OldItem { get; set; }
        public AxonRadioItem NewItem { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
    }

    #endregion
}