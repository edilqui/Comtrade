using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Axon.UI.Components
{
    public partial class AxonTextBox : UserControl, INotifyPropertyChanged
    {
        private Storyboard _focusedStoryboard;
        private Storyboard _unfocusedStoryboard;

        public AxonTextBox()
        {
            InitializeComponent();
            InitializeStoryboards();
        }

        #region Dependency Properties

        // Text Property
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(AxonTextBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnTextChanged));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        // Label Property
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(AxonTextBox),
                new PropertyMetadata(string.Empty, OnLabelChanged));

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        // Units Property
        public static readonly DependencyProperty UnitsProperty =
            DependencyProperty.Register(nameof(Units), typeof(string), typeof(AxonTextBox),
                new PropertyMetadata(string.Empty, OnUnitsChanged));

        public string Units
        {
            get => (string)GetValue(UnitsProperty);
            set => SetValue(UnitsProperty, value);
        }

        // IsReadOnly Property
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(AxonTextBox),
                new PropertyMetadata(false));

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        // Placeholder Property
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(AxonTextBox),
                new PropertyMetadata(string.Empty));

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        #endregion

        #region Computed Properties

        public bool HasLabel => !string.IsNullOrEmpty(Label);
        public bool HasUnits => !string.IsNullOrEmpty(Units);

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<TextChangedEventArgs> TextChanged;
        public event EventHandler GotFocus;
        public event EventHandler LostFocus;

        #endregion

        #region Private Methods

        private void InitializeStoryboards()
        {
            // Las storyboards se inicializan cuando se carga el control
            Loaded += (s, e) =>
            {
                _focusedStoryboard = (Storyboard)Resources["FocusedStoryboard"];
                _unfocusedStoryboard = (Storyboard)Resources["UnfocusedStoryboard"];
            };
        }

        #endregion

        #region Event Handlers

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AxonTextBox control)
            {
                control.TextChanged?.Invoke(control, new TextChangedEventArgs(TextBox.TextChangedEvent, UndoAction.None));
            }
        }

        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AxonTextBox control)
            {
                control.OnPropertyChanged(nameof(HasLabel));
            }
        }

        private static void OnUnitsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AxonTextBox control)
            {
                control.OnPropertyChanged(nameof(HasUnits));
            }
        }

        private void MainTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                // Cambiar color del borde
                //MainBorder.BorderBrush = (System.Windows.Media.Brush)FindResource("ColorBrandStroke1Brush");

                // Ejecutar animación de foco
                _focusedStoryboard?.Begin();

                GotFocus?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                // Log error si es necesario
                System.Diagnostics.Debug.WriteLine($"Error en GotFocus: {ex.Message}");
            }
        }

        private void MainTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                // Restaurar color del borde
                MainBorder.BorderBrush = (System.Windows.Media.Brush)FindResource("ColorNeutralStroke2Brush");

                // Ejecutar animación de pérdida de foco
                _unfocusedStoryboard?.Begin();

                LostFocus?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                // Log error si es necesario
                System.Diagnostics.Debug.WriteLine($"Error en LostFocus: {ex.Message}");
            }
        }

        private void MainTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged?.Invoke(this, e);
        }

        #endregion

        #region Public Methods

        public void Focus()
        {
            MainTextBox?.Focus();
        }

        public void SelectAll()
        {
            MainTextBox?.SelectAll();
        }

        public void SetValidationState(bool isValid)
        {
            try
            {
                if (isValid)
                {
                    MainBorder.BorderBrush = (System.Windows.Media.Brush)FindResource("ColorNeutralStroke2Brush");
                    FocusLine.Fill = (System.Windows.Media.Brush)FindResource("ColorBrandBackgroundBrush");
                }
                else
                {
                    MainBorder.BorderBrush = (System.Windows.Media.Brush)FindResource("ColorStatusDangerBorder2Brush");
                    FocusLine.Fill = (System.Windows.Media.Brush)FindResource("ColorStatusDangerBackground3Brush");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en SetValidationState: {ex.Message}");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}