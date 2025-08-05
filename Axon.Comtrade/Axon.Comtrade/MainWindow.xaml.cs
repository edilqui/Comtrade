using Axon.UI.Components.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Axon.Comtrade
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainNavigation_ItemSelected(object sender, NavigationItemSelectedEventArgs e)
        {
            // Manejo de navegación
            switch (e.ItemId)
            {
                case "devices":
                    Title = "Axon Comtrade - Dispositivos";
                    // MainContentPresenter.Content = new DevicesView();
                    break;
                case "substation":
                    Title = "Axon Comtrade - Subestación";
                    // MainContentPresenter.Content = new SubstationView();
                    break;
                case "archive":
                    Title = "Axon Comtrade - Archivado";
                    // MainContentPresenter.Content = new ArchiveView();
                    break;
                case "faultAnalysis":
                    Title = "Axon Comtrade - Análisis de Fallas";
                    // MainContentPresenter.Content = new FaultAnalysisView();
                    break;
                case "configuration":
                    Title = "Axon Comtrade - Configuración";
                    // MainContentPresenter.Content = new ConfigurationView();
                    break;
            }
        }

    }
}
