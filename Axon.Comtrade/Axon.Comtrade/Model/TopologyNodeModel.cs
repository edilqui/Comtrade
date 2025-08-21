using Axon.UI.Components.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Axon.Comtrade.Model
{
    public class TopologyNodeModel : BaseViewModel
    {
        private string _name;

        public int Id { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Type { get; set; }
        public List<TopologyNodeModel> Topologies { get; set; } = new List<TopologyNodeModel>();
        public List<ProtocolNodeModel> Protocols { get; set; } = new List<ProtocolNodeModel>();

        // Propiedades adicionales para el TreeView
        public TopologyNodeModel Parent { get; set; }

        internal bool ContainProtocol(string name)
        {
            foreach (var node in Protocols)
            {
                if (node.Name == name)
                    return true;
            }
            return false;
        }
    }
}
