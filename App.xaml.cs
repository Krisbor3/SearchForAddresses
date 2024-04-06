using System.Configuration;
using System.Windows;

namespace SearchForAddresses
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Note: it is not best practice to store API keys in source code.
            // The API key is referenced here for the convenience of this tutorial.
            Esri.ArcGISRuntime.ArcGISRuntimeEnvironment.ApiKey = "AAPK5177dbae692f4da6b57951522b63710afAVso_ojlGfIMYaFsKn89oKKlmZlfTfLbSDZWaIwkS7r2-lLqB5BUz9W6SG07noM";
        }

    }
}
