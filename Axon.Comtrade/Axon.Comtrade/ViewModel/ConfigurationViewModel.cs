using Axon.UI.Components.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.Comtrade.ViewModel
{
    public class ConfigurationViewModel: BaseViewModel
    {
		private bool _unZipFile;

		public bool UnZipFile
		{
			get { return _unZipFile; }
			set { _unZipFile = value; OnPropertyChanged(); }
		}

	}
}
