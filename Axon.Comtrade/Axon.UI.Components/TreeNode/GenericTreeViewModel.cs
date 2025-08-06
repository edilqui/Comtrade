using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Axon.UI.Components.TreeNode
{
    public class GenericTreeViewModel : INotifyPropertyChanged
    {
        private GenericTreeNodeModel _selectedNode;

        public ObservableCollection<GenericTreeNodeModel> Nodes { get; set; }

        public GenericTreeNodeModel SelectedNode
        {
            get => _selectedNode;
            set => SetProperty(ref _selectedNode, value);
        }

        // Comandos globales
        public ICommand AddRootNodeCommand { get; private set; }
        public ICommand ExpandAllCommand { get; private set; }
        public ICommand CollapseAllCommand { get; private set; }

        public GenericTreeViewModel()
        {
            Nodes = new ObservableCollection<GenericTreeNodeModel>();
            InitializeCommands();
            InitializeSampleData(); // Para demostración
        }

        private void InitializeCommands()
        {
            AddRootNodeCommand = new RelayCommand(AddRootNode);
            ExpandAllCommand = new RelayCommand(ExpandAll);
            CollapseAllCommand = new RelayCommand(CollapseAll);
        }

        private void InitializeSampleData()
        {
            // Crear nodos de ejemplo con diferentes iconos
            var root = CreateNode("Dispositivos", TreeNodeIcons.None, 0);

            var subestacion = CreateNode("Subestación", TreeNodeIcons.None, 1, root);
            var bahia = CreateNode("Bahía", TreeNodeIcons.None, 2, subestacion);

            // Protocolos con diferentes iconos
            CreateNode("IEC-61850", TreeNodeIcons.FileNetwork, 3, bahia);
            CreateNode("FTP", TreeNodeIcons.FileNetwork, 3, bahia);
            CreateNode("TFTP", TreeNodeIcons.FileNetwork, 3, bahia); // Sin icono

            // Más bahías
            CreateNode("Bahía2", TreeNodeIcons.None, 2, subestacion);
            CreateNode("Bahía3", TreeNodeIcons.None, 2, subestacion);
            CreateNode("Bahía4", TreeNodeIcons.None, 2, subestacion); // Sin icono

            // Expandir nodos iniciales
            root.IsExpanded = true;
            subestacion.IsExpanded = true;
            bahia.IsExpanded = true;
        }

        /// <summary>
        /// Crea un nuevo nodo y lo configura
        /// </summary>
        private GenericTreeNodeModel CreateNode(string title, string iconPath, int level, GenericTreeNodeModel parent = null)
        {
            var node = new GenericTreeNodeModel
            {
                Title = title,
                IconPath = iconPath,
                Level = level,
                Parent = parent
            };

            SetupNodeCommands(node);

            if (parent != null)
            {
                parent.Children.Add(node);
            }
            else
            {
                Nodes.Add(node);
            }

            return node;
        }

        private void SetupNodeCommands(GenericTreeNodeModel node)
        {
            node.AddChildCommand = new RelayCommand(() => AddChild(node));
            node.DeleteCommand = new RelayCommand(() => DeleteNode(node), () => CanDeleteNode(node));
            node.MoveUpCommand = new RelayCommand(() => MoveNode(node, -1));
            node.MoveDownCommand = new RelayCommand(() => MoveNode(node, 1));
        }

        private void AddRootNode()
        {
            var newNode = CreateNode($"Nuevo Elemento {Nodes.Count + 1}", TreeNodeIcons.Folder, 0);
        }

        private void AddChild(GenericTreeNodeModel parent)
        {
            var newNode = new GenericTreeNodeModel
            {
                Title = $"Nuevo Elemento {parent.Children.Count + 1}",
                IconPath = TreeNodeIcons.File, // Icono por defecto
                Level = parent.Level + 1,
                Parent = parent
            };

            SetupNodeCommands(newNode);
            parent.Children.Add(newNode);
            parent.IsExpanded = true;
        }

        private void DeleteNode(GenericTreeNodeModel node)
        {
            if (node.Parent != null)
            {
                node.Parent.Children.Remove(node);
            }
            else
            {
                Nodes.Remove(node);
            }
        }

        private bool CanDeleteNode(GenericTreeNodeModel node)
        {
            // Permitir eliminar cualquier nodo (personalizable según tus reglas)
            return true;
        }

        private void MoveNode(GenericTreeNodeModel node, int direction)
        {
            var collection = node.Parent?.Children ?? Nodes;
            var currentIndex = collection.IndexOf(node);
            var newIndex = currentIndex + direction;

            if (newIndex >= 0 && newIndex < collection.Count)
            {
                collection.Move(currentIndex, newIndex);
            }
        }

        private void ExpandAll()
        {
            foreach (var node in Nodes)
            {
                ExpandNodeRecursively(node);
            }
        }

        private void CollapseAll()
        {
            foreach (var node in Nodes)
            {
                CollapseNodeRecursively(node);
            }
        }

        private void ExpandNodeRecursively(GenericTreeNodeModel node)
        {
            node.IsExpanded = true;
            foreach (var child in node.Children)
            {
                ExpandNodeRecursively(child);
            }
        }

        private void CollapseNodeRecursively(GenericTreeNodeModel node)
        {
            node.IsExpanded = false;
            foreach (var child in node.Children)
            {
                CollapseNodeRecursively(child);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

    // Clase helper para comandos
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void Execute(object parameter)
        {
            _execute();
        }
    }
}