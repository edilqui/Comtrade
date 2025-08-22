using System;
using System.ComponentModel;
using System.Windows;

namespace Axon.UI.Components
{
    /// <summary>
    /// Representa un elemento individual del tab
    /// </summary>
    public class AxonTabItem : INotifyPropertyChanged
    {
        private string _header;
        private object _content;
        private DataTemplate _contentTemplate;
        private bool _isSelected;
        private bool _isEnabled = true;
        private Visibility _visibility = Visibility.Visible;

        public string Header
        {
            get => _header;
            set
            {
                _header = value;
                OnPropertyChanged(nameof(Header));
            }
        }

        public object Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

        public DataTemplate ContentTemplate
        {
            get => _contentTemplate;
            set
            {
                _contentTemplate = value;
                OnPropertyChanged(nameof(ContentTemplate));
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        public Visibility Visibility
        {
            get => _visibility;
            set
            {
                _visibility = value;
                OnPropertyChanged(nameof(Visibility));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}