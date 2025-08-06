using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Axon.UI.Components.TreeNode
{
    public class GenericTreeNodeModel : INotifyPropertyChanged
    {
        private string _title;
        private bool _isExpanded;
        private bool _isSelected;
        private int _level;
        private string _iconPath;
        private object _tag; // Para asociar cualquier objeto de tu aplicación
        private ObservableCollection<GenericTreeNodeModel> _children;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public int Level
        {
            get => _level;
            set => SetProperty(ref _level, value);
        }

        /// <summary>
        /// Path data para el icono SVG (opcional)
        /// </summary>
        public string IconPath
        {
            get => _iconPath;
            set => SetProperty(ref _iconPath, value);
        }

        /// <summary>
        /// Indica si el nodo tiene icono
        /// </summary>
        public bool HasIcon => !string.IsNullOrEmpty(IconPath);

        /// <summary>
        /// Objeto asociado del dominio de tu aplicación
        /// </summary>
        public object Tag
        {
            get => _tag;
            set => SetProperty(ref _tag, value);
        }

        public ObservableCollection<GenericTreeNodeModel> Children
        {
            get => _children;
            set
            {
                if (_children != null)
                {
                    _children.CollectionChanged -= OnChildrenCollectionChanged;
                }

                SetProperty(ref _children, value);

                if (_children != null)
                {
                    _children.CollectionChanged += OnChildrenCollectionChanged;
                }

                OnPropertyChanged(nameof(HasChildren));
            }
        }

        public bool HasChildren => Children?.Count > 0;

        public GenericTreeNodeModel Parent { get; set; }

        // Comandos
        public ICommand AddChildCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand MoveUpCommand { get; set; }
        public ICommand MoveDownCommand { get; set; }

        public GenericTreeNodeModel()
        {
            Children = new ObservableCollection<GenericTreeNodeModel>();
        }

        private void OnChildrenCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HasChildren));

            // Actualizar Parent en nodos agregados
            if (e.NewItems != null)
            {
                foreach (GenericTreeNodeModel newChild in e.NewItems)
                {
                    newChild.Parent = this;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            // Notificar HasIcon cuando cambie IconPath
            if (propertyName == nameof(IconPath))
            {
                OnPropertyChanged(nameof(HasIcon));
            }
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}