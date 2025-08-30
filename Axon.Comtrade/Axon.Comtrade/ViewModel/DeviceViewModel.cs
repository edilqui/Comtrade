using Axon.Comtrade.Base;
using Axon.UI.Components.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Axon.Comtrade.ViewModel
{
    public class DeviceViewModel : BaseViewModel
    {
        private string _name;
        private string _ip = "127.0.0.1";
        private int _port = 555;
        private string _topology;
        private string _protocol;
        private bool _isEnabled = true;

        public DeviceViewModel()
        {
            this.CollectionMode = new ObservableCollection<CustomItemCombobox>()
            {
                new CustomItemCombobox(){ Display = "Trigger", Value = 1},
                new CustomItemCombobox(){ Display = "Cyclic", Value = 2},
                new CustomItemCombobox(){ Display = "Manual", Value = 3},
            };

            this.CollectionModeSelected = CollectionMode[0];

            OnPropertyChanged("CollectionMode");

            this.Filtro = new ObservableCollection<CustomItemCombobox>()
            {
                new CustomItemCombobox(){Display = "Lista Negra", Value = 1},
                new CustomItemCombobox(){Display = "Lista Blanca", Value = 2}
            };
            this.FiltroSelected = Filtro[0];
            OnPropertyChanged("Filtro");
        }

        private ArchivedItemModel _archivedItemSelected;
        public ArchivedItemModel ArchivedItemSelected
        {
            get { return _archivedItemSelected; }
            set { _archivedItemSelected = value; OnPropertyChanged(); }
        }


        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Ip
        {
            get => _ip;
            set { _ip = value; OnPropertyChanged(); }
        }

        public int Port
        {
            get => _port;
            set { _port = value; OnPropertyChanged(); }
        }

        public string Topology
        {
            get => _topology;
            set { _topology = value; OnPropertyChanged(); }
        }


        public string Protocol
        {
            get => _protocol;
            set { _protocol = value; OnPropertyChanged(); }
        }


        public bool IsEnabled
        {
            get => _isEnabled;
            set { _isEnabled = value; OnPropertyChanged(); }
        }

        private string _group;

        public string Group
        {
            get { return _group; }
            set { _group = value; }
        }

        private int _idDevice;

        public int IdDevice
        {
            get { return _idDevice; }
            set { _idDevice = value; OnPropertyChanged(); }
        }

        private string _folderName;

        public string FolderName
        {
            get { return _folderName; }
            set { _folderName = value; OnPropertyChanged(); }
        }

        private int _timeout = 1000;

        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; OnPropertyChanged(); }
        }

        private int _retry = 3;

        public int Retry
        {
            get { return _retry; }
            set { _retry = value; OnPropertyChanged(); }
        }

        private int _waitingTimeAfterInt;

        public int WaitingTimeAfterInt
        {
            get { return _waitingTimeAfterInt; }
            set { _waitingTimeAfterInt = value; OnPropertyChanged(); }
        }

        private int _requestTimeout = 120;

        public int RequiestTimeout
        {
            get { return _requestTimeout; }
            set { _requestTimeout = value; OnPropertyChanged(); }
        }

        private int _waitingTimeAmongRequest;

        public int WaitingTimeAmongRequest
        {
            get { return _waitingTimeAmongRequest; }
            set { _waitingTimeAmongRequest = value; OnPropertyChanged(); }
        }

        private string _serverAppTitle = "1.1.999.1";
        public string ServerAppTitle
        {
            get { return _serverAppTitle; }
            set { _serverAppTitle = value; OnPropertyChanged(); }
        }

        private string _serverAEQualifier = "12";

        public string ServerAEQualifier
        {
            get { return _serverAEQualifier; }
            set { _serverAEQualifier = value; OnPropertyChanged(); }
        }

        private string _selectedSignal;

        public string SelectedSignal
        {
            get { return _selectedSignal; }
            set { _selectedSignal = value; OnPropertyChanged(); }
        }


        public ICommand SearchSignalCommand
        {
            get { return new DelegateCommand(OnSearchSignal); }
        }

        private void OnSearchSignal()
        {
            MessageBox.Show("OnSearchSignal");
            SelectedSignal = "NameSignal";
        }

        public ObservableCollection<CustomItemCombobox> CollectionMode { get; set; }

        private CustomItemCombobox _collectionModeSelected;

        public CustomItemCombobox CollectionModeSelected
        {
            get { return _collectionModeSelected; }
            set
            {
                _collectionModeSelected = value;
                SignalTriggerVisible = (int)value.Value == 1 || (int)value.Value == 3;
            }
        }

        private bool _signalTriggerVisible;
        public bool SignalTriggerVisible
        {
            get { return _signalTriggerVisible; }
            set { _signalTriggerVisible = value; OnPropertyChanged(); }
        }


        public ObservableCollection<CustomItemCombobox> Filtro { get; set; }

        private CustomItemCombobox _filtroSelected;

        public CustomItemCombobox FiltroSelected
        {
            get { return _filtroSelected; }
            set { _filtroSelected = value; }
        }


        /// <summary>
        /// Verifica si este dispositivo pertenece a la topología especificada
        /// </summary>
        /// <param name="topologyPath">Ruta de topología a verificar</param>
        /// <returns>True si el dispositivo pertenece a esa topología</returns>
        public bool BelongsToTopology(string topologyPath)
        {
            if (string.IsNullOrEmpty(Topology) || string.IsNullOrEmpty(topologyPath))
                return false;

            return Topology.StartsWith(topologyPath, StringComparison.OrdinalIgnoreCase);
        }
    }
}