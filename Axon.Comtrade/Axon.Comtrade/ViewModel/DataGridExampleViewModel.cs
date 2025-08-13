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
        private bool _showGrouping = true; // ✅ CAMBIO: Habilitado por defecto
        private int _selectedItemsCount;
        private ICollectionView _dataView;

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

        // Comandos
        public ICommand ToggleFiltersCommand { get; private set; }
        public ICommand ToggleGroupingCommand { get; private set; }
        public ICommand ExportCommand { get; private set; }
        public ICommand ConfigureCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public DataGridExampleViewModel()
        {
            InitializeCommands();
            LoadSampleData();
            SetupCollectionView();
        }

        private void InitializeCommands()
        {
            ToggleFiltersCommand = new RelayCommand(() => ShowFilters = !ShowFilters);
            ToggleGroupingCommand = new RelayCommand(() => ShowGrouping = !ShowGrouping);
            ExportCommand = new RelayCommand(ExportData);
            ConfigureCommand = new RelayCommand<DeviceItemModel>(ConfigureDevice);
            DeleteCommand = new RelayCommand<DeviceItemModel>(DeleteDevice);
        }

        private void LoadSampleData()
        {
            DataItems = new ObservableCollection<DeviceItemModel>
            {
                // Grupo Bahía/IEC-61850
                new DeviceItemModel { IsEnabled = true, DeviceName = "IED Dispositivo 001", IPAddress = "127.0.0.1", Port = 2405, Protocol = "IEC-61850", Group = "Bahía/IEC-61850" },
                new DeviceItemModel { IsEnabled = true, DeviceName = "IED Dispositivo 002", IPAddress = "127.0.0.1", Port = 2405, Protocol = "IEC-61850", Group = "Bahía/IEC-61850" },
                new DeviceItemModel { IsEnabled = true, DeviceName = "IED Dispositivo 003", IPAddress = "127.0.0.1", Port = 2405, Protocol = "IEC-61850", Group = "Bahía/IEC-61850" },
                new DeviceItemModel { IsEnabled = true, DeviceName = "IED Dispositivo 004", IPAddress = "127.0.0.1", Port = 2405, Protocol = "IEC-61850", Group = "Bahía/IEC-61850" },

                // Grupo Bahía/FTP
                new DeviceItemModel { IsEnabled = true, DeviceName = "FTP Server 001", IPAddress = "192.168.1.100", Port = 21, Protocol = "FTP", Group = "Bahía/FTP" },
                new DeviceItemModel { IsEnabled = false, DeviceName = "FTP Server 002", IPAddress = "192.168.1.101", Port = 21, Protocol = "FTP", Group = "Bahía/FTP" },
                new DeviceItemModel { IsEnabled = true, DeviceName = "FTP Server 003", IPAddress = "192.168.1.102", Port = 21, Protocol = "FTP", Group = "Bahía/FTP" },

                // Grupo Bahía/TFTP
                new DeviceItemModel { IsEnabled = true, DeviceName = "TFTP Server 001", IPAddress = "10.0.0.50", Port = 69, Protocol = "TFTP", Group = "Bahía/TFTP" },
                new DeviceItemModel { IsEnabled = true, DeviceName = "TFTP Server 002", IPAddress = "10.0.0.51", Port = 69, Protocol = "TFTP", Group = "Bahía/TFTP" },

                // Sin agrupar
                new DeviceItemModel { IsEnabled = false, DeviceName = "MODBUS Device", IPAddress = "172.16.0.10", Port = 502, Protocol = "MODBUS", Group = "Otros" },
            };

            OnPropertyChanged(nameof(ItemCount));
        }

        private void SetupCollectionView()
        {
            _dataView = CollectionViewSource.GetDefaultView(DataItems);
            UpdateGrouping();
        }

        private void UpdateGrouping()
        {
            if (_dataView != null)
            {
                _dataView.GroupDescriptions.Clear();

                if (ShowGrouping)
                {
                    _dataView.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
                    // ✅ CAMBIO: Forzar refresh después de agrupar
                    _dataView.Refresh();
                }

                DiagnoseGrouping();
            }
        }

        public void DiagnoseGrouping()
        {
            System.Diagnostics.Debug.WriteLine("=== DIAGNÓSTICO DE AGRUPACIÓN ===");
            System.Diagnostics.Debug.WriteLine($"ShowGrouping: {ShowGrouping}");
            System.Diagnostics.Debug.WriteLine($"DataItems.Count: {DataItems?.Count ?? 0}");

            if (_dataView != null)
            {
                System.Diagnostics.Debug.WriteLine($"GroupDescriptions.Count: {_dataView.GroupDescriptions.Count}");

                foreach (var groupDesc in _dataView.GroupDescriptions)
                {
                    if (groupDesc is PropertyGroupDescription pgd)
                    {
                        System.Diagnostics.Debug.WriteLine($"Agrupando por: {pgd.PropertyName}");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Groups in CollectionView: {_dataView.Groups?.Count ?? 0}");

                if (_dataView.Groups != null)
                {
                    foreach (CollectionViewGroup group in _dataView.Groups)
                    {
                        System.Diagnostics.Debug.WriteLine($"Grupo: {group.Name}, Items: {group.ItemCount}");
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine("=== FIN DIAGNÓSTICO ===");
        }

        // ✅ CAMBIO: Método público para configurar agrupación
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
            // Implementar exportación a Excel/CSV
            try
            {
                // Ejemplo de exportación básica
                var data = new System.Text.StringBuilder();
                data.AppendLine("Enabled,Dispositivo,IP,Port,Protocolo");

                foreach (var item in DataItems)
                {
                    data.AppendLine($"{item.IsEnabled},{item.DeviceName},{item.IPAddress},{item.Port},{item.Protocol}");
                }

                // Guardar archivo
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    DefaultExt = "csv"
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
                // Abrir ventana de configuración
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
                }
            }
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

    // Modelo de datos para los elementos del DataGrid
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

    // ✅ CAMBIO: Agregar RelayCommand sin parámetros
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

    // Comando helper reutilizable
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