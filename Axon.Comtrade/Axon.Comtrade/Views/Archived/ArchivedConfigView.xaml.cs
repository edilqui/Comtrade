using Axon.Comtrade.ViewModel;
using Axon.UI.Components;
using Axon.UI.Components.TreeNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Axon.Comtrade.Views
{
    /// <summary>
    /// Interaction logic for DevicesExplorer.xaml
    /// </summary>
    public partial class ArchivedConfigView : UserControl
    {
        public ArchivedConfigView()
        {
            InitializeComponent();
        }

        private void TreeDir_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is ArchivedListViewModel viewModel)
            {
                viewModel.ArchivedItemSelected.SelectedNode = (GenericTreeNodeModel)e.NewValue;
            }
        }
    }
}
