using Axon.UI.Components.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.Comtrade.ViewModel
{
    public class DeviceViewModel:BaseViewModel
    {
        public string Name { get; set; }
        public string Ip { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 555;
    }
}
