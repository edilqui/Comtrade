using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Axon.Comtrade.Model
{
    public class TopologyNodeModel : INotifyPropertyChanged
    {
        private string _name;

        public int Id { get; set; }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Type { get; set; }
        public List<TopologyNodeModel> Topologies { get; set; } = new List<TopologyNodeModel>();
        public List<ProtocolNodeModel> Protocols { get; set; } = new List<ProtocolNodeModel>();

        // Propiedades adicionales para el TreeView
        public TopologyNodeModel Parent { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(backingStore, value))
            {
                backingStore = value;
                OnPropertyChanged(propertyName);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
