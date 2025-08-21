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

        public ArchivedListViewModel()
        {
            InitializeCommands();
            DataItems = new ObservableCollection<ArchivedItemModel>();

            LoadExample();

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
            AddCommand = new RelayCommand(Add);
            DeleteCommand = new RelayCommand<ArchivedItemModel>(Delete);
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
    }
}
