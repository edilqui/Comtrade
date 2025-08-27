using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Axon.UI.Components
{
    /// <summary>
    /// Control personalizado que combina texto con un botón con icono
    /// </summary>
    [ContentProperty("Button")]
    public partial class AxonTextBlockButton : UserControl, INotifyPropertyChanged
    {
        public AxonTextBlockButton()
        {
            InitializeComponent();
            Button = new AxonTextBlockButtonButton();
        }

        #region Dependency Properties

        /// <summary>
        /// Texto del label superior
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(AxonTextBlockButton),
                new PropertyMetadata(""));

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        /// <summary>
        /// Texto principal que se muestra
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(AxonTextBlockButton),
                new PropertyMetadata(""));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set
            {
                SetValue(TextProperty, value);
                OnPropertyChanged(nameof(DisplayText));
            }
        }

        /// <summary>
        /// Texto placeholder cuando no hay texto
        /// </summary>
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(AxonTextBlockButton),
                new PropertyMetadata(""));

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        /// <summary>
        /// Comando del botón (opción 1: binding directo)
        /// </summary>
        public static readonly DependencyProperty ButtonCommandProperty =
            DependencyProperty.Register(nameof(ButtonCommand), typeof(ICommand), typeof(AxonTextBlockButton),
                new PropertyMetadata(null));

        public ICommand ButtonCommand
        {
            get => (ICommand)GetValue(ButtonCommandProperty);
            set => SetValue(ButtonCommandProperty, value);
        }

        /// <summary>
        /// Parámetro del comando del botón
        /// </summary>
        public static readonly DependencyProperty ButtonCommandParameterProperty =
            DependencyProperty.Register(nameof(ButtonCommandParameter), typeof(object), typeof(AxonTextBlockButton),
                new PropertyMetadata(null));

        public object ButtonCommandParameter
        {
            get => GetValue(ButtonCommandParameterProperty);
            set => SetValue(ButtonCommandParameterProperty, value);
        }

        /// <summary>
        /// Icono del botón como Path geometry
        /// </summary>
        public static readonly DependencyProperty ButtonIconProperty =
            DependencyProperty.Register(nameof(ButtonIcon), typeof(Geometry), typeof(AxonTextBlockButton),
                new PropertyMetadata(null));

        public Geometry ButtonIcon
        {
            get => (Geometry)GetValue(ButtonIconProperty);
            set => SetValue(ButtonIconProperty, value);
        }

        /// <summary>
        /// Tooltip del botón
        /// </summary>
        public static readonly DependencyProperty ButtonTooltipProperty =
            DependencyProperty.Register(nameof(ButtonTooltip), typeof(string), typeof(AxonTextBlockButton),
                new PropertyMetadata(""));

        public string ButtonTooltip
        {
            get => (string)GetValue(ButtonTooltipProperty);
            set => SetValue(ButtonTooltipProperty, value);
        }

        /// <summary>
        /// Visibilidad del botón
        /// </summary>
        public static readonly DependencyProperty ButtonVisibilityProperty =
            DependencyProperty.Register(nameof(ButtonVisibility), typeof(Visibility), typeof(AxonTextBlockButton),
                new PropertyMetadata(Visibility.Visible));

        public Visibility ButtonVisibility
        {
            get => (Visibility)GetValue(ButtonVisibilityProperty);
            set => SetValue(ButtonVisibilityProperty, value);
        }

        /// <summary>
        /// Si el botón está habilitado
        /// </summary>
        public static readonly DependencyProperty ButtonIsEnabledProperty =
            DependencyProperty.Register(nameof(ButtonIsEnabled), typeof(bool), typeof(AxonTextBlockButton),
                new PropertyMetadata(true));

        public bool ButtonIsEnabled
        {
            get => (bool)GetValue(ButtonIsEnabledProperty);
            set => SetValue(ButtonIsEnabledProperty, value);
        }

        /// <summary>
        /// Si el texto es de solo lectura
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(AxonTextBlockButton),
                new PropertyMetadata(false));

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Configuración del botón (opción 2: declarativo en XAML)
        /// </summary>
        public AxonTextBlockButtonButton Button { get; set; }

        /// <summary>
        /// Texto que se muestra efectivamente
        /// </summary>
        public string DisplayText
        {
            get => !string.IsNullOrEmpty(Text) ? Text : Placeholder;
            set
            {
                if (DisplayText != value)
                {
                    OnPropertyChanged(nameof(DisplayText)); // Notifica que DisplayText ha cambiado
                }
            }
        }

        /// <summary>
        /// Color del texto mostrado
        /// </summary>
        public Brush DisplayTextBrush
        {
            get
            {
                if (IsReadOnly)
                    return new SolidColorBrush(Color.FromRgb(108, 117, 125)); // Gris para readonly

                return !string.IsNullOrEmpty(Text)
                    ? new SolidColorBrush(Color.FromRgb(33, 37, 41))    // Negro para texto real
                    : new SolidColorBrush(Color.FromRgb(153, 153, 153)); // Gris para placeholder
            }
        }

        /// <summary>
        /// Comando efectivo del botón (combina las dos opciones)
        /// </summary>
        public ICommand EffectiveButtonCommand => ButtonCommand ?? Button?.Command;

        /// <summary>
        /// Parámetro efectivo del comando
        /// </summary>
        public object EffectiveButtonCommandParameter => ButtonCommandParameter ?? Button?.CommandParameter;

        /// <summary>
        /// Icono efectivo del botón
        /// </summary>
        public Geometry EffectiveButtonIcon => ButtonIcon ?? Button?.Icon;

        /// <summary>
        /// Tooltip efectivo del botón
        /// </summary>
        public string EffectiveButtonTooltip => !string.IsNullOrEmpty(ButtonTooltip) ? ButtonTooltip : Button?.Tooltip;

        /// <summary>
        /// Visibilidad efectiva del botón
        /// </summary>
        public Visibility EffectiveButtonVisibility
        {
            get
            {
                if (ButtonVisibility == Visibility.Collapsed || ButtonVisibility == Visibility.Hidden)
                    return ButtonVisibility;

                return Button?.Visibility ?? ButtonVisibility;
            }
        }

        /// <summary>
        /// Estado habilitado efectivo del botón
        /// </summary>
        public bool EffectiveButtonIsEnabled => ButtonIsEnabled && (Button?.IsEnabled ?? true);

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Evento cuando se hace click en el botón
        /// </summary>
        public event EventHandler<RoutedEventArgs> ButtonClick;

        #endregion

        #region Private Methods

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            // Ejecutar comando si existe
            var command = EffectiveButtonCommand;
            var parameter = EffectiveButtonCommandParameter;

            if (command?.CanExecute(parameter) == true)
            {
                command.Execute(parameter);
            }

            // Disparar evento
            ButtonClick?.Invoke(this, e);
        }

        #endregion
    }

    /// <summary>
    /// Configuración del botón para uso declarativo en XAML
    /// </summary>
    public class AxonTextBlockButtonButton : INotifyPropertyChanged
    {
        private ICommand _command;
        private object _commandParameter;
        private Geometry _icon;
        private string _tooltip;
        private Visibility _visibility = Visibility.Visible;
        private bool _isEnabled = true;

        /// <summary>
        /// Comando del botón
        /// </summary>
        public ICommand Command
        {
            get => _command;
            set { _command = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Parámetro del comando
        /// </summary>
        public object CommandParameter
        {
            get => _commandParameter;
            set { _commandParameter = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Icono como Path geometry
        /// </summary>
        public Geometry Icon
        {
            get => _icon;
            set { _icon = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Tooltip del botón
        /// </summary>
        public string Tooltip
        {
            get => _tooltip;
            set { _tooltip = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Visibilidad del botón
        /// </summary>
        public Visibility Visibility
        {
            get => _visibility;
            set { _visibility = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Si el botón está habilitado
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set { _isEnabled = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}