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


        private DevicesExplorerViewModel _devicesExplorerViewModel;

        public DevicesExplorerViewModel DevicesExplorerVieModel
        {
            get {
                if (_devicesExplorerViewModel == null) _devicesExplorerViewModel = new DevicesExplorerViewModel();

                return _devicesExplorerViewModel; 
            }
            set { _devicesExplorerViewModel = value; }
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

        private DevicesExplorerView _devicesExplorerView;
        public DevicesExplorerView DevicesExplorerView
        {
            get
            {
                if (_devicesExplorerView == null) 
                    _devicesExplorerView = new DevicesExplorerView() { DataContext = this};
                return _devicesExplorerView;
            }
            set { _devicesExplorerView = value; }
        }

        public void OnMenuItemSelected(string name)
        {
            switch (name)
            {
                case "devices":
                    this.RootContent = DevicesExplorerView;
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
