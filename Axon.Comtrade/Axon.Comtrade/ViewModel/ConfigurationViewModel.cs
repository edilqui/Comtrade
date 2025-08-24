using Axon.UI.Components.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.Comtrade.ViewModel
{
    public class ConfigurationViewModel : BaseViewModel
    {
        private bool _unZipFile;

        public bool UnZipFile
        {
            get { return _unZipFile; }
            set { _unZipFile = value; OnPropertyChanged(); }
        }

        private bool _converterCEVtoComtrade;

        public bool ConverterCEVtoComtrade
        {
            get { return _converterCEVtoComtrade; }
            set { _converterCEVtoComtrade = value; OnPropertyChanged(); }
        }

        private bool _webServer;

        public bool WebServer
        {
            get { return _webServer; }
            set { _webServer = value; OnPropertyChanged(); }
        }

        private string _patOut;

        public string PatOut
        {
            get { return _patOut; }
            set { _patOut = value; OnPropertyChanged(); }
        }

        private FolderStructure _folderStructure = FolderStructure.ARCHITECTURE; // Valor por defecto
        public FolderStructure FolderStructure
        {
            get { return _folderStructure; }
            set
            {
                _folderStructure = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FolderStructureAsString));
                OnPropertyChanged(nameof(FolderStructureAsInt));

                // Lógica adicional cuando cambia la estructura
                OnFolderStructureChanged(value);
            }
        }

        // Propiedades auxiliares para diferentes tipos de binding
        public string FolderStructureAsString
        {
            get => FolderStructure.ToString();
            set
            {
                if (Enum.TryParse<FolderStructure>(value, out var result))
                {
                    FolderStructure = result;
                }
            }
        }

        public int FolderStructureAsInt
        {
            get => (int)FolderStructure;
            set
            {
                if (Enum.IsDefined(typeof(FolderStructure), value))
                {
                    FolderStructure = (FolderStructure)value;
                }
            }
        }

        private void OnFolderStructureChanged(FolderStructure newStructure)
        {
            // Aquí puedes agregar lógica específica según la estructura seleccionada
            switch (newStructure)
            {
                case FolderStructure.ARCHITECTURE:
                    // Configuración específica para estructura por arquitectura
                    break;
                case FolderStructure.DATE:
                    // Configuración específica para estructura por fecha
                    break;
            }
        }


    }

    public enum FolderStructure
    {
        ARCHITECTURE = 0,
        DATE = 1
    }
}
