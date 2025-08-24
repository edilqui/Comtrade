using Axon.UI.Components.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.ComponentModel;
using Axon.Comtrade.Model;
using Axon.UI.Components.TreeNode;

namespace Axon.Comtrade.ViewModel
{
    public class DevicesExplorerViewModel : BaseViewModel
    {
        private ObservableCollection<DeviceViewModel> _allDevices;
        private ICollectionView _filteredDevices;
        private string _selectedTopologyPath;

        public ObservableCollection<DeviceViewModel> AllDevices
        {
            get => _allDevices;
            set
            {
                _allDevices = value;
                SetupFilteredView();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Vista filtrada de dispositivos según la topología seleccionada
        /// </summary>
        public ICollectionView FilteredDevices
        {
            get => _filteredDevices;
            private set
            {
                _filteredDevices = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Ruta de topología actualmente seleccionada para filtrar
        /// </summary>
        public string SelectedTopologyPath
        {
            get => _selectedTopologyPath;
            set
            {
                _selectedTopologyPath = value;
                ApplyTopologyFilter();
                OnPropertyChanged();
            }
        }

        public ComtradeConfiguration ComtradeController { get; set; }
        public DevicesExplorerViewModel(ComtradeConfiguration comtradeController)
        {
            InitializeDevices();
            SetupFilteredView();
            ComtradeController= comtradeController;
        }

        private void InitializeDevices()
        {
            AllDevices = new ObservableCollection<DeviceViewModel>
            {
                // Dispositivos IEC-61850
                new DeviceViewModel
                {
                    Name = "IED Dispositivo 001",
                    Ip = "127.0.0.1",
                    Port = 2405,
                    Protocol = "IEC-61850",
                    Topology = "Subestación/Bahía/IEC-61850",
                    IsEnabled = true
                },
                new DeviceViewModel
                {
                    Name = "IED Dispositivo 002",
                    Ip = "127.0.0.1",
                    Port = 2405,
                    Protocol = "IEC-61850",
                    Topology = "Subestación/Bahía/IEC-61850",
                    IsEnabled = true
                },
                new DeviceViewModel
                {
                    Name = "IED Dispositivo 003",
                    Ip = "127.0.0.1",
                    Port = 2405,
                    Protocol = "IEC-61850",
                    Topology = "Subestación/Bahía/IEC-61850",
                    IsEnabled = true
                },
                new DeviceViewModel
                {
                    Name = "IED Dispositivo 004",
                    Ip = "127.0.0.1",
                    Port = 2405,
                    Protocol = "IEC-61850",
                    Topology = "Subestación/Bahía/IEC-61850",
                    IsEnabled = true
                },

                // Dispositivos FTP
                new DeviceViewModel
                {
                    Name = "FTP Server 001",
                    Ip = "192.168.1.100",
                    Port = 21,
                    Protocol = "FTP",
                    Topology = "Subestación/Bahía/FTP",
                    IsEnabled = true
                },
                new DeviceViewModel
                {
                    Name = "FTP Server 002",
                    Ip = "192.168.1.101",
                    Port = 21,
                    Protocol = "FTP",
                    Topology = "Subestación/Bahía/FTP",
                    IsEnabled = false
                },
                new DeviceViewModel
                {
                    Name = "FTP Server 003",
                    Ip = "192.168.1.102",
                    Port = 21,
                    Protocol = "FTP",
                    Topology = "Subestación/Bahía/FTP",
                    IsEnabled = true
                },

                // Dispositivos TFTP
                new DeviceViewModel
                {
                    Name = "TFTP Server 001",
                    Ip = "10.0.0.50",
                    Port = 69,
                    Protocol = "TFTP",
                    Topology = "Subestación/Bahía/TFTP",
                    IsEnabled = true
                },
                new DeviceViewModel
                {
                    Name = "TFTP Server 002",
                    Ip = "10.0.0.51",
                    Port = 69,
                    Protocol = "TFTP",
                    Topology = "Subestación/Bahía/TFTP",
                    IsEnabled = true
                },

                // Otros dispositivos
                new DeviceViewModel
                {
                    Name = "MODBUS Device",
                    Ip = "172.16.0.10",
                    Port = 502,
                    Protocol = "MODBUS",
                    Topology = "Subestación/Otros",
                    IsEnabled = false
                },
            };
        }

        private void SetupFilteredView()
        {
            if (AllDevices != null)
            {
                FilteredDevices = CollectionViewSource.GetDefaultView(AllDevices);
                FilteredDevices.Filter = DeviceFilter;
            }
        }

        private bool DeviceFilter(object item)
        {
            if (item is DeviceViewModel device)
            {
                // Si no hay topología seleccionada, mostrar todos los dispositivos
                if (string.IsNullOrEmpty(SelectedTopologyPath))
                    return true;

                // Filtrar por topología
                return device.BelongsToTopology(SelectedTopologyPath);
            }
            return false;
        }

        /// <summary>
        /// Aplica el filtro de topología a la vista de dispositivos
        /// </summary>
        private void ApplyTopologyFilter()
        {
            FilteredDevices?.Refresh();

            // Actualizar la grilla con los dispositivos filtrados
            if (GridExample != null)
            {
                GridExample.UpdateDevicesFromTopology(GetFilteredDeviceItems());
                GridExample.SetTopologyFilter(SelectedTopologyPath);
            }
        }

        /// <summary>
        /// Convierte los dispositivos filtrados al formato requerido por el DataGrid
        /// </summary>
        private List<DeviceViewModel> GetFilteredDeviceItems()
        {
            var filteredItems = new List<DeviceViewModel>();

            if (FilteredDevices != null)
            {
                foreach (DeviceViewModel device in FilteredDevices)
                {
                    filteredItems.Add(new DeviceViewModel
                    {
                        IsEnabled = device.IsEnabled,
                        Name = device.Name,
                        Ip = device.Ip,
                        Port = device.Port,
                        Protocol = device.Protocol,
                        Group = GetGroupFromTopology(device.Topology)
                    });
                }
            }

            return filteredItems;
        }

        /// <summary>
        /// Extrae el grupo del path de topología para el DataGrid
        /// </summary>
        private string GetGroupFromTopology(string topology)
        {
            //if (string.IsNullOrEmpty(topology))
            //    return "Sin Grupo";

            //var parts = topology.Split('/');
            //if (parts.Length >= 2)
            //{
            //    // Tomar los últimos dos niveles (ej: "Bahía/IEC-61850")
            //    return $"{parts[parts.Length - 2]}/{parts[parts.Length - 1]}";
            //}

            return topology;
        }

        /// <summary>
        /// Agrega un nuevo dispositivo a la topología seleccionada
        /// </summary>
        public void AddDevice(string name, string ip, int port, string protocol, GenericTreeNodeModel node = null)
        {
            string topology = "";
            if (node != null)
            {
                topology = GetTopologyPath(node);
            }
            else topology = SelectedTopologyPath ?? "Sin Asignar";

            var newDevice = new DeviceViewModel
            {
                Name = name,
                Ip = ip,
                Port = port,
                Protocol = protocol,
                Topology = topology,
                IsEnabled = true
            };

            AllDevices.Add(newDevice);
            ApplyTopologyFilter();
        }

        /// <summary>
        /// Elimina un dispositivo
        /// </summary>
        public void RemoveDevice(DeviceViewModel device)
        {
            if (device != null && AllDevices.Contains(device))
            {
                AllDevices.Remove(device);
                ApplyTopologyFilter();
            }
        }

        #region ViewModels Anidados

        private TopologyTreeViewModel _tree;
        public TopologyTreeViewModel Tree
        {
            get
            {
                if (_tree == null)
                {
                    _tree = new TopologyTreeViewModel(this);
                    // Suscribirse a cambios de selección en el árbol
                    _tree.PropertyChanged += OnTreePropertyChanged;
                }
                return _tree;
            }
            set
            {
                if (_tree != null)
                    _tree.PropertyChanged -= OnTreePropertyChanged;

                _tree = value;

                if (_tree != null)
                    _tree.PropertyChanged += OnTreePropertyChanged;

                OnPropertyChanged();
            }
        }

        private void OnTreePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TopologyTreeViewModel.SelectedNode))
            {
                UpdateSelectedTopology();
            }
        }

        /// <summary>
        /// Actualiza la topología seleccionada basada en el nodo seleccionado en el árbol
        /// </summary>
        private void UpdateSelectedTopology()
        {
            if (Tree?.SelectedNode?.Tag != null)
            {
                var topologyPath = GetTopologyPath(Tree.SelectedNode);
                SelectedTopologyPath = topologyPath;
            }
            else
            {
                SelectedTopologyPath = null; // Mostrar todos los dispositivos
            }
        }

        /// <summary>
        /// Construye el path completo de topología desde el nodo seleccionado
        /// </summary>
        private string GetTopologyPath(Axon.UI.Components.TreeNode.GenericTreeNodeModel node)
        {
            var pathParts = new List<string>();
            var currentNode = node;

            while (currentNode != null)
            {
                if (currentNode.Tag is Axon.Comtrade.Model.TopologyNodeModel topology)
                {
                    pathParts.Insert(0, topology.Name);
                }
                else if (currentNode.Tag is Axon.Comtrade.Model.ProtocolNodeModel protocol)
                {
                    pathParts.Insert(0, protocol.Name);
                }
                currentNode = currentNode.Parent;
            }

            return string.Join("/", pathParts);
        }

        private DeviceViewModel _deviceConfigSelected;

        public DeviceViewModel DeviceConfigSelected
        {
            get { return _deviceConfigSelected; }
            set { _deviceConfigSelected = value; OnPropertyChanged(); }
        }


        internal void ConfigureDevice(DeviceViewModel device)
        {
            DeviceConfigSelected= device;
            this.ComtradeController.OnMenuItemSelected("deviceContent");
        }

        private DeviceListViewModel _gridExample;
        public DeviceListViewModel GridExample
        {
            get
            {
                if (_gridExample == null)
                {
                    _gridExample = new DeviceListViewModel(this);

                    // Configurar eventos bidireccionales
                    _gridExample.OnTopologyFilterCleared += () =>
                    {
                        Tree?.ClearSelection();
                        SelectedTopologyPath = null;
                    };

                    _gridExample.OnDeviceDeleted += (deviceItem) =>
                    {
                        // Buscar y eliminar el dispositivo correspondiente
                        var deviceToRemove = AllDevices.FirstOrDefault(d =>
                            d.Name == deviceItem.Name &&
                            d.Ip == deviceItem.Ip &&
                            d.Port == deviceItem.Port);

                        if (deviceToRemove != null)
                        {
                            AllDevices.Remove(deviceToRemove);
                        }
                    };

                    // Cargar datos iniciales
                    _gridExample.UpdateDevicesFromTopology(GetFilteredDeviceItems());
                    _gridExample.SetTopologyFilter(SelectedTopologyPath);
                }
                return _gridExample;
            }
            set { _gridExample = value; OnPropertyChanged(); }
        }

        #endregion
    }
}