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

        public ComtradeConfiguration()
        {

        }

        private ConfigurationViewModel _configurationViewModel;

        public ConfigurationViewModel ConfigurationViewModel
        {
            get
            {
                if (_configurationViewModel == null) _configurationViewModel = new ConfigurationViewModel();
                return _configurationViewModel;
            }
            set { _configurationViewModel = value; OnPropertyChanged(); }
        }



        private DevicesExplorerViewModel _devicesExplorerViewModel;

        public DevicesExplorerViewModel DevicesExplorerVieModel
        {
            get
            {
                if (_devicesExplorerViewModel == null) _devicesExplorerViewModel = new DevicesExplorerViewModel(this);

                return _devicesExplorerViewModel;
            }
            set { _devicesExplorerViewModel = value; }
        }

        private ArchivedListViewModel _archivedListViewModel;

        public ArchivedListViewModel ArchivedListViewModel
        {
            get
            {
                if (_archivedListViewModel == null)
                    _archivedListViewModel = new ArchivedListViewModel();
                return _archivedListViewModel;
            }
            set { _archivedListViewModel = value; }
        }

        private RenameListViewModel _renameListViewModel;

        public RenameListViewModel RenameListViewModel
        {
            get
            {
                if (_renameListViewModel == null)
                    _renameListViewModel = new RenameListViewModel();
                return _renameListViewModel;
            }
            set { _renameListViewModel = value; }
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
            get
            {
                if (_sideBar == null) _sideBar = new SideBarView(this);
                return _sideBar;
            }
            set { _sideBar = value; }
        }

        private DevicesExplorerView _devicesExplorerView;
        public DevicesExplorerView DevicesExplorerView
        {
            get
            {
                if (_devicesExplorerView == null)
                    _devicesExplorerView = new DevicesExplorerView() { DataContext = this };
                return _devicesExplorerView;
            }
            set { _devicesExplorerView = value; }
        }

        private DeviceContentView _deviceContentView;
        public DeviceContentView DeviceContentView
        {
            get
            {
                if (_deviceContentView == null)
                    _deviceContentView = new DeviceContentView() { DataContext = this };
                return _deviceContentView;
            }
            set { _deviceContentView = value; }
        }

        private ArchivedListView _archivedListView;
        public ArchivedListView ArchivedListView
        {
            get
            {
                if (_archivedListView == null)
                    _archivedListView = new ArchivedListView() { DataContext = this };
                return _archivedListView;
            }
            set { _archivedListView = value; }
        }

        private RenameListView _renameListView;
        public RenameListView RenameListView
        {
            get
            {
                if (_renameListView == null)
                    _renameListView = new RenameListView() { DataContext = this };
                return _renameListView;
            }
            set { _renameListView = value; }
        }

        private ConfigurationView _configurationView;
        public ConfigurationView ConfigurationView
        {
            get
            {
                if (_configurationView == null)
                    _configurationView = new ConfigurationView() { DataContext = this.ConfigurationViewModel };
                return _configurationView;
            }
            set { _configurationView = value; }
        }

        public void OnMenuItemSelected(string name)
        {
            switch (name)
            {
                case "devices":
                    this.RootContent = DevicesExplorerView;
                    break;
                case "deviceContent":
                    this.RootContent = DeviceContentView;
                    break;
                case "archivado":
                    this.RootContent = ArchivedListView;
                    break;
                case "renombrado":
                    this.RootContent = RenameListView;
                    break;
                case "Configuracion":
                    this.RootContent = ConfigurationView;
                    break;
                default:
                    var emptyPage = new EmptyPageView();
                    emptyPage.UpdateContent(
                        name,
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
            get
            {
                return new DelegateCommand<string>(OnSideBarSelected);
            }
        }

        private void OnSideBarSelected(string obj)
        {
            OnMenuItemSelected(obj);
        }
    }
}
