using AiCore;
using ArcGIS.Core.CIM;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Geocoding;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArcGISProAi
{
    internal class ShowCafe : MapTool
    {
        public ShowCafe()
        {
            IsSketchTool = false;
            UseSnapping = false;
        }

        protected override void OnToolMouseDown(MapViewMouseButtonEventArgs args)
        {
            //On mouse down check if the mouse button pressed is the left mouse button. If it is handle the event.
            if (args.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                args.Handled = true;
            }
        }

        /// <summary>
        /// Called when the OnToolMouseDown event is handled. Allows the opportunity to perform asynchronous operations corresponding to the event.
        /// </summary>
        protected override async Task HandleMouseDownAsync(MapViewMouseButtonEventArgs args)
        {
            string path = @"D:\DevDay\2024\ShowCafes.json";
            await QueuedTask.Run(() =>
            {
                // Convert the screen point to map coordinates
                string question = $"Kun je me van eerder gevonden 5 cafe's de naam en het adres in JSON gegeven. " +
                                   "De Naam is daarbij geschreven als Naam en het adres als Adres, alles is samengevoegd onder de naam cafes.";

                var cityInfo = Core.GetAiResponse(question);
                var json = JsonDocument.Parse(cityInfo);                
                //Core.WriteResultToFile(json, path);

                Cafes foundCafes = json.Deserialize<Cafes>();                

                return CreateGraphicOverlay(foundCafes);
            });
        }

        private static async Task CreateGraphicOverlay(Cafes foundCafes)
        {
            // get the current mapview and point
            var mapView = MapView.Active;
            if (mapView == null)
            {
                return;
            }

            await MapView.Active.LocatorManager.AddLocatorAsync("https://geocoder.arcgisonline.nl/arcgis/rest/services/Geocoder_NL_WGS/GeocodeServer");

            foreach (var cafe in foundCafes.cafes)
            {
                
                var geocodes = await MapView.Active.LocatorManager.GeocodeAsync(cafe.Adres, true, false);

                // add point graphic to the overlay at the center of the mapView
                await QueuedTask.Run(() =>
                {
                    GeocodeResult geocode = geocodes[0];
                    {
                        CIMTextGraphic textGraphic = Core.GetTextGraphic(cafe.Naam, geocode.Extent.Center, ColorFactory.Instance.RedRGB);
                        CIMPointGraphic pointGraphic = Core.GetPointGraphic(cafe.Naam, geocode.Extent.Center, ColorFactory.Instance.BlueRGB);
                        
                        _ = mapView.AddOverlay(textGraphic);
                        _ = mapView.AddOverlay(pointGraphic);
                    }
                });                
            }
        }        
    }
}
