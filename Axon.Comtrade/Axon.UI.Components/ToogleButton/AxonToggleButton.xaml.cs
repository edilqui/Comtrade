using Axon.UI.Components.Base;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Axon.UI.Components
{
    /// <summary>
    /// Control personalizado de toggle button con animaciones y binding completo
    /// </summary>
    public partial class AxonToggleButton : UserControl, INotifyPropertyChanged
    {
        public AxonToggleButton()
        {
            InitializeComponent();
            ToggleCommand = new DelegateCommand(ExecuteToggle, CanExecuteToggle);
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Establecer posición inicial del círculo basada en IsChecked
            var circle = FindName("SwitchCircle") as FrameworkElement;
            if (circle != null)
            {
                var initialPosition = IsChecked ? 22.0 : 2.0;
                Canvas.SetLeft(circle, initialPosition);
            }

            // Actualizar estado visual inicial
            UpdateVisualState();
        }

        #region Dependency Properties

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(AxonToggleButton),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsCheckedChanged));

        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public static readonly DependencyProperty CheckedTextProperty =
            DependencyProperty.Register(nameof(CheckedText), typeof(string), typeof(AxonToggleButton),
                new PropertyMetadata("Habilitado"));

        public string CheckedText
        {
            get => (string)GetValue(CheckedTextProperty);
            set => SetValue(CheckedTextProperty, value);
        }

        public static readonly DependencyProperty UncheckedTextProperty =
            DependencyProperty.Register(nameof(UncheckedText), typeof(string), typeof(AxonToggleButton),
                new PropertyMetadata("Deshabilitado"));

        public string UncheckedText
        {
            get => (string)GetValue(UncheckedTextProperty);
            set => SetValue(UncheckedTextProperty, value);
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(AxonToggleButton),
                new PropertyMetadata("Estado"));

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(AxonToggleButton),
                new PropertyMetadata(true, OnIsEnabledChanged));

        public new bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        #endregion

        #region Commands

        public ICommand ToggleCommand { get; private set; }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<ToggleChangedEventArgs> Toggled;

        #endregion

        #region Event Handlers

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AxonToggleButton control)
            {
                var oldValue = (bool)e.OldValue;
                var newValue = (bool)e.NewValue;

                control.UpdateVisualState();
                control.Toggled?.Invoke(control, new ToggleChangedEventArgs
                {
                    OldValue = oldValue,
                    NewValue = newValue
                });
            }
        }

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AxonToggleButton control)
            {
                control.UpdateVisualState();
                control.OnPropertyChanged(nameof(IsEnabled));
            }
        }

        #endregion

        #region Command Methods

        private void ExecuteToggle()
        {
            IsChecked = !IsChecked;
        }

        private bool CanExecuteToggle()
        {
            return IsEnabled;
        }

        #endregion

        #region Private Methods

        private void UpdateVisualState()
        {
            OnPropertyChanged(nameof(CurrentText));
            OnPropertyChanged(nameof(CurrentTextColor));
            OnPropertyChanged(nameof(SwitchBackground));

            // Animar el círculo con un pequeño delay para asegurar que el control esté completamente cargado
            Dispatcher.BeginInvoke(new Action(() =>
            {
                AnimateCircle();
            }), System.Windows.Threading.DispatcherPriority.Render);
        }

        private void AnimateCircle()
        {
            var circle = FindName("SwitchCircle") as FrameworkElement;
            if (circle == null) return;

            try
            {
                var currentPosition = Canvas.GetLeft(circle);
                var targetPosition = IsChecked ? 14.0 : 2.0;

                // Si la posición actual es NaN, establecer posición inicial
                if (double.IsNaN(currentPosition))
                {
                    currentPosition = IsChecked ? 2.0 : 14.0; // Posición opuesta para animar
                    Canvas.SetLeft(circle, currentPosition);
                }

                // Solo animar si hay diferencia en la posición
                if (Math.Abs(currentPosition - targetPosition) > 0.1)
                {
                    var animation = new DoubleAnimation
                    {
                        From = currentPosition,
                        To = targetPosition,
                        Duration = TimeSpan.FromMilliseconds(200),
                        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                    };

                    circle.BeginAnimation(Canvas.LeftProperty, animation);
                }
                else
                {
                    // Si ya está en la posición correcta, solo asegurar
                    Canvas.SetLeft(circle, targetPosition);
                }
            }
            catch (Exception ex)
            {
                // En caso de error, posicionar directamente
                System.Diagnostics.Debug.WriteLine($"Error en animación: {ex.Message}");
                Canvas.SetLeft(circle, IsChecked ? 14.0 : 2.0);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Public Properties for Binding

        public string CurrentText => IsChecked ? CheckedText : UncheckedText;

        public Brush CurrentTextColor
        {
            get
            {
                if (!IsEnabled)
                    return (System.Windows.Media.Brush)FindResource("TextDisabledBrush");

                return IsChecked
                    ? (System.Windows.Media.Brush)FindResource("PrimaryBrush") // Morado
                    : (System.Windows.Media.Brush)FindResource("TextSecondaryBrush");  // Gris
            }
        }

        public Brush SwitchBackground
        {
            get
            {
                if (!IsEnabled)
                    return new SolidColorBrush(Color.FromRgb(220, 220, 220));

                return IsChecked
                    ? new SolidColorBrush(Color.FromRgb(107, 70, 193)) // Morado
                    : new SolidColorBrush(Color.FromRgb(200, 200, 200)); // Gris
            }
        }

        #endregion

        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (CanExecuteToggle())
                ExecuteToggle();
        }
    }

    #region Helper Classes

    public class ToggleChangedEventArgs : EventArgs
    {
        public bool OldValue { get; set; }
        public bool NewValue { get; set; }
    }

    #endregion
}