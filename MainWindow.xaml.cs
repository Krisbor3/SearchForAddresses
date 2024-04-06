using Esri.ArcGISRuntime.Geometry;
using SearchForAddresses.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SearchForAddresses
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            MainMapView.LocationDisplay.IsEnabled = true;
            MainMapView.LocationDisplay.AutoPanMode = Esri.ArcGISRuntime.UI.LocationDisplayAutoPanMode.Recenter;

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

            //Add to history
            HistoryService.SearchedAddresses.Push(AddressTextBox.Text);
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

        private void Image_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //show history
            if (!HistoryService.SearchedAddresses.Any())
            {
                MessageBox.Show("No history found");
                return;
            }
            SuggestionsListView.Items.Clear();
            foreach (var sa in HistoryService.SearchedAddresses)
            {
                SuggestionsListView.Items.Add(sa);
            }
            ShowSuggestions();
        }
    }
}
