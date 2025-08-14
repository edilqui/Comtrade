using Axon.Comtrade.Base;
using Axon.Comtrade.Views;
using Axon.UI.Components.Base;
using Axon.UI.Components.TreeNode;
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
    public class ComtradeConfiguration : BaseViewModel, ISideBar
    {
        public ObservableCollection<DeviceViewModel> Devices { get; set; }
        public ComtradeConfiguration()
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
                if (_tree == null) _tree = new TopologyTreeViewModel();
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

        private FrameworkElement _rootContent;

        public FrameworkElement RootContent
        {
            get { return _rootContent; }
            set { _rootContent = value; OnPropertyChanged(); }
        }


        private SideBarView _sideBar;

        public SideBarView SideBar
        {
            get { 
                if(_sideBar == null) _sideBar = new SideBarView(this);
                return _sideBar;
            }
            set { _sideBar = value; }
        }

        private DevicesExplorer _devicesExplorer;
        public DevicesExplorer DevicesExplorer
        {
            get
            {
                if (_devicesExplorer == null) _devicesExplorer = new DevicesExplorer() { DataContext = this};
                return _devicesExplorer;
            }
            set { _devicesExplorer = value; }
        }

        public void OnMenuItemSelected(string name)
        {
            switch (name)
            {
                case "devices":
                    this.RootContent = DevicesExplorer;
                    break;
                default:
                    var emptyPage = new EmptyPageView();
                    emptyPage.UpdateContent(
                        "Configuración",
                        "La página de configuración está en desarrollo.",
                        true,
                        "Volver al inicio"
                    );
                    this.RootContent = emptyPage;
                    break;
            }
        }

        public ICommand SideBarSelectedCommand
        {
            get {
                return new RelayCommand<string>(OnSideBarSelected);
            }
        }

        private void OnSideBarSelected(string obj)
        {
            OnMenuItemSelected(obj);
        }
    }
}
