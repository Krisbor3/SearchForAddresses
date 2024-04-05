using System;
using System.Configuration;
using System.Windows;

namespace SearchForAddresses
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Esri.ArcGISRuntime.ArcGISRuntimeEnvironment.ApiKey = ConfigurationManager.AppSettings.Get("Api_Key");
        }

    }
}
