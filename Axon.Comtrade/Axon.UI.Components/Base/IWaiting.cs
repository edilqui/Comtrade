using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Axon.UI.Components.Base
{    public interface IWaiting
    {
        Visibility Waiting { get; set; }

        string WaitMessage { get; set; }

        int Loop { get; set; }
    }
}
