using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Axon.Comtrade.Views
{
    /// <summary>
    /// Interaction logic for EmptyPageView.xaml
    /// </summary>
    public partial class EmptyPageView : UserControl
    {
        #region Dependency Properties

        /// <summary>
        /// Título principal de la página vacía
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(EmptyPageView),
                new PropertyMetadata("Página en construcción", OnTitleChanged));

        /// <summary>
        /// Subtítulo o descripción de la página vacía
        /// </summary>
        public static readonly DependencyProperty SubtitleProperty =
            DependencyProperty.Register("Subtitle", typeof(string), typeof(EmptyPageView),
                new PropertyMetadata("Esta vista aún no ha sido implementada. Estamos trabajando para tenerla lista pronto.", OnSubtitleChanged));

        /// <summary>
        /// Texto del botón de acción (si se muestra)
        /// </summary>
        public static readonly DependencyProperty ActionButtonTextProperty =
            DependencyProperty.Register("ActionButtonText", typeof(string), typeof(EmptyPageView),
                new PropertyMetadata("Volver atrás", OnActionButtonTextChanged));

        /// <summary>
        /// Visibilidad del botón de acción
        /// </summary>
        public static readonly DependencyProperty ShowActionButtonProperty =
            DependencyProperty.Register("ShowActionButton", typeof(bool), typeof(EmptyPageView),
                new PropertyMetadata(false, OnShowActionButtonChanged));

        /// <summary>
        /// Comando a ejecutar cuando se hace clic en el botón de acción
        /// </summary>
        public static readonly DependencyProperty ActionCommandProperty =
            DependencyProperty.Register("ActionCommand", typeof(System.Windows.Input.ICommand), typeof(EmptyPageView),
                new PropertyMetadata(null));

        #endregion

        #region Properties

        /// <summary>
        /// Título principal de la página vacía
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// Subtítulo o descripción de la página vacía
        /// </summary>
        public string Subtitle
        {
            get => (string)GetValue(SubtitleProperty);
            set => SetValue(SubtitleProperty, value);
        }

        /// <summary>
        /// Texto del botón de acción
        /// </summary>
        public string ActionButtonText
        {
            get => (string)GetValue(ActionButtonTextProperty);
            set => SetValue(ActionButtonTextProperty, value);
        }

        /// <summary>
        /// Determina si se muestra el botón de acción
        /// </summary>
        public bool ShowActionButton
        {
            get => (bool)GetValue(ShowActionButtonProperty);
            set => SetValue(ShowActionButtonProperty, value);
        }

        /// <summary>
        /// Comando a ejecutar cuando se hace clic en el botón de acción
        /// </summary>
        public System.Windows.Input.ICommand ActionCommand
        {
            get => (System.Windows.Input.ICommand)GetValue(ActionCommandProperty);
            set => SetValue(ActionCommandProperty, value);
        }

        #endregion

        #region Events

        /// <summary>
        /// Evento que se dispara cuando se hace clic en el botón de acción
        /// </summary>
        public event EventHandler ActionButtonClicked;

        #endregion

        #region Constructor

        public EmptyPageView()
        {
            InitializeComponent();
        }

        #endregion

        #region Event Handlers

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            // Ejecutar comando si está disponible
            if (ActionCommand?.CanExecute(null) == true)
            {
                ActionCommand.Execute(null);
            }

            // Disparar evento
            ActionButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Property Changed Callbacks

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EmptyPageView control && control.TitleText != null)
            {
                control.TitleText.Text = e.NewValue?.ToString() ?? string.Empty;
            }
        }

        private static void OnSubtitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EmptyPageView control && control.SubtitleText != null)
            {
                control.SubtitleText.Text = e.NewValue?.ToString() ?? string.Empty;
            }
        }

        private static void OnActionButtonTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EmptyPageView control && control.ActionButton != null)
            {
                control.ActionButton.Content = e.NewValue?.ToString() ?? string.Empty;
            }
        }

        private static void OnShowActionButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EmptyPageView control && control.ActionButton != null)
            {
                control.ActionButton.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Muestra el indicador de carga
        /// </summary>
        //public void ShowLoading()
        //{
        //    if (LoadingIndicator != null)
        //    {
        //        LoadingIndicator.Visibility = Visibility.Visible;
        //    }
        //}

        /// <summary>
        /// Oculta el indicador de carga
        /// </summary>
        //public void HideLoading()
        //{
        //    if (LoadingIndicator != null)
        //    {
        //        LoadingIndicator.Visibility = Visibility.Collapsed;
        //    }
        //}

        /// <summary>
        /// Actualiza el contenido de la página vacía
        /// </summary>
        /// <param name="title">Nuevo título</param>
        /// <param name="subtitle">Nuevo subtítulo</param>
        /// <param name="showButton">Mostrar botón de acción</param>
        /// <param name="buttonText">Texto del botón</param>
        public void UpdateContent(string title, string subtitle, bool showButton = false, string buttonText = "Volver atrás")
        {
            Title = title;
            Subtitle = subtitle;
            ShowActionButton = showButton;
            ActionButtonText = buttonText;
        }

        #endregion
    }
}
