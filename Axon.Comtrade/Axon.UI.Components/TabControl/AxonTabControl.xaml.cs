using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Axon.UI.Components
{
    public partial class AxonTabControl : UserControl, INotifyPropertyChanged
    {
        private Storyboard _indicatorAnimation;

        public AxonTabControl()
        {
            InitializeComponent();
            TabItems = new ObservableCollection<AxonTabItem>();
            SelectTabCommand = new RelayCommand<AxonTabItem>(SelectTab);

            Loaded += OnLoaded;
        }

        #region Dependency Properties

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

        #region Properties

        public ObservableCollection<AxonTabItem> TabItems { get; private set; }

        public ICommand SelectTabCommand { get; private set; }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<TabSelectionChangedEventArgs> SelectionChanged;

        #endregion

        #region Event Handlers

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

            if (TabItems.Count > 0 && SelectedIndex < 0)
            {
                SelectedIndex = 0;
            }

            UpdateIndicatorPosition(false);
        }

        #endregion

        #region Private Methods

        private void SelectTab(AxonTabItem tabItem)
        {
            if (tabItem == null || !tabItem.IsEnabled) return;

            var oldIndex = SelectedIndex;
            var newIndex = TabItems.IndexOf(tabItem);

            if (newIndex >= 0 && newIndex != oldIndex)
            {
                SelectedIndex = newIndex;

                // Disparar evento de cambio de selección
                SelectionChanged?.Invoke(this, new TabSelectionChangedEventArgs
                {
                    OldIndex = oldIndex,
                    NewIndex = newIndex,
                    OldTabItem = oldIndex >= 0 && oldIndex < TabItems.Count ? TabItems[oldIndex] : null,
                    NewTabItem = tabItem
                });
            }
        }

        private void UpdateSelectedTab()
        {
            // Deseleccionar todos los tabs
            foreach (var tab in TabItems)
            {
                tab.IsSelected = false;
            }

            // Seleccionar el tab actual
            if (SelectedIndex >= 0 && SelectedIndex < TabItems.Count)
            {
                var selectedTab = TabItems[SelectedIndex];
                selectedTab.IsSelected = true;
                SelectedTabItem = selectedTab;
            }
            else
            {
                SelectedTabItem = null;
            }

            UpdateIndicatorPosition(true);
            OnPropertyChanged(nameof(SelectedTabItem));
        }

        private void UpdateIndicatorPosition(bool animate)
        {
            if (SelectedIndex < 0 || SelectedIndex >= TabItems.Count)
            {
                SelectedIndicator.Visibility = Visibility.Hidden;
                return;
            }

            SelectedIndicator.Visibility = Visibility.Visible;

            // Calcular posición del indicador
            var containerPanel = GetTabHeadersPanel();
            if (containerPanel == null) return;

            var targetButton = GetTabButton(SelectedIndex);
            if (targetButton == null) return;

            var buttonPosition = targetButton.TranslatePoint(new Point(0, 0), containerPanel);
            var targetLeft = buttonPosition.X;
            var targetWidth = targetButton.ActualWidth;

            if (animate && _indicatorAnimation != null)
            {
                // Animar el movimiento del indicador
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
                // Posicionar directamente sin animación
                Canvas.SetLeft(SelectedIndicator, targetLeft);
                SelectedIndicator.Width = targetWidth;
            }
        }

        private Panel GetTabHeadersPanel()
        {
            if (TabHeadersContainer.ItemsSource is System.Collections.IEnumerable)
            {
                return VisualTreeHelperExtensions.FindChild<StackPanel>(TabHeadersContainer);
                //return TabHeadersContainer.FindChild<StackPanel>();
            }
            return null;
        }

        private Button GetTabButton(int index)
        {
            var containerPanel = GetTabHeadersPanel();
            if (containerPanel != null && index < containerPanel.Children.Count)
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

            TabItems.Add(tabItem);

            if (TabItems.Count == 1)
            {
                SelectedIndex = 0;
            }
        }

        public void RemoveTab(int index)
        {
            if (index >= 0 && index < TabItems.Count)
            {
                TabItems.RemoveAt(index);

                if (SelectedIndex >= TabItems.Count)
                {
                    SelectedIndex = TabItems.Count - 1;
                }
            }
        }

        public void RemoveTab(AxonTabItem tabItem)
        {
            var index = TabItems.IndexOf(tabItem);
            if (index >= 0)
            {
                RemoveTab(index);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    #region Helper Classes

    /// <summary>
    /// Argumentos para el evento SelectionChanged
    /// </summary>
    public class TabSelectionChangedEventArgs : EventArgs
    {
        public int OldIndex { get; set; }
        public int NewIndex { get; set; }
        public AxonTabItem OldTabItem { get; set; }
        public AxonTabItem NewTabItem { get; set; }
    }

    /// <summary>
    /// Comando simple para MVVM
    /// </summary>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke((T)parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
    }

    #endregion
}