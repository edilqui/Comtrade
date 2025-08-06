using Axon.UI.Components.Navigation;
using Axon.UI.Components.TreeNode;
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

        // Event handler para el botón de menú contextual
        private void ShowContextMenu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.ContextMenu != null)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.IsOpen = true;
            }
        }

    }

    // Extension methods para facilitar el uso
    public static class TreeViewExtensions
    {
        public static TreeNodeModel FindNodeByTitle(this TreeViewModel viewModel, string title)
        {
            foreach (var node in viewModel.Nodes)
            {
                var found = FindNodeRecursive(node, title);
                if (found != null)
                    return found;
            }
            return null;
        }

        private static TreeNodeModel FindNodeRecursive(TreeNodeModel node, string title)
        {
            if (node.Title == title)
                return node;

            foreach (var child in node.Children)
            {
                var found = FindNodeRecursive(child, title);
                if (found != null)
                    return found;
            }
            return null;
        }

        public static void ExpandAll(this TreeNodeModel node)
        {
            node.IsExpanded = true;
            foreach (var child in node.Children)
            {
                ExpandAll(child);
            }
        }

        public static void CollapseAll(this TreeNodeModel node)
        {
            node.IsExpanded = false;
            foreach (var child in node.Children)
            {
                CollapseAll(child);
            }
        }
    }
}
