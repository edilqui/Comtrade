using Axon.UI.Components.Base;
using System;
using System.ComponentModel;
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
        private Storyboard _toggleOnAnimation;
        private Storyboard _toggleOffAnimation;

        public AxonToggleButton()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += OnLoaded;
            ToggleCommand = new DelegateCommand(Toggle);
        }

        #region Dependency Properties

        /// <summary>
        /// Indica si el toggle está activado
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(AxonToggleButton),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsCheckedChanged));

        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        /// <summary>
        /// Texto que se muestra cuando el toggle está activado
        /// </summary>
        public static readonly DependencyProperty CheckedTextProperty =
            DependencyProperty.Register(nameof(CheckedText), typeof(string), typeof(AxonToggleButton),
                new PropertyMetadata("Habilitado", OnTextChanged));

        public string CheckedText
        {
            get => (string)GetValue(CheckedTextProperty);
            set => SetValue(CheckedTextProperty, value);
        }

        /// <summary>
        /// Texto que se muestra cuando el toggle está desactivado
        /// </summary>
        public static readonly DependencyProperty UncheckedTextProperty =
            DependencyProperty.Register(nameof(UncheckedText), typeof(string), typeof(AxonToggleButton),
                new PropertyMetadata("Deshabilitado", OnTextChanged));

        public string UncheckedText
        {
            get => (string)GetValue(UncheckedTextProperty);
            set => SetValue(UncheckedTextProperty, value);
        }

        /// <summary>
        /// Texto del label (título del control)
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(AxonToggleButton),
                new PropertyMetadata("Estado"));

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        /// <summary>
        /// Controla si el toggle está habilitado para interacción
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(AxonToggleButton),
                new PropertyMetadata(true, OnIsEnabledChanged));

        public new bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        /// <summary>
        /// Duración de la animación en milisegundos
        /// </summary>
        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register(nameof(AnimationDuration), typeof(int), typeof(AxonToggleButton),
                new PropertyMetadata(200));

        public int AnimationDuration
        {
            get => (int)GetValue(AnimationDurationProperty);
            set => SetValue(AnimationDurationProperty, value);
        }

        #endregion

        #region Read-Only Properties

        /// <summary>
        /// Texto actual que se muestra según el estado
        /// </summary>
        public string CurrentText => IsChecked ? CheckedText : UncheckedText;

        /// <summary>
        /// Color del texto según el estado
        /// </summary>
        public Brush CurrentTextColor
        {
            get
            {
                if (!IsEnabled)
                    return new SolidColorBrush(Color.FromRgb(160, 160, 160)); // Gris para deshabilitado

                return IsChecked
                    ? new SolidColorBrush(Color.FromRgb(107, 70, 193)) // Morado para activado
                    : new SolidColorBrush(Color.FromRgb(100, 100, 100)); // Gris oscuro para desactivado
            }
        }

        /// <summary>
        /// Color del switch según el estado
        /// </summary>
        public Brush SwitchBackground
        {
            get
            {
                if (!IsEnabled)
                    return new SolidColorBrush(Color.FromRgb(220, 220, 220)); // Gris claro para deshabilitado

                return IsChecked
                    ? new SolidColorBrush(Color.FromRgb(107, 70, 193)) // Morado para activado
                    : new SolidColorBrush(Color.FromRgb(200, 200, 200)); // Gris para desactivado
            }
        }

        /// <summary>
        /// Posición del círculo del switch
        /// </summary>
        public double CirclePosition => IsChecked ? 22 : 2;

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
                control.OnToggled((bool)e.OldValue, (bool)e.NewValue);
                control.UpdateVisualState(true);
            }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AxonToggleButton control)
            {
                control.OnPropertyChanged(nameof(CurrentText));
            }
        }

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AxonToggleButton control)
            {
                control.UpdateVisualState(false);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            InitializeAnimations();
            UpdateVisualState(false);
        }

        #endregion

        #region Private Methods

        private void InitializeAnimations()
        {
            var duration = TimeSpan.FromMilliseconds(AnimationDuration);

            // Animación para activar (mover círculo a la derecha)
            _toggleOnAnimation = new Storyboard();
            var moveRightAnimation = new DoubleAnimation
            {
                To = 22,
                Duration = duration,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTargetName(moveRightAnimation, "SwitchCircle");
            Storyboard.SetTargetProperty(moveRightAnimation, new PropertyPath("(Canvas.Left)"));
            _toggleOnAnimation.Children.Add(moveRightAnimation);

            // Animación para desactivar (mover círculo a la izquierda)
            _toggleOffAnimation = new Storyboard();
            var moveLeftAnimation = new DoubleAnimation
            {
                To = 2,
                Duration = duration,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTargetName(moveLeftAnimation, "SwitchCircle");
            Storyboard.SetTargetProperty(moveLeftAnimation, new PropertyPath("(Canvas.Left)"));
            _toggleOffAnimation.Children.Add(moveLeftAnimation);

            // Registrar las animaciones como recursos
            Resources["ToggleOnAnimation"] = _toggleOnAnimation;
            Resources["ToggleOffAnimation"] = _toggleOffAnimation;
        }

        private void UpdateVisualState(bool animate)
        {
            OnPropertyChanged(nameof(CurrentText));
            OnPropertyChanged(nameof(CurrentTextColor));
            OnPropertyChanged(nameof(SwitchBackground));
            OnPropertyChanged(nameof(CirclePosition));

            if (animate && IsLoaded)
            {
                if (IsChecked)
                {
                    _toggleOnAnimation?.Begin(this);
                }
                else
                {
                    _toggleOffAnimation?.Begin(this);
                }
            }
            else
            {
                // Posicionamiento directo sin animación
                if (FindName("SwitchCircle") is FrameworkElement circle)
                {
                    Canvas.SetLeft(circle, CirclePosition);
                }
            }
        }

        private void Toggle()
        {
            if (IsEnabled)
            {
                IsChecked = !IsChecked;
            }
        }

        private void OnToggled(bool oldValue, bool newValue)
        {
            Toggled?.Invoke(this, new ToggleChangedEventArgs
            {
                OldValue = oldValue,
                NewValue = newValue
            });
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    #region Helper Classes

    /// <summary>
    /// Argumentos para el evento Toggled
    /// </summary>
    public class ToggleChangedEventArgs : EventArgs
    {
        public bool OldValue { get; set; }
        public bool NewValue { get; set; }
    }

    #endregion
}