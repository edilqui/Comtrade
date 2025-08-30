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
    public partial class DeviceRulesView : UserControl
    {
        public DeviceRulesView()
        {
            InitializeComponent();
        }

        private void OnFolderStructureChanged(object sender, RadioSelectionChangedEventArgs e)
        {

        }

        private void TreeDir_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is ComtradeConfiguration viewModel)
            {
                viewModel.DevicesExplorerVieModel.DeviceConfigSelected.ArchivedItemSelected.SelectedNode = (GenericTreeNodeModel)e.NewValue;
            }
        }
    }
}
