using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.Geocoding;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SearchForAddresses
{

    internal class MapViewModel : INotifyPropertyChanged
    {

        public MapViewModel()
        {
            SetupMap();
        }
        public ObservableCollection<string> suggestionsList { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Map? _map;
        public Map? Map
        {
            get { return _map; }
            set
            {
                _map = value;
                OnPropertyChanged();
            }
        }

        private GraphicsOverlayCollection? _graphicsOverlayCollection;
        public GraphicsOverlayCollection? GraphicsOverlays
        {
            get { return _graphicsOverlayCollection; }
            set
            {
                _graphicsOverlayCollection = value;
                OnPropertyChanged();
            }
        }

        private void SetupMap()
        {

            // Create a new map with a 'topographic vector' basemap.
            Map = new Map(BasemapStyle.ArcGISTopographic);

            // Set the view model "GraphicsOverlays" property.
            GraphicsOverlay overlay = new GraphicsOverlay();
            GraphicsOverlayCollection overlayCollection = new GraphicsOverlayCollection
            {
                overlay
            };
            this.GraphicsOverlays = overlayCollection;
        }


        // Create a locator task.
        private LocatorTask locatorTask = new LocatorTask(new Uri("https://geocode-api.arcgis.com/arcgis/rest/services/World/GeocodeServer"));
        public async Task<MapPoint?> SearchAddress(string address, SpatialReference spatialReference)
        {
            MapPoint? addressLocation = null;

            try
            {
                // Get the first graphics overlay from the GraphicsOverlays and remove any previous result graphics.
                GraphicsOverlay? graphicsOverlay = this.GraphicsOverlays?.FirstOrDefault();
                graphicsOverlay?.Graphics.Clear();

                // Define geocode parameters: limit the results to one and get all attributes.
                GeocodeParameters geocodeParameters = new GeocodeParameters();
                geocodeParameters.ResultAttributeNames.Add("*");
                geocodeParameters.MaxResults = 1;
                geocodeParameters.OutputSpatialReference = spatialReference;

                // Geocode the address string and get the first (only) result.
                IReadOnlyList<GeocodeResult> results = await locatorTask.GeocodeAsync(address, geocodeParameters);
                GeocodeResult? geocodeResult = results[0];
                if (geocodeResult == null) { throw new Exception("No matches found."); }

                // Create a graphic to display the result location.
                Uri imageUri = new Uri("https://i.postimg.cc/6pLPS24S/pin.png");
                PictureMarkerSymbol picSymbol = new PictureMarkerSymbol(imageUri);
                Graphic markerGraphic = new Graphic(geocodeResult.DisplayLocation, geocodeResult.Attributes, picSymbol);

                // Create a graphic to display the result address label.
                TextSymbol textSymbol = new TextSymbol(geocodeResult.Label, Color.Red, 16, HorizontalAlignment.Center, VerticalAlignment.Bottom);
                Graphic textGraphic = new Graphic(geocodeResult.DisplayLocation, textSymbol);

                // Add the location and label graphics to the graphics overlay.
                graphicsOverlay?.Graphics.Add(markerGraphic);
                graphicsOverlay?.Graphics.Add(textGraphic);

                // Set the address location to return from the function.
                addressLocation = geocodeResult?.DisplayLocation;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Couldn't find address: " + ex.Message);
            }

            // Return the location of the geocode result.
            return addressLocation;
        }

        public async Task<ObservableCollection<string>> SuggestAddress(string partialAddress)
        {
            suggestionsList = new ObservableCollection<string>();
            if (partialAddress == "")
            {
                return suggestionsList;
            }

            var suggestions = await locatorTask.SuggestAsync(partialAddress);
            if (suggestions?.Any() ?? false)
            {
                foreach (var suggestion in suggestions)
                {
                    suggestionsList.Add(suggestion.Label);
                }
            }
            return suggestionsList;
        }
    }
}
