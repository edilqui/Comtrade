using Axon.UI.Components.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.Comtrade.ViewModel
{
    public class DevicesExplorerViewModel:BaseViewModel
    {
        public ObservableCollection<DeviceViewModel> Devices { get; set; }
        public DevicesExplorerViewModel()
        {
            Devices = new ObservableCollection<DeviceViewModel>();
            Devices.Add(new DeviceViewModel() { Name = "Device1" });
            Devices.Add(new DeviceViewModel() { Name = "Device2" });
            Devices.Add(new DeviceViewModel() { Name = "Device3" });
            OnPropertyChanged("Devices");
        }

        private TopologyTreeViewModel _tree;

        public TopologyTreeViewModel Tree
        {
            get
            {
                if (_tree == null) _tree = new TopologyTreeViewModel(this);
                return _tree;
            }
            set { _tree = value; OnPropertyChanged(); }
        }

        private DataGridExampleViewModel _gridExample;

        public DataGridExampleViewModel GridExample
        {
            get
            {
                if (_gridExample == null) _gridExample = new DataGridExampleViewModel();
                return _gridExample;
            }
            set { _gridExample = value; OnPropertyChanged(); }
        }
    }
}
