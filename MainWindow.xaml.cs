using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SearchForAddresses
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            MapPoint mapCenterPoint = new MapPoint(-118.805, 34.027, SpatialReferences.Wgs84);
            MainMapView.SetViewpoint(new Viewpoint(mapCenterPoint, 100000));

        }

        private async void SearchAddressButton_Click(object sender, RoutedEventArgs e)
        {
            //Hide suggestions
            HideSuggestions();
            // Get the MapViewModel from the page (defined as a static resource).
            var viewModel = (MapViewModel)FindResource("MapViewModel");
            if (viewModel == null) { return; }

            // Call SearchAddress on the view model, pass the address text and the map view's spatial reference.
            var spatialReference = MainMapView.SpatialReference;
            if (spatialReference == null) { return; }
            MapPoint? addressPoint = await viewModel.SearchAddress(AddressTextBox.Text, spatialReference);

            // If a result was found, center the display on it.
            if (addressPoint != null)
            {
                await MainMapView.SetViewpointCenterAsync(addressPoint);
            }
        }

        private async void SuggestAddress(object sender, TextChangedEventArgs args)
        {
            // Get the MapViewModel from the page (defined as a static resource).
            var viewModel = (MapViewModel)FindResource("MapViewModel");
            if (viewModel == null) { return; }
            ShowSuggestions();
            var sugs = await viewModel.SuggestAddress(AddressTextBox.Text);
            if (!sugs.Any())
            {
                HideSuggestions();
                return;
            }
            SuggestionsListView.Items.Clear();
            foreach (var s in sugs)
            {
                SuggestionsListView.Items.Add(s);
            }
        }

        private async void SelectSuggestion(object sender, SelectionChangedEventArgs args)
        {
            var selection = SuggestionsListView.SelectedValue;
            HideSuggestions();
            if (selection is null)
            {
                return;
            }
            AddressTextBox.Text = selection.ToString();
        }

        private void ShowSuggestions() => SuggestionsListView.Visibility = Visibility.Visible;
        private void HideSuggestions() => SuggestionsListView.Visibility = Visibility.Hidden;

    }
}
