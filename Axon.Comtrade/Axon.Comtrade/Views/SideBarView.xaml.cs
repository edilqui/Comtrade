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
    /// Interaction logic for SideBarView.xaml
    /// </summary>
    public partial class SideBarView : UserControl
    {
        public SideBarView()
        {
            InitializeComponent();
        }

        private void MainNavigation_ItemSelected(object sender, UI.Components.Navigation.NavigationItemSelectedEventArgs e)
        {

        }
    }
}
