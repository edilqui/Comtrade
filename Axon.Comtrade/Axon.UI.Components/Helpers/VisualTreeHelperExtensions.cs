using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace Axon.UI.Components
{
    /// <summary>
    /// Extensiones para VisualTreeHelper que facilitan la búsqueda de elementos en el árbol visual
    /// </summary>
    public static class VisualTreeHelperExtensions
    {
        /// <summary>
        /// Busca el primer hijo del tipo especificado en el árbol visual
        /// </summary>
        public static T FindChild<T>(this DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T result)
                    return result;

                var childOfChild = FindChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }

            return null;
        }

        /// <summary>
        /// Busca el primer hijo con el nombre especificado
        /// </summary>
        public static T FindChild<T>(this DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild && (child as FrameworkElement)?.Name == childName)
                    return typedChild;

                var childOfChild = FindChild<T>(child, childName);
                if (childOfChild != null)
                    return childOfChild;
            }

            return null;
        }

        /// <summary>
        /// Busca el primer padre del tipo especificado en el árbol visual
        /// </summary>
        public static T FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;
            if (parentObject is T parent) return parent;

            return FindParent<T>(parentObject);
        }

        /// <summary>
        /// Busca todos los hijos del tipo especificado
        /// </summary>
        public static System.Collections.Generic.List<T> FindChildren<T>(this DependencyObject parent) where T : DependencyObject
        {
            var result = new System.Collections.Generic.List<T>();
            if (parent == null) return result;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild)
                    result.Add(typedChild);

                result.AddRange(FindChildren<T>(child));
            }

            return result;
        }
    }
}
