using Axon.Comtrade.Model;
using Axon.UI.Components.Base;
using Axon.UI.Components.TreeNode;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace Axon.Comtrade.ViewModel
{
    public class ArchivedItemModel : BaseViewModel
    {
        private List<FolderModel> _originalData;

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        public ObservableCollection<GenericTreeNodeModel> TreeNodes { get; set; }
        public ObservableCollection<FolderModel> FolderModelNodes { get; set; }



        private GenericTreeNodeModel _selectedNode;
        public GenericTreeNodeModel SelectedNode
        {
            get => _selectedNode;
            set
            {
                _selectedNode = value;
                if (value.Tag is FolderModel folder)
                    FolderModelSelected = folder;
                OnPropertyChanged();
            }
        }

        private FolderModel _folderModelSelected;

        public FolderModel FolderModelSelected
        {
            get { return _folderModelSelected; }
            set { _folderModelSelected = value; OnPropertyChanged(); }
        }


        public ArchivedItemModel()
        {
            TreeNodes = new ObservableCollection<GenericTreeNodeModel>();
            FolderModelNodes = new ObservableCollection<FolderModel>();

            LoadSampleData();

        }
        private void LoadSampleData()
        {
            var folderSystem = new FolderModel
            {
                Name = "System",
            };
            folderSystem.Subfolders = new List<FolderModel>()
                                {
                                    new FolderModel { Name = "Ajustes", Parent = folderSystem },
                                    new FolderModel { Name = "Eventos", Parent = folderSystem },
                                    new FolderModel { Name = "Reportes",Parent = folderSystem },
                                    new FolderModel { Name = "Oscilografías",Parent = folderSystem }
                                };


            LoadTopologyData(new List<FolderModel>() { folderSystem });
        }

        public void LoadTopologyData(List<FolderModel> folderData)
        {
            _originalData = folderData;
            TreeNodes.Clear();


            var mappedNodes = FoderTreeMapper.MapToTreeView(folderData);
            foreach (var node in mappedNodes)
            {
                SetupNodeCommands(node);
                TreeNodes.Add(node);
            }
        }

        private void SetupNodeCommands(GenericTreeNodeModel node)
        {
            node.AddChildCommand = new DelegateCommand(() => AddChildToNode(node));
            node.DeleteCommand = new DelegateCommand(() => DeleteNode(node));
            node.RenameCommand = new DelegateCommand(() => RenameNode(node));


            // Configurar comandos para los hijos recursivamente
            foreach (var child in node.Children)
            {
                SetupNodeCommands(child);
            }
        }

        private void RenameNode(GenericTreeNodeModel node)
        {
            //throw new NotImplementedException();
        }

        private void DeleteNode(GenericTreeNodeModel node)
        {
            if (node.Tag is FolderModel folder && folder.Parent != null)
            {
                folder.Parent.Subfolders.Remove(folder);
            }

            // Remover del TreeView
            if (node.Parent != null)
            {
                node.Parent.Children.Remove(node);
            }
            //else
            //{
            //    TreeNodes.Remove(node);
            //}
        }

        private void AddChildToNode(GenericTreeNodeModel node)
        {
            var newFolderModel = new FolderModel
            {
                Name = $"Nueva Carpeta"
            };

            var newNode = new GenericTreeNodeModel()
            {
                Title = newFolderModel.Name,
                Parent = node,
                Level = node.Level + 1,
                IconPath = node.IconPath,
                Tag = newFolderModel
            };
            SetupNodeCommands(newNode);

            node.Children.Add(newNode);
            node.IsExpanded = true;
        }


        public ICommand AddFilterCommand
        {
            get { return new DelegateCommand(OnAddFilter); }
        }

        private void OnAddFilter()
        {
            if (SelectedNode != null)
            {
                if (SelectedNode.Tag is FolderModel folder)
                {
                    folder.AddFilter();
                }
            }
        }
    }
}
