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
    public class RenameListViewModel : BaseViewModel
    {
        public ObservableCollection<RenameItemModel> DataItems { get; set; }

        public ICommand AddCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public RenameListViewModel()
        {
            InitializeCommands();
            DataItems = new ObservableCollection<RenameItemModel>();

            LoadExample();

        }

        private void LoadExample()
        {
            DataItems.Add(new RenameItemModel() { Name = "Rename1" });
            DataItems.Add(new RenameItemModel() { Name = "Rename2" });
            DataItems.Add(new RenameItemModel() { Name = "Rename3" });
            DataItems.Add(new RenameItemModel() { Name = "Rename4" });
            DataItems.Add(new RenameItemModel() { Name = "Rename5" });
            OnPropertyChanged("DataItems");

        }

        private void InitializeCommands()
        {
            AddCommand = new DelegateCommand(Add);
            DeleteCommand = new DelegateCommand<RenameItemModel>(Delete);
        }

        private void Delete(RenameItemModel obj)
        {
            DataItems.Remove(obj);
        }

        private void Add()
        {
            var Item = new RenameItemModel() { Name = "Rename" + DataItems.Count };
            DataItems.Add(Item);
        }
    }
}
