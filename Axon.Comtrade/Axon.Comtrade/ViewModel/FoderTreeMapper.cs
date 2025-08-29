using System.Collections.Generic;
using System.Collections.ObjectModel;
using Axon.Comtrade.Model;
using Axon.UI.Components.TreeNode;

namespace Axon.Comtrade.ViewModel
{
    

    public class FoderTreeMapper
    {
        /// <summary>
        /// Convierte tu jerarquía de FolderModel a GenericTreeNodeModel para el TreeView
        /// </summary>
        public static ObservableCollection<GenericTreeNodeModel> MapToTreeView(List<FolderModel> folderNodes)
        {
            var result = new ObservableCollection<GenericTreeNodeModel>();

            foreach (var folder in folderNodes)
            {
                var treeNode = MapFolderToTreeNode(folder, 0);
                result.Add(treeNode);
            }

            return result;
        }

        private static GenericTreeNodeModel MapFolderToTreeNode(FolderModel folder, int level)
        {
            var treeNode = new GenericTreeNodeModel
            {
                Title = folder.Name,
                IconPath = TreeNodeIcons.FolderOutline,
                Level = level,
                Tag = folder // ← Asociar tu objeto original
            };

            // Mapear sub-topologías (hijos)
            foreach (var childFolder in folder.Subfolders)
            {
                var childTreeNode = MapFolderToTreeNode(childFolder, level + 1);
                childTreeNode.Parent = treeNode;
                treeNode.Children.Add(childTreeNode);
            }

            return treeNode;
        }


        /// <summary>
        /// Convierte de vuelta el TreeView a tu estructura de dominio
        /// </summary>
        public static List<FolderModel> MapFromTreeView(ObservableCollection<GenericTreeNodeModel> treeNodes)
        {
            var result = new List<FolderModel>();

            foreach (var treeNode in treeNodes)
            {
                if (treeNode.Tag is FolderModel folder)
                {
                    UpdateFolderFromTreeNode(folder, treeNode);
                    result.Add(folder);
                }
            }

            return result;
        }

        private static void UpdateFolderFromTreeNode(FolderModel folder, GenericTreeNodeModel treeNode)
        {
            // Actualizar propiedades básicas
            folder.Name = treeNode.Title;

            // Limpiar y reconstruir listas
            folder.Subfolders.Clear();

            // Procesar hijos
            foreach (var childTreeNode in treeNode.Children)
            {
                if (childTreeNode.Tag is FolderModel childFolder)
                {
                    folder.Subfolders.Add(childFolder);
                    UpdateFolderFromTreeNode(childFolder, childTreeNode);
                }
            }
        }
    }
}
