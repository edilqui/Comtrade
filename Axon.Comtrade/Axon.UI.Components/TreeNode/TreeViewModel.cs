using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Axon.UI.Components.TreeNode
{
    public class TreeViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TreeNodeModel> Nodes { get; set; }

        public TreeViewModel()
        {
            Nodes = new ObservableCollection<TreeNodeModel>();
            InitializeSampleDataWithLevels();
        }

        // Método para inicializar niveles en el TreeViewModel
        private void InitializeSampleDataWithLevels()
        {
            // Crear estructura inicial
            var dispositivos = new TreeNodeModel
            {
                Title = "Dispositivos",
                NodeType = TreeNodeType.Folder,
                IsExpanded = true,
                Level = 0 // Nivel raíz
            };

            var subestacion = new TreeNodeModel
            {
                Title = "Subestación",
                NodeType = TreeNodeType.Device,
                IsExpanded = true,
                Parent = dispositivos,
                Level = 1 // Primer nivel
            };

            var bahia = new TreeNodeModel
            {
                Title = "Bahía",
                NodeType = TreeNodeType.Device,
                IsExpanded = true,
                Parent = subestacion,
                Level = 2 // Segundo nivel
            };

            // Agregar protocolos con nivel 3
            var protocolos = new[]
            {
        new TreeNodeModel {
            Title = "61 IEC-61850",
            NodeType = TreeNodeType.Protocol,
            Parent = bahia,
            Level = 3
        },
        new TreeNodeModel {
            Title = "Ft FTP",
            NodeType = TreeNodeType.Protocol,
            Parent = bahia,
            Level = 3
        },
        new TreeNodeModel {
            Title = "Tf TFTP",
            NodeType = TreeNodeType.Protocol,
            Parent = bahia,
            Level = 3
        }
    };

            foreach (var protocolo in protocolos)
            {
                SetupNodeCommands(protocolo);
                bahia.Children.Add(protocolo);
            }

            // Agregar otras bahías con nivel 2
            var otrasBahias = new[]
            {
        new TreeNodeModel {
            Title = "Bahía2",
            NodeType = TreeNodeType.Device,
            Parent = subestacion,
            Level = 2
        },
        new TreeNodeModel {
            Title = "Bahía3",
            NodeType = TreeNodeType.Device,
            Parent = subestacion,
            Level = 2
        },
        new TreeNodeModel {
            Title = "Bahía4",
            NodeType = TreeNodeType.Device,
            Parent = subestacion,
            Level = 2
        }
    };

            // Configurar comandos y agregar nodos
            SetupNodeCommands(bahia);
            subestacion.Children.Add(bahia);

            foreach (var bahiaExtra in otrasBahias)
            {
                SetupNodeCommands(bahiaExtra);
                subestacion.Children.Add(bahiaExtra);
            }

            SetupNodeCommands(subestacion);
            dispositivos.Children.Add(subestacion);

            SetupNodeCommands(dispositivos);
            Nodes.Add(dispositivos);
        }

        // Método helper para calcular niveles automáticamente
        private void CalculateLevelsRecursively(TreeNodeModel node, int level = 0)
        {
            node.Level = level;
            foreach (var child in node.Children)
            {
                CalculateLevelsRecursively(child, level + 1);
            }
        }

        private void SetupNodeCommands(TreeNodeModel node)
        {
            node.AddChildCommand = new RelayCommand(() => AddChild(node));
            node.DeleteCommand = new RelayCommand(() => DeleteNode(node));
            node.MoveUpCommand = new RelayCommand(() => MoveNode(node, -1));
            node.MoveDownCommand = new RelayCommand(() => MoveNode(node, 1));

            foreach (var child in node.Children)
            {
                SetupNodeCommands(child);
            }
        }

        private void AddChild(TreeNodeModel parent)
        {
            var newNode = new TreeNodeModel
            {
                Title = $"Nuevo Nodo {parent.Children.Count + 1}",
                NodeType = TreeNodeType.Device,
                Parent = parent,
                Level = parent.Level + 1
            };

            // Configurar comandos antes de agregar
            SetupNodeCommands(newNode);

            // Agregar a la colección del padre
            parent.Children.Add(newNode);

            // Expandir el padre para mostrar el nuevo hijo
            parent.IsExpanded = true;

            // Debug: Verificar la estructura
            System.Diagnostics.Debug.WriteLine($"Agregado '{newNode.Title}' a '{parent.Title}' (Level {newNode.Level})");
            System.Diagnostics.Debug.WriteLine($"'{parent.Title}' ahora tiene {parent.Children.Count} hijos");

            // Notificar cambios si es necesario
            parent.OnPropertyChanged(nameof(parent.HasChildren));
            parent.OnPropertyChanged(nameof(parent.Children));
        }

        private void DeleteNode(TreeNodeModel node)
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

        private void MoveNode(TreeNodeModel node, int direction)
        {
            var collection = node.Parent?.Children ?? Nodes;
            var currentIndex = collection.IndexOf(node);
            var newIndex = currentIndex + direction;

            if (newIndex >= 0 && newIndex < collection.Count)
            {
                collection.Move(currentIndex, newIndex);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    

    // Implementación simple de RelayCommand
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
