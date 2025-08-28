using Axon.UI.Components.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.Comtrade.ViewModel
{
    public class RenameItemModel : BaseViewModel
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        private string _comtradeRenameAs = "{Date},{Time},{Bar},{Protection}";

        public string ComtradeRenameAs
        {
            get { return _comtradeRenameAs; }
            set { _comtradeRenameAs = value; OnPropertyChanged(); }
        }

        private string _dateFormat= "yyyyMMdd";

        public string DateFormat
        {
            get { return _dateFormat; }
            set { _dateFormat = value; OnPropertyChanged(); }
        }

        private string _barCodeNnemonics= "MnemoBar";

        public string BarCodeCodeNnemonics
        {
            get { return _barCodeNnemonics; }
            set { _barCodeNnemonics = value; OnPropertyChanged(); }
        }

        private string _protectionNnemonics= "MnemoProtection";

        public string ProtectionCodeNnemonics
        {
            get { return _protectionNnemonics; }
            set { _protectionNnemonics = value; OnPropertyChanged(); }
        }

        private string _subestationNnemonics= "Substation";

        public string SubestationNnemonics
        {
            get { return _subestationNnemonics; }
            set { _subestationNnemonics = value; OnPropertyChanged(); }
        }

        private string _otherFiles= "{Date},{Time},{Bar},{Protection}Protection}";

        public string OtherFiles
        {
            get { return _otherFiles; }
            set { _otherFiles = value; OnPropertyChanged(); }
        }

        private string _hourFormat= "HHmmssfff";

        public string HourFormat
        {
            get { return _hourFormat; }
            set { _hourFormat = value; OnPropertyChanged(); }
        }



    }
}
