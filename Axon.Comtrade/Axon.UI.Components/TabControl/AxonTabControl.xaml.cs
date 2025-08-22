using Axon.UI.Components.Base;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Axon.UI.Components
{
    [ContentProperty("Items")]
    public partial class AxonTabControl : UserControl, INotifyPropertyChanged
    {
        private Storyboard _indicatorAnimation;
        private bool _isLoaded = false;

        public AxonTabControl()
        {
            InitializeComponent();
            Items = new ObservableCollection<AxonTabItem>();
            SelectTabCommand = new DelegateCommand<AxonTabItem>(SelectTab);

            Items.CollectionChanged += Items_CollectionChanged;
            Loaded += OnLoaded;
            SizeChanged += OnSizeChanged;
        }

        #region Dependency Properties

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(nameof(Items), typeof(ObservableCollection<AxonTabItem>),
                typeof(AxonTabControl), new PropertyMetadata(null, OnItemsChanged));

        public ObservableCollection<AxonTabItem> Items
        {
            get => (ObservableCollection<AxonTabItem>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AxonTabControl control)
            {
                if (e.OldValue is ObservableCollection<AxonTabItem> oldItems)
                {
                    oldItems.CollectionChanged -= control.Items_CollectionChanged;
                }

                if (e.NewValue is ObservableCollection<AxonTabItem> newItems)
                {
                    newItems.CollectionChanged += control.Items_CollectionChanged;
                }
            }
        }

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(AxonTabControl),
                new PropertyMetadata(-1, OnSelectedIndexChanged));

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        private static readonly DependencyPropertyKey SelectedTabItemPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(SelectedTabItem), typeof(AxonTabItem), typeof(AxonTabControl),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedTabItemProperty = SelectedTabItemPropertyKey.DependencyProperty;

        public AxonTabItem SelectedTabItem
        {
            get => (AxonTabItem)GetValue(SelectedTabItemProperty);
            private set => SetValue(SelectedTabItemPropertyKey, value);
        }

        #endregion

        #region Properties

        [Obsolete("Use Items property instead")]
        public ObservableCollection<AxonTabItem> TabItems => Items;

        public ICommand SelectTabCommand { get; private set; }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<TabSelectionChangedEventArgs> SelectionChanged;

        #endregion

        #region Event Handlers

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Items.Count > 0 && SelectedIndex < 0)
            {
                SelectedIndex = 0;
            }
            else if (SelectedIndex >= Items.Count)
            {
                SelectedIndex = Math.Max(0, Items.Count - 1);
            }

            // Actualizar indicador después de que el layout se complete
            InvalidateVisual();
            Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateIndicatorPosition(false);
            }), DispatcherPriority.Loaded);
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AxonTabControl control)
            {
                control.UpdateSelectedTab();
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
            _indicatorAnimation = (Storyboard)Resources["IndicatorMoveAnimation"];

            if (Items.Count > 0 && SelectedIndex < 0)
            {
                SelectedIndex = 0;
            }

            // Múltiples intentos para asegurar que el indicador se posicione correctamente
            Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateIndicatorPosition(false);
            }), DispatcherPriority.Loaded);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateIndicatorPosition(false);
            }), DispatcherPriority.ApplicationIdle);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_isLoaded)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    UpdateIndicatorPosition(false);
                }), DispatcherPriority.ApplicationIdle);
            }
        }

        #endregion

        #region Private Methods

        private void SelectTab(AxonTabItem tabItem)
        {
            if (tabItem == null || !tabItem.IsEnabled) return;

            var oldIndex = SelectedIndex;
            var newIndex = Items.IndexOf(tabItem);

            if (newIndex >= 0 && newIndex != oldIndex)
            {
                SelectedIndex = newIndex;

                SelectionChanged?.Invoke(this, new TabSelectionChangedEventArgs
                {
                    OldIndex = oldIndex,
                    NewIndex = newIndex,
                    OldTabItem = oldIndex >= 0 && oldIndex < Items.Count ? Items[oldIndex] : null,
                    NewTabItem = tabItem
                });
            }
        }

        private void UpdateSelectedTab()
        {
            foreach (var tab in Items)
            {
                tab.IsSelected = false;
            }

            if (SelectedIndex >= 0 && SelectedIndex < Items.Count)
            {
                var selectedTab = Items[SelectedIndex];
                selectedTab.IsSelected = true;
                SelectedTabItem = selectedTab;
            }
            else
            {
                SelectedTabItem = null;
            }

            // Forzar actualización del layout antes de mover el indicador
            UpdateLayout();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateIndicatorPosition(true);
            }), DispatcherPriority.Render);

            OnPropertyChanged(nameof(SelectedTabItem));
        }

        private void UpdateIndicatorPosition(bool animate)
        {
            if (!_isLoaded || SelectedIndex < 0 || SelectedIndex >= Items.Count)
            {
                if (SelectedIndicator != null)
                {
                    SelectedIndicator.Visibility = Visibility.Hidden;
                }
                return;
            }

            if (SelectedIndicator != null)
            {
                SelectedIndicator.Visibility = Visibility.Visible;
            }

            var containerPanel = GetTabHeadersPanel();
            if (containerPanel == null)
            {
                // Reintentar después de un breve delay
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    UpdateIndicatorPosition(animate);
                }), DispatcherPriority.ApplicationIdle);
                return;
            }

            var targetButton = GetTabButton(SelectedIndex);
            if (targetButton == null || targetButton.ActualWidth == 0)
            {
                // Si el botón no está renderizado aún, reintentar
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    UpdateIndicatorPosition(animate);
                }), DispatcherPriority.ApplicationIdle);
                return;
            }

            try
            {
                var buttonPosition = targetButton.TranslatePoint(new Point(0, 0), containerPanel);
                var targetLeft = buttonPosition.X;
                var targetWidth = targetButton.ActualWidth;

                System.Diagnostics.Debug.WriteLine($"Moviendo indicador: Left={targetLeft}, Width={targetWidth}, Index={SelectedIndex}");

                if (animate && _indicatorAnimation != null && targetLeft >= 0)
                {
                    var moveAnimation = _indicatorAnimation.Children.OfType<DoubleAnimation>()
                        .FirstOrDefault(a => a.Name == "IndicatorMoveX");
                    var resizeAnimation = _indicatorAnimation.Children.OfType<DoubleAnimation>()
                        .FirstOrDefault(a => a.Name == "IndicatorResizeWidth");

                    if (moveAnimation != null)
                    {
                        var currentLeft = Canvas.GetLeft(SelectedIndicator);
                        if (double.IsNaN(currentLeft)) currentLeft = 0;

                        moveAnimation.From = currentLeft;
                        moveAnimation.To = targetLeft;
                    }

                    if (resizeAnimation != null)
                    {
                        var currentWidth = SelectedIndicator.Width;
                        if (double.IsNaN(currentWidth) || currentWidth == 0) currentWidth = 80;

                        resizeAnimation.From = currentWidth;
                        resizeAnimation.To = targetWidth;
                    }

                    _indicatorAnimation.Begin();
                }
                else
                {
                    Canvas.SetLeft(SelectedIndicator, targetLeft);
                    SelectedIndicator.Width = targetWidth;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al actualizar indicador: {ex.Message}");
                // En caso de error, intentar posicionamiento directo
                Canvas.SetLeft(SelectedIndicator, 0);
                SelectedIndicator.Width = 80;
            }
        }

        private Panel GetTabHeadersPanel()
        {
            try
            {
                if (TabHeadersContainer?.ItemContainerGenerator != null)
                {
                    // Método alternativo: buscar en el ItemsControl
                    var presenter = VisualTreeHelperExtensions.FindChild<ItemsPresenter>(TabHeadersContainer);
                    if (presenter != null)
                    {
                        var panel = VisualTreeHelperExtensions.FindChild<StackPanel>(presenter);
                        if (panel != null) return panel;
                    }
                }

                // Método original como fallback
                return VisualTreeHelperExtensions.FindChild<StackPanel>(TabHeadersContainer);
            }
            catch
            {
                return null;
            }
        }

        private Button GetTabButton(int index)
        {
            try
            {
                var containerPanel = GetTabHeadersPanel();
                if (containerPanel != null && index >= 0 && index < containerPanel.Children.Count)
                {
                    var button = containerPanel.Children[index] as Button;
                    if (button != null && button.IsLoaded)
                    {
                        return button;
                    }
                }

                // Método alternativo usando ItemContainerGenerator
                if (TabHeadersContainer?.ItemContainerGenerator != null && index < Items.Count)
                {
                    var container = TabHeadersContainer.ItemContainerGenerator.ContainerFromIndex(index);
                    return VisualTreeHelperExtensions.FindChild<Button>(container as DependencyObject);
                }
            }
            catch
            {
                // Ignorar errores
            }

            return null;
        }

        #endregion

        #region Public Methods

        public void AddTab(string header, object content, DataTemplate contentTemplate = null)
        {
            var tabItem = new AxonTabItem
            {
                Header = header,
                Content = content,
                ContentTemplate = contentTemplate
            };

            Items.Add(tabItem);
        }

        public void RemoveTab(int index)
        {
            if (index >= 0 && index < Items.Count)
            {
                Items.RemoveAt(index);
            }
        }

        public void RemoveTab(AxonTabItem tabItem)
        {
            Items.Remove(tabItem);
        }

        /// <summary>
        /// Fuerza la actualización del indicador
        /// </summary>
        public void RefreshIndicator()
        {
            if (_isLoaded)
            {
                UpdateLayout();
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    UpdateIndicatorPosition(false);
                }), DispatcherPriority.ApplicationIdle);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    /// <summary>
    /// Argumentos para el evento SelectionChanged del AxonTabControl
    /// </summary>
    public class TabSelectionChangedEventArgs : EventArgs
    {
        public int OldIndex { get; set; }
        public int NewIndex { get; set; }
        public AxonTabItem OldTabItem { get; set; }
        public AxonTabItem NewTabItem { get; set; }
    }
}