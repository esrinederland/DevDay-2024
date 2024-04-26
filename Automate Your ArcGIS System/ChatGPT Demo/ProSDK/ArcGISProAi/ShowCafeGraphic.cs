using AiCore;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArcGISProAi
{
    internal class ShowCafeGraphic : MapTool
    {
        private GraphicsLayer _graphicsLayer;


        public ShowCafeGraphic()
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

        protected override async Task HandleMouseDownAsync(MapViewMouseButtonEventArgs args)
        {
            string path = @"D:\DevDay\2024\ShowCafesGraphic.json";
            await QueuedTask.Run(() =>
            {
                // Convert the screen point to map coordinates
                // pm var mapPoint = MapView.Active.ScreenToMap(args.ClientPoint);
                // pm var point = GeometryEngine.Instance.Project(mapPoint, SpatialReferences.WGS84);
                // pm string question = $"Geef in JSON formaat de naam en een beschrijving van 5 andere bekende cafes inclusief coordinaten die liggen in deze plaats {(point as MapPoint).Y}, {(point as MapPoint).X} waarbij de naam is geschreven als Naam, de beschrijving als Beschrijving en de coordinaten als array met de naam Locatie, alles is samengevoegd onder de naam cafes";
                //var cityInfo = Core.GetAiResponse(question);
                //var json = JsonDocument.Parse(cityInfo);

                //Core.WriteResultToFile(json, path);
                //Cafes foundCafes = json.Deserialize<Cafes>();

                Cafes foundCafes = JsonSerializer.Deserialize<Cafes>(Core.OpenJsonStreamResonse(path));

                AddGraphicsLayer();
                _ = AddCafesToGraphicsLayer(foundCafes);
            });
        }

        private void AddGraphicsLayer()
        {
            var graphicsLayerName = @"ChatGPT_Results";
            var graphicsLayerCreationParams = new GraphicsLayerCreationParams { Name = graphicsLayerName, MapMemberPosition = MapMemberPosition.AutoArrange };
            _graphicsLayer = MapView.Active.Map.GetLayersAsFlattenedList().OfType<GraphicsLayer>().FirstOrDefault(e => e.Name == graphicsLayerName);

            _ = QueuedTask.Run(() =>
            {
                if (_graphicsLayer == null)
                {
                    _graphicsLayer = LayerFactory.Instance.CreateLayer<GraphicsLayer>(graphicsLayerCreationParams, MapView.Active.Map);
                    if (_graphicsLayer == null)
                    {
                        MessageBox.Show($@"{graphicsLayerName} not found", $@"Unable to create the '{graphicsLayerName}' graphics layer",
                          System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
                    }
                }
            });
        }

        public async Task AddCafesToGraphicsLayer(Cafes foundCafes)
        {
            foreach (var cafe in foundCafes.cafes)
            {
                // add point graphic to the overlay at the center of the mapView
                await QueuedTask.Run(() =>
                {
                    MapPoint projectedXY = cafe.GetPoint();

                    CIMTextGraphic textGraphic = Core.GetTextGraphic(cafe.Naam, projectedXY, ColorFactory.Instance.BlackRGB);
                    CIMPointGraphic pointGraphic = Core.GetPointGraphic(cafe.Naam, projectedXY, ColorFactory.Instance.GreenRGB);

                    ElementInfo elementInfo = new()
                    {
                        CustomProperties = new List<CIMStringMap>()
                        {
                            new()
                            {
                                Key = cafe.Naam,
                                Value = $"<H1>{cafe.Naam}</H1><br>{cafe.Beschrijving}"
                            }
                        }
                    };

                    _graphicsLayer.AddElement(pointGraphic, cafe.Naam, false, elementInfo);
                    _graphicsLayer.AddElement(textGraphic);
                });
            }
        }        
    }
}
