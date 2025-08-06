using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Collections.Specialized;

namespace Axon.UI.Components.TreeNode
{
    public class TreeNodeModel : INotifyPropertyChanged
    {
        public TreeNodeModel()
        {
            Children = new ObservableCollection<TreeNodeModel>();
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _title;
        private bool _isExpanded;
        private bool _isSelected;
        private bool _isMouseOver;
        private TreeNodeType _nodeType;

        public ICommand AddChildCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand MoveUpCommand { get; set; }
        public ICommand MoveDownCommand { get; set; }

        private ObservableCollection<TreeNodeModel> _children;

        public ObservableCollection<TreeNodeModel> Children
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

        private void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HasChildren));
        }

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

        public bool IsMouseOver
        {
            get => _isMouseOver;
            set => SetProperty(ref _isMouseOver, value);
        }

        public TreeNodeType NodeType
        {
            get => _nodeType;
            set => SetProperty(ref _nodeType, value);
        }

        public int _level;
        public int Level
        {
            get => _level;
            set => SetProperty(ref _level, value);
        }


        public TreeNodeModel Parent { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    public enum TreeNodeType
    {
        Folder,
        Device,
        Protocol
    }
}


