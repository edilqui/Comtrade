using Axon.Comtrade.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Axon.Comtrade
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Crear y mostrar manualmente la ventana principal
            var mainWindow = new MainWindow(new ComtradeConfiguration());
            mainWindow.Show();
        }
    }
}
