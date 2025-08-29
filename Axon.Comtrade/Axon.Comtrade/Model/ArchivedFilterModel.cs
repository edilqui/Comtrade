using Axon.UI.Components.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.Comtrade.Model
{
    public class ArchivedFilterModel : BaseViewModel
    {
        private string _filter = "*,*";

        public string Filter
        {
            get { return _filter; }
            set { _filter = value; OnPropertyChanged(); }
        }

    }
}
