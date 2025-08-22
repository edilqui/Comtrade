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

namespace Axon.UI.Components
{
    [ContentProperty("Items")] // Permite contenido directo en XAML
    public partial class AxonTabControl : UserControl, INotifyPropertyChanged
    {
        private Storyboard _indicatorAnimation;

        public AxonTabControl()
        {
            InitializeComponent();
            Items = new ObservableCollection<AxonTabItem>();
            SelectTabCommand = new DelegateCommand<AxonTabItem>(SelectTab);

            Items.CollectionChanged += Items_CollectionChanged;
            Loaded += OnLoaded;
        }

        #region Dependency Properties

        // Items Property para soporte XAML
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

        // SelectedIndex Property
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(AxonTabControl),
                new PropertyMetadata(-1, OnSelectedIndexChanged));

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        // SelectedTabItem Property (readonly)
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

        #region Properties (Backward compatibility)

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

            Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateIndicatorPosition(false);
            }), System.Windows.Threading.DispatcherPriority.Loaded);
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
            _indicatorAnimation = (Storyboard)Resources["IndicatorMoveAnimation"];

            if (Items.Count > 0 && SelectedIndex < 0)
            {
                SelectedIndex = 0;
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateIndicatorPosition(false);
            }), System.Windows.Threading.DispatcherPriority.Loaded);
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

            Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateIndicatorPosition(true);
            }), System.Windows.Threading.DispatcherPriority.Render);

            OnPropertyChanged(nameof(SelectedTabItem));
        }

        private void UpdateIndicatorPosition(bool animate)
        {
            if (SelectedIndex < 0 || SelectedIndex >= Items.Count)
            {
                SelectedIndicator.Visibility = Visibility.Hidden;
                return;
            }

            SelectedIndicator.Visibility = Visibility.Visible;

            var containerPanel = GetTabHeadersPanel();
            if (containerPanel == null)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    UpdateIndicatorPosition(animate);
                }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
                return;
            }

            var targetButton = GetTabButton(SelectedIndex);
            if (targetButton == null) return;

            var buttonPosition = targetButton.TranslatePoint(new Point(0, 0), containerPanel);
            var targetLeft = buttonPosition.X;
            var targetWidth = targetButton.ActualWidth;

            if (animate && _indicatorAnimation != null && targetLeft >= 0)
            {
                var moveAnimation = _indicatorAnimation.Children.OfType<DoubleAnimation>()
                    .FirstOrDefault(a => a.Name == "IndicatorMoveX");
                var resizeAnimation = _indicatorAnimation.Children.OfType<DoubleAnimation>()
                    .FirstOrDefault(a => a.Name == "IndicatorResizeWidth");

                if (moveAnimation != null)
                {
                    moveAnimation.From = Canvas.GetLeft(SelectedIndicator);
                    moveAnimation.To = targetLeft;
                }

                if (resizeAnimation != null)
                {
                    resizeAnimation.From = SelectedIndicator.Width;
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

        private Panel GetTabHeadersPanel()
        {
            try
            {
                return VisualTreeHelperExtensions.FindChild<StackPanel>(TabHeadersContainer);
            }
            catch
            {
                return null;
            }
        }

        private Button GetTabButton(int index)
        {
            var containerPanel = GetTabHeadersPanel();
            if (containerPanel != null && index >= 0 && index < containerPanel.Children.Count)
            {
                return containerPanel.Children[index] as Button;
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