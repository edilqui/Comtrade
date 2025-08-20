using Axon.Comtrade.Model;
using Axon.UI.Components.TreeNode;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Axon.Comtrade.ViewModel
{
    public class TopologyTreeViewModel : INotifyPropertyChanged
    {
        private GenericTreeNodeModel _selectedNode;
        private List<TopologyNodeModel> _originalData;

        public ObservableCollection<GenericTreeNodeModel> TreeNodes { get; set; }

        public GenericTreeNodeModel SelectedNode
        {
            get => _selectedNode;
            set => SetProperty(ref _selectedNode, value);
        }

        // Comandos
        public ICommand AddTopologyCommand { get; private set; }
        public ICommand AddProtocolCommand { get; private set; }
        public ICommand ExpandAllCommand { get; private set; }
        public ICommand CollapseAllCommand { get; private set; }
        public ICommand SaveChangesCommand { get; private set; }

        DevicesExplorerViewModel DevicesExplorerViewModel { get; set; }

        public TopologyTreeViewModel(DevicesExplorerViewModel devicesExplorerViewModel)
        {
            TreeNodes = new ObservableCollection<GenericTreeNodeModel>();
            InitializeCommands();
            LoadSampleData(); // Para testing
            DevicesExplorerViewModel = devicesExplorerViewModel;
        }

        public TopologyTreeViewModel(List<TopologyNodeModel> topologyData, DevicesExplorerViewModel devicesExplorerViewModel) 
            : this(devicesExplorerViewModel)
        {
            LoadTopologyData(topologyData);
        }

        private void InitializeCommands()
        {
            AddTopologyCommand = new RelayCommand(AddTopology);
            AddProtocolCommand = new RelayCommand(AddProtocol, CanAddProtocol);
            ExpandAllCommand = new RelayCommand(ExpandAll);
            CollapseAllCommand = new RelayCommand(CollapseAll);
            SaveChangesCommand = new RelayCommand(SaveChanges);
        }

        /// <summary>
        /// Carga datos de tu aplicación en el TreeView
        /// </summary>
        public void LoadTopologyData(List<TopologyNodeModel> topologyData)
        {
            _originalData = topologyData;
            TreeNodes.Clear();

            var mappedNodes = TopologyTreeMapper.MapToTreeView(topologyData);
            foreach (var node in mappedNodes)
            {
                SetupNodeCommands(node);
                TreeNodes.Add(node);
            }
        }

        private void SetupNodeCommands(GenericTreeNodeModel node)
        {
            node.AddChildCommand = new RelayCommand(() => AddChildToNode(node));
            node.DeleteCommand = new RelayCommand(() => DeleteNode(node));
            node.MoveUpCommand = new RelayCommand(() => MoveNode(node, -1));
            node.MoveDownCommand = new RelayCommand(() => MoveNode(node, 1));

            // Configurar comandos para los hijos recursivamente
            foreach (var child in node.Children)
            {
                SetupNodeCommands(child);
            }
        }

        private void AddTopology()
        {
            var newTopology = new TopologyNodeModel
            {
                Id = GetNextTopologyId(),
                Name = $"Nueva Topología {TreeNodes.Count + 1}",
                Type = "General"
            };

            var treeNode = new GenericTreeNodeModel
            {
                Title = newTopology.Name,
                IconPath = TreeNodeIcons.Folder,
                Level = 0,
                Tag = newTopology
            };

            SetupNodeCommands(treeNode);
            TreeNodes.Add(treeNode);
        }

        private void AddProtocol()
        {
            if (SelectedNode?.Tag is TopologyNodeModel selectedTopology)
            {
                AddProtocolToTopology(selectedTopology, SelectedNode);
            }
        }

        private bool CanAddProtocol()
        {
            return SelectedNode?.Tag is TopologyNodeModel;
        }

        private void AddChildToNode(GenericTreeNodeModel parentNode)
        {
            if (parentNode.Tag is TopologyNodeModel topology)
            {
                // Determinar si agregar topología o protocolo
                //var shouldAddProtocol = ShouldAddProtocol(topology);

                //if (shouldAddProtocol)
                //{
                //    AddProtocolToTopology(topology, parentNode);
                //}
                //else
                //{
                    AddSubTopology(topology, parentNode);
                //}
            }
        }

        private void AddProtocolToTopology(TopologyNodeModel topology, GenericTreeNodeModel parentNode)
        {
            var newProtocol = new ProtocolNodeModel
            {
                Id = GetNextProtocolId(),
                Name = $"Nuevo Protocolo {topology.Protocols.Count + 1}",
                Type = "IEC-61850",
                Parent = topology
            };

            topology.Protocols.Add(newProtocol);

            var protocolTreeNode = new GenericTreeNodeModel
            {
                Title = newProtocol.Name,
                IconPath = TreeNodeIcons.FileNetwork,
                Level = parentNode.Level + 1,
                Tag = newProtocol,
                Parent = parentNode
            };

            SetupNodeCommands(protocolTreeNode);
            parentNode.Children.Add(protocolTreeNode);
            parentNode.IsExpanded = true;
        }

        private void AddSubTopology(TopologyNodeModel parentTopology, GenericTreeNodeModel parentNode)
        {
            var newSubTopology = new TopologyNodeModel
            {
                Id = GetNextTopologyId(),
                Name = $"Sub-Topología {parentTopology.Topologies.Count + 1}",
                Type = "Bahia",
                Parent = parentTopology
            };

            parentTopology.Topologies.Add(newSubTopology);

            var subTopologyTreeNode = new GenericTreeNodeModel
            {
                Title = newSubTopology.Name,
                IconPath = TreeNodeIcons.None,
                Level = parentNode.Level + 1,
                Tag = newSubTopology,
                Parent = parentNode
            };

            SetupNodeCommands(subTopologyTreeNode);
            parentNode.Children.Add(subTopologyTreeNode);
            parentNode.IsExpanded = true;
        }

        private bool ShouldAddProtocol(TopologyNodeModel topology)
        {
            // Lógica para decidir si agregar protocolo o sub-topología
            // Por ejemplo, si ya tiene 2+ niveles de profundidad, agregar protocolo
            return topology.Type?.ToLower() == "bahia" ||
                   (topology.Topologies.Count > 0 && topology.Protocols.Count < 5);
        }

        private void DeleteNode(GenericTreeNodeModel node)
        {
            if (node.Tag is TopologyNodeModel topology && topology.Parent != null)
            {
                topology.Parent.Topologies.Remove(topology);
            }
            else if (node.Tag is ProtocolNodeModel protocol && protocol.Parent != null)
            {
                protocol.Parent.Protocols.Remove(protocol);
            }

            // Remover del TreeView
            if (node.Parent != null)
            {
                node.Parent.Children.Remove(node);
            }
            else
            {
                TreeNodes.Remove(node);
            }
        }

        private void MoveNode(GenericTreeNodeModel node, int direction)
        {
            var collection = node.Parent?.Children ?? TreeNodes;
            var currentIndex = collection.IndexOf(node);
            var newIndex = currentIndex + direction;

            if (newIndex >= 0 && newIndex < collection.Count)
            {
                collection.Move(currentIndex, newIndex);

                // También mover en el modelo de dominio
                MoveInDomainModel(node, direction);
            }
        }

        private void MoveInDomainModel(GenericTreeNodeModel node, int direction)
        {
            if (node.Tag is TopologyNodeModel topology && topology.Parent != null)
            {
                var currentIndex = topology.Parent.Topologies.IndexOf(topology);
                var newIndex = currentIndex + direction;

                if (newIndex >= 0 && newIndex < topology.Parent.Topologies.Count)
                {
                    topology.Parent.Topologies.RemoveAt(currentIndex);
                    topology.Parent.Topologies.Insert(newIndex, topology);
                }
            }
            else if (node.Tag is ProtocolNodeModel protocol && protocol.Parent != null)
            {
                var currentIndex = protocol.Parent.Protocols.IndexOf(protocol);
                var newIndex = currentIndex + direction;

                if (newIndex >= 0 && newIndex < protocol.Parent.Protocols.Count)
                {
                    protocol.Parent.Protocols.RemoveAt(currentIndex);
                    protocol.Parent.Protocols.Insert(newIndex, protocol);
                }
            }
        }

        private void ExpandAll()
        {
            foreach (var node in TreeNodes)
            {
                ExpandNodeRecursively(node);
            }
        }

        private void CollapseAll()
        {
            foreach (var node in TreeNodes)
            {
                CollapseNodeRecursively(node);
            }
        }

        private void ExpandNodeRecursively(GenericTreeNodeModel node)
        {
            node.IsExpanded = true;
            foreach (var child in node.Children)
            {
                ExpandNodeRecursively(child);
            }
        }

        private void CollapseNodeRecursively(GenericTreeNodeModel node)
        {
            node.IsExpanded = false;
            foreach (var child in node.Children)
            {
                CollapseNodeRecursively(child);
            }
        }

        private void SaveChanges()
        {
            // Convertir de vuelta a tu modelo de dominio
            var updatedData = TopologyTreeMapper.MapFromTreeView(TreeNodes);

            // Aquí llamarías a tu servicio/repositorio para guardar
            OnDataChanged?.Invoke(updatedData);
        }

        private int GetNextTopologyId()
        {
            return TreeNodes.Count > 0 ?
                TreeNodes.Max(n => n.Tag is TopologyNodeModel t ? t.Id : 0) + 1 : 1;
        }

        private int GetNextProtocolId()
        {
            var allProtocols = GetAllProtocols(TreeNodes);
            return allProtocols.Count > 0 ? allProtocols.Max(p => p.Id) + 1 : 1;
        }

        private List<ProtocolNodeModel> GetAllProtocols(ObservableCollection<GenericTreeNodeModel> nodes)
        {
            var protocols = new List<ProtocolNodeModel>();

            foreach (var node in nodes)
            {
                if (node.Tag is ProtocolNodeModel protocol)
                {
                    protocols.Add(protocol);
                }
                protocols.AddRange(GetAllProtocols(node.Children));
            }

            return protocols;
        }

        private void LoadSampleData()
        {
            var sampleData = new List<TopologyNodeModel>
        {
            new TopologyNodeModel
            {
                Id = 1,
                Name = "Subestación",
                Type = "Subestacion",
                Topologies = new List<TopologyNodeModel>
                {
                    new TopologyNodeModel
                    {
                        Id = 2,
                        Name = "Bahía",
                        Type = "Bahia",
                        Protocols = new List<ProtocolNodeModel>
                                {
                                    new ProtocolNodeModel { Id = 1, Name = "IEC-61850", Type = "IEC-61850" },
                                    new ProtocolNodeModel { Id = 2, Name = "FTP", Type = "FTP" },
                                    new ProtocolNodeModel { Id = 3, Name = "TFTP", Type = "TFTP" }
                                }
                    }
                }
            }
        };

            LoadTopologyData(sampleData);
        }

        // Evento para notificar cambios a tu aplicación
        public event Action<List<TopologyNodeModel>> OnDataChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
