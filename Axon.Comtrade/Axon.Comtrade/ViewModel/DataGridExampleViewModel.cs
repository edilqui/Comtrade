using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Axon.Comtrade.ViewModel
{
    public class DataGridExampleViewModel : INotifyPropertyChanged
    {
        private bool _showFilters = true;
        private bool _showGrouping = true;
        private int _selectedItemsCount;
        private ICollectionView _dataView;
        private string _currentTopologyFilter;

        public ObservableCollection<DeviceItemModel> DataItems { get; set; }

        public bool ShowFilters
        {
            get => _showFilters;
            set => SetProperty(ref _showFilters, value);
        }

        public bool ShowGrouping
        {
            get => _showGrouping;
            set
            {
                SetProperty(ref _showGrouping, value);
                UpdateGrouping();
            }
        }

        public int ItemCount => DataItems?.Count ?? 0;

        public int SelectedItemsCount
        {
            get => _selectedItemsCount;
            set => SetProperty(ref _selectedItemsCount, value);
        }

        /// <summary>
        /// Filtro de topología actual aplicado
        /// </summary>
        public string CurrentTopologyFilter
        {
            get => _currentTopologyFilter;
            set => SetProperty(ref _currentTopologyFilter, value);
        }

        // Comandos
        public ICommand ToggleFiltersCommand { get; private set; }
        public ICommand ToggleGroupingCommand { get; private set; }
        public ICommand ExportCommand { get; private set; }
        public ICommand ConfigureCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand ClearFilterCommand { get; private set; }

        public DataGridExampleViewModel()
        {
            InitializeCommands();
            DataItems = new ObservableCollection<DeviceItemModel>();
            SetupCollectionView();
        }

        private void InitializeCommands()
        {
            ToggleFiltersCommand = new RelayCommand(() => ShowFilters = !ShowFilters);
            ToggleGroupingCommand = new RelayCommand(() => ShowGrouping = !ShowGrouping);
            ExportCommand = new RelayCommand(ExportData);
            ConfigureCommand = new RelayCommand<DeviceItemModel>(ConfigureDevice);
            DeleteCommand = new RelayCommand<DeviceItemModel>(DeleteDevice);
            ClearFilterCommand = new RelayCommand(ClearTopologyFilter);
        }

        /// <summary>
        /// Actualiza la lista de dispositivos desde el filtro de topología
        /// </summary>
        public void UpdateDevicesFromTopology(List<DeviceItemModel> filteredDevices)
        {
            DataItems.Clear();

            if (filteredDevices != null)
            {
                foreach (var device in filteredDevices)
                {
                    DataItems.Add(device);
                }
            }

            OnPropertyChanged(nameof(ItemCount));
            _dataView?.Refresh();
        }

        /// <summary>
        /// Establece el filtro de topología actual
        /// </summary>
        public void SetTopologyFilter(string topologyPath)
        {
            CurrentTopologyFilter = string.IsNullOrEmpty(topologyPath) ?
                "Todos los dispositivos" :
                $"Filtrado por: {topologyPath}";
        }

        /// <summary>
        /// Limpia el filtro de topología
        /// </summary>
        private void ClearTopologyFilter()
        {
            // Este comando podría comunicarse de vuelta con DevicesExplorerViewModel
            // para limpiar la selección del árbol
            CurrentTopologyFilter = "Todos los dispositivos";
            OnTopologyFilterCleared?.Invoke();
        }

        /// <summary>
        /// Evento que se dispara cuando se limpia el filtro de topología
        /// </summary>
        public event Action OnTopologyFilterCleared;

        private void SetupCollectionView()
        {
            if (DataItems != null)
            {
                _dataView = CollectionViewSource.GetDefaultView(DataItems);
                UpdateGrouping();
            }
        }

        private void UpdateGrouping()
        {
            if (_dataView != null)
            {
                _dataView.GroupDescriptions.Clear();

                if (ShowGrouping)
                {
                    _dataView.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
                    _dataView.Refresh();
                }
            }
        }

        public void GroupBy(string propertyName)
        {
            if (_dataView != null)
            {
                _dataView.GroupDescriptions.Clear();
                if (!string.IsNullOrEmpty(propertyName))
                {
                    _dataView.GroupDescriptions.Add(new PropertyGroupDescription(propertyName));
                    ShowGrouping = true;
                }
                else
                {
                    ShowGrouping = false;
                }
                _dataView.Refresh();
            }
        }

        private void ExportData()
        {
            try
            {
                var data = new System.Text.StringBuilder();
                data.AppendLine("Enabled,Dispositivo,IP,Port,Protocolo,Grupo");

                foreach (var item in DataItems)
                {
                    data.AppendLine($"{item.IsEnabled},{item.DeviceName},{item.IPAddress},{item.Port},{item.Protocol},{item.Group}");
                }

                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    DefaultExt = "csv",
                    FileName = $"Dispositivos_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (dialog.ShowDialog() == true)
                {
                    System.IO.File.WriteAllText(dialog.FileName, data.ToString());
                    System.Windows.MessageBox.Show("Datos exportados exitosamente", "Exportación",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error al exportar: {ex.Message}", "Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ConfigureDevice(DeviceItemModel device)
        {
            if (device != null)
            {
                System.Windows.MessageBox.Show($"Configurar dispositivo: {device.DeviceName}", "Configuración",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }

        private void DeleteDevice(DeviceItemModel device)
        {
            if (device != null)
            {
                var result = System.Windows.MessageBox.Show(
                    $"¿Está seguro de eliminar el dispositivo '{device.DeviceName}'?",
                    "Confirmar eliminación",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Question);

                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    DataItems.Remove(device);
                    OnPropertyChanged(nameof(ItemCount));

                    // Notificar la eliminación al DevicesExplorerViewModel
                    OnDeviceDeleted?.Invoke(device);
                }
            }
        }

        /// <summary>
        /// Evento que se dispara cuando se elimina un dispositivo
        /// </summary>
        public event Action<DeviceItemModel> OnDeviceDeleted;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    // Modelo de datos para los elementos del DataGrid (sin cambios)
    public class DeviceItemModel : INotifyPropertyChanged
    {
        private bool _isEnabled;
        private string _deviceName;
        private string _ipAddress;
        private int _port;
        private string _protocol;
        private string _group;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public string DeviceName
        {
            get => _deviceName;
            set => SetProperty(ref _deviceName, value);
        }

        public string IPAddress
        {
            get => _ipAddress;
            set => SetProperty(ref _ipAddress, value);
        }

        public int Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        public string Protocol
        {
            get => _protocol;
            set => SetProperty(ref _protocol, value);
        }

        public string Group
        {
            get => _group;
            set => SetProperty(ref _group, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    // Comandos auxiliares (sin cambios)
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object parameter) => _execute();
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke((T)parameter) ?? true;
        public void Execute(object parameter) => _execute((T)parameter);
    }
}