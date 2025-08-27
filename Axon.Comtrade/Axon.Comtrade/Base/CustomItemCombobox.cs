using Axon.UI.Components.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.Comtrade.Base
{
    public class CustomItemCombobox : BaseViewModel
    {
        private string _display;

        public string Display
        {
            get { return _display; }
            set { _display = value; OnPropertyChanged(); }
        }

        private object _value;

        public object Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged(); }
        }

    }
}
