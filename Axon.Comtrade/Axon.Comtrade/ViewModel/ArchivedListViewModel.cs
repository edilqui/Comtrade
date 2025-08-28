using Axon.UI.Components.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Axon.Comtrade.ViewModel
{
    public class ArchivedListViewModel : BaseViewModel
    {
        public ObservableCollection<ArchivedItemModel> DataItems { get; set; }

        public ICommand AddCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand GoBackCommand { get; private set; }
        public ComtradeConfiguration ComtradeController { get; set; }

        public ArchivedListViewModel(ComtradeConfiguration _contradeController)
        {
            InitializeCommands();
            DataItems = new ObservableCollection<ArchivedItemModel>();

            LoadExample();
            ComtradeController = _contradeController;

        }

        private void LoadExample()
        {
            DataItems.Add(new ArchivedItemModel() { Name = "Archivado1" });
            DataItems.Add(new ArchivedItemModel() { Name = "Archivado2" });
            DataItems.Add(new ArchivedItemModel() { Name = "Archivado3" });
            DataItems.Add(new ArchivedItemModel() { Name = "Archivado4" });
            DataItems.Add(new ArchivedItemModel() { Name = "Archivado5" });
            OnPropertyChanged("DataItems");

        }

        private void InitializeCommands()
        {
            AddCommand = new DelegateCommand(Add);
            DeleteCommand = new DelegateCommand<ArchivedItemModel>(Delete);
            GoBackCommand = new DelegateCommand(OnGoBack);
        }

        private void OnGoBack()
        {
            ComtradeController.OnMenuItemSelected("archivado");
        }

        private void Delete(ArchivedItemModel obj)
        {
            DataItems.Remove(obj);
        }

        private void Add()
        {
            var Item = new ArchivedItemModel() { Name = "Archivated" + DataItems.Count };
            DataItems.Add(Item);
        }
        public ICommand ConfigureCommand
        {
            get { return new DelegateCommand<ArchivedItemModel>(OnConfigure); }
        }

        private ArchivedItemModel _archivedItemSelected;

        public ArchivedItemModel ArchivedItemSelected
        {
            get { return _archivedItemSelected; }
            set { _archivedItemSelected = value; OnPropertyChanged(); }
        }

        private void OnConfigure(ArchivedItemModel _archiveItem)
        {
            if (_archiveItem != null)
            {
                ArchivedItemSelected = _archiveItem;
                this.ComtradeController.OnMenuItemSelected("archivedItemConfig");
            }
        }
    }
}
