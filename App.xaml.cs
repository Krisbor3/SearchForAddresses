using Microsoft.Extensions.Configuration;
using System.Windows;

namespace SearchForAddresses
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            IConfigurationBuilder builder = new ConfigurationBuilder()
            .AddUserSecrets<App>();

            IConfigurationRoot configuration = builder.Build();

            string apiKey = configuration["APIKey"];
            Esri.ArcGISRuntime.ArcGISRuntimeEnvironment.ApiKey = apiKey;
        }

    }
}
