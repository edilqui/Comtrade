using Axon.Comtrade.ViewModel;
using Axon.UI.Components;
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
    /// Interaction logic for DevicesExplorer.xaml
    /// </summary>
    public partial class ConfigurationView : UserControl
    {
        public ConfigurationView()
        {
            InitializeComponent();
            InitTab();
        }

        private void InitTab()
        {
            //MainTabControl.AddTab("Comunicación", new EmptyPageView());
            //MainTabControl.AddTab("Recolección", null);
            //MainTabControl.AddTab("Reglas", null);
            //MainTabControl.AddTab("Análisis", null);
        }

        private void OnTabChanged(object sender, TabSelectionChangedEventArgs e)
        {
            Console.WriteLine($"Cambió de '{e.OldTabItem?.Header}' a '{e.NewTabItem?.Header}'");

            // Aquí puedes implementar lógica específica
            switch (e.NewTabItem?.Header)
            {
                case "Comunicación":
                    // Lógica para comunicación
                    break;
                case "Recolección":
                    // Lógica para recolección
                    break;
            }
        }

        private void OnFolderStructureChanged(object sender, RadioSelectionChangedEventArgs e)
        {
            //var oldValue = (FolderStructure?)e.OldValue;
            //var newValue = (FolderStructure?)e.NewValue;

            //Console.WriteLine($"Estructura de carpetas cambió de '{oldValue}' a '{newValue}'");

            //// Lógica específica según la estructura seleccionada
            //switch (newValue)
            //{
            //    case FolderStructure.ARCHITECTURE:
            //        // Configurar para estructura por arquitectura
            //        Console.WriteLine("Configurando estructura por arquitectura...");
            //        break;
            //    case FolderStructure.DATE:
            //        // Configurar para estructura por fecha
            //        Console.WriteLine("Configurando estructura por fecha...");
            //        break;
            //}
        }
    }
}
