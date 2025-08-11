using Axon.Comtrade.Model;
using Axon.Comtrade.ViewModel;
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
    /// Interaction logic for TopologyPanel.xaml
    /// </summary>
    public partial class TopologyPanel : UserControl
    {
        private TopologyTreeViewModel _viewModel;
        public TopologyPanel()
        {
            InitializeComponent();
            // Cargar datos de tu aplicación
            var topologyData = LoadTopologyDataFromDatabase();

            _viewModel = new TopologyTreeViewModel(topologyData);
            _viewModel.OnDataChanged += SaveTopologyData;

            DataContext = _viewModel;
        }

        private List<TopologyNodeModel> LoadTopologyDataFromDatabase()
        {
            // Tu lógica para cargar datos existentes
            // Por ejemplo, desde Entity Framework, API, etc.
            return new List<TopologyNodeModel>
        {
            new TopologyNodeModel
            {
                Id = 1,
                Name = "Dispositivos",
                Type = "Dispositivos",
                Topologies = new List<TopologyNodeModel>
                {
                    new TopologyNodeModel
                    {
                        Id = 2,
                        Name = "Subestación Principal",
                        Type = "Subestacion",
                        Topologies = new List<TopologyNodeModel>
                        {
                            new TopologyNodeModel
                            {
                                Id = 3,
                                Name = "Bahía A1",
                                Type = "Bahia",
                                Protocols = new List<ProtocolNodeModel>
                                {
                                    new ProtocolNodeModel { Id = 1, Name = "IEC-61850", Type = "IEC-61850" },
                                    new ProtocolNodeModel { Id = 2, Name = "FTP Server", Type = "FTP" }
                                }
                            },
                            new TopologyNodeModel
                            {
                                Id = 4,
                                Name = "Bahía B2",
                                Type = "Bahia",
                                Protocols = new List<ProtocolNodeModel>
                                {
                                    new ProtocolNodeModel { Id = 3, Name = "MODBUS TCP", Type = "MODBUS" }
                                }
                            }
                        }
                    }
                }
            }
        };
        }

        private void SaveTopologyData(List<TopologyNodeModel> updatedData)
        {
            // Tu lógica para guardar cambios
            // Por ejemplo, en base de datos, archivo, API, etc.

            try
            {
                // Ejemplo con Entity Framework
                // dbContext.TopologyNodes.UpdateRange(updatedData);
                // dbContext.SaveChanges();

                MessageBox.Show("Cambios guardados exitosamente", "Información",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TopologyTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }
    }
}
