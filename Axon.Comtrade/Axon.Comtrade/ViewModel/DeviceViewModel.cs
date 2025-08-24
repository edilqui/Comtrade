using Axon.UI.Components.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.Comtrade.ViewModel
{
    public class DeviceViewModel : BaseViewModel
    {
        private string _name;
        private string _ip = "127.0.0.1";
        private int _port = 555;
        private string _topology;
        private string _protocol;
        private bool _isEnabled = true;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Ip
        {
            get => _ip;
            set { _ip = value; OnPropertyChanged(); }
        }

        public int Port
        {
            get => _port;
            set { _port = value; OnPropertyChanged(); }
        }

        public string Topology
        {
            get => _topology;
            set { _topology = value; OnPropertyChanged(); }
        }

        
        public string Protocol
        {
            get => _protocol;
            set { _protocol = value; OnPropertyChanged(); }
        }

        
        public bool IsEnabled
        {
            get => _isEnabled;
            set { _isEnabled = value; OnPropertyChanged(); }
        }

        private string _group;

        public string Group
        {
            get { return _group; }
            set { _group = value; }
        }


        /// <summary>
        /// Verifica si este dispositivo pertenece a la topología especificada
        /// </summary>
        /// <param name="topologyPath">Ruta de topología a verificar</param>
        /// <returns>True si el dispositivo pertenece a esa topología</returns>
        public bool BelongsToTopology(string topologyPath)
        {
            if (string.IsNullOrEmpty(Topology) || string.IsNullOrEmpty(topologyPath))
                return false;

            return Topology.StartsWith(topologyPath, StringComparison.OrdinalIgnoreCase);
        }
    }
}