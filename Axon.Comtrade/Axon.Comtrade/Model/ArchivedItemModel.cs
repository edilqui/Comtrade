using Axon.Comtrade.Model;
using Axon.UI.Components.Base;
using Axon.UI.Components.TreeNode;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.Comtrade.ViewModel
{
    public class ArchivedItemModel : BaseViewModel
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        public ObservableCollection<GenericTreeNodeModel> TreeNodes { get; set; }
        private GenericTreeNodeModel _selectedNode;
        public GenericTreeNodeModel SelectedNode
        {
            get => _selectedNode;
            set
            {
                _selectedNode = value;
                OnPropertyChanged();
            }
        }
        public ArchivedItemModel()
        {
            TreeNodes = new ObservableCollection<GenericTreeNodeModel>();
            LoadSampleData();

        }
        private void LoadSampleData()
        {
            var topologyBahia = new GenericTreeNodeModel
            {
                Title = "System",
            };
            topologyBahia.Children = new ObservableCollection<GenericTreeNodeModel>
                                {
                                    new GenericTreeNodeModel { Title = "Ajustes", Parent = topologyBahia , IconPath = TreeNodeIcons.FolderOutline},
                                    new GenericTreeNodeModel { Title = "Eventos", Parent = topologyBahia, IconPath = TreeNodeIcons.FolderOutline },
                                    new GenericTreeNodeModel { Title = "Reportes",Parent = topologyBahia, IconPath = TreeNodeIcons.FolderOutline },
                                    new GenericTreeNodeModel { Title = "Oscilografías",Parent = topologyBahia, IconPath = TreeNodeIcons.FolderOutline }
                                };

            TreeNodes.Add(topologyBahia);
            LoadTopologyData(TreeNodes);
        }

        public void LoadTopologyData(ObservableCollection<GenericTreeNodeModel> topologyData)
        {
            //List<GenericTreeNodeModel> temp = new List<GenericTreeNodeModel>();

            //foreach (var node in topologyData)
            //{
            //    SetupNodeCommands(node);
            //    temp.Add(node);
            //}

            foreach (var node in topologyData)
            {
                SetupNodeCommands(node);
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
            //throw new NotImplementedException();
        }

        private void AddChildToNode(GenericTreeNodeModel node)
        {
            var newNode = new GenericTreeNodeModel()
            {
                Title = node.Title + "-" + (node.Children.Count + 1),
                Parent = node,
                Level = node.Level + 1,
                IconPath = node.IconPath,
            };
            SetupNodeCommands(newNode);
            node.Children.Add(newNode);
            node.IsExpanded = true;
        }
    }
}
