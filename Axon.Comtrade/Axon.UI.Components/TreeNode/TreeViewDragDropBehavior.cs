using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace Axon.UI.Components.TreeNode
{
    public static class TreeViewDragDropBehavior
    {
        private static TreeViewItem _draggedItem;
        private static Point _startPoint;

        public static readonly DependencyProperty EnableDragDropProperty =
            DependencyProperty.RegisterAttached(
                "EnableDragDrop",
                typeof(bool),
                typeof(TreeViewDragDropBehavior),
                new PropertyMetadata(false, OnEnableDragDropChanged));

        public static bool GetEnableDragDrop(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableDragDropProperty);
        }

        public static void SetEnableDragDrop(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableDragDropProperty, value);
        }

        private static void OnEnableDragDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeView treeView)
            {
                if ((bool)e.NewValue)
                {
                    treeView.PreviewMouseLeftButtonDown += TreeView_PreviewMouseLeftButtonDown;
                    treeView.PreviewMouseMove += TreeView_PreviewMouseMove;
                    treeView.Drop += TreeView_Drop;
                    treeView.DragOver += TreeView_DragOver;
                    treeView.AllowDrop = true;
                }
                else
                {
                    treeView.PreviewMouseLeftButtonDown -= TreeView_PreviewMouseLeftButtonDown;
                    treeView.PreviewMouseMove -= TreeView_PreviewMouseMove;
                    treeView.Drop -= TreeView_Drop;
                    treeView.DragOver -= TreeView_DragOver;
                    treeView.AllowDrop = false;
                }
            }
        }

        private static void TreeView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
            _draggedItem = FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource);
        }

        private static void TreeView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _draggedItem != null)
            {
                Point currentPosition = e.GetPosition(null);
                Vector diff = _startPoint - currentPosition;

                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    var dragData = new DataObject("TreeNodeModel", _draggedItem.DataContext);
                    DragDrop.DoDragDrop(_draggedItem, dragData, DragDropEffects.Move);
                }
            }
        }

        private static void TreeView_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("TreeNodeModel"))
            {
                e.Effects = DragDropEffects.None;
                return;
            }

            var targetItem = FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource);
            if (targetItem == null || targetItem == _draggedItem)
            {
                e.Effects = DragDropEffects.None;
                return;
            }

            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        private static void TreeView_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("TreeNodeModel"))
                return;

            var targetItem = FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource);
            if (targetItem == null || targetItem == _draggedItem)
                return;

            var draggedNode = (TreeNodeModel)e.Data.GetData("TreeNodeModel");
            var targetNode = (TreeNodeModel)targetItem.DataContext;

            // Prevenir mover un nodo a sí mismo o a sus hijos
            if (IsDescendant(targetNode, draggedNode))
                return;

            // Remover el nodo de su posición actual
            if (draggedNode.Parent != null)
            {
                draggedNode.Parent.Children.Remove(draggedNode);
            }
            else if (sender is TreeView treeView && treeView.DataContext is TreeViewModel viewModel)
            {
                viewModel.Nodes.Remove(draggedNode);
            }

            // Agregar el nodo a la nueva posición
            draggedNode.Parent = targetNode;
            targetNode.Children.Add(draggedNode);
            targetNode.IsExpanded = true;

            _draggedItem = null;
            e.Handled = true;
        }

        private static bool IsDescendant(TreeNodeModel node, TreeNodeModel potentialAncestor)
        {
            var current = node.Parent;
            while (current != null)
            {
                if (current == potentialAncestor)
                    return true;
                current = current.Parent;
            }
            return false;
        }

        private static T FindAncestor<T>(DependencyObject current) where T : class
        {
            do
            {
                if (current is T ancestor)
                    return ancestor;
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }
    }
}
