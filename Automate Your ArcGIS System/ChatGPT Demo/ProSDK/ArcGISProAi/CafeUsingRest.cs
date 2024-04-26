using AiCore;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using EsriNL.Net.API;
using EsriNL.Net.API.Data;
using EsriNL.Net.API.Geometry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArcGISPortal = ArcGIS.Desktop.Core.ArcGISPortal;
using FeatureLayer = EsriNL.Net.API.FeatureLayer;
using SpatialReference = EsriNL.Net.API.Geometry.SpatialReference;

namespace ArcGISProAi
{
    internal class CafeUsingRest : MapTool
    {
        public IArcGISClient ArcGISOnlineClient { get; set; }

        public CafeUsingRest()
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
            string path = @"D:\DevDay\2024\ShowCafesRest.json";
            string url = "https://services.arcgis.com/emS4w7iyWEQiulAb/arcgis/rest/services/RestCafes/FeatureServer/6";
            await QueuedTask.Run(() =>
            {                
                // Convert the screen point to map coordinates
                //var mapPoint = MapView.Active.ScreenToMap(args.ClientPoint);
                //var point = GeometryEngine.Instance.Project(mapPoint, SpatialReferences.WGS84);
                //string question = $"Geef in JSON formaat de naam en een beschrijving van 5 andere bekende cafes inclusief coordinaten die liggen in deze plaats {(point as MapPoint).Y}, {(point as MapPoint).X} waarbij de naam is geschreven als Naam, de beschrijving als Beschrijving en de coordinaten in het coordinaten systeem WGS84 als array met de naam Locatie, alles is samengevoegd onder de naam cafes";
                //var cityInfo = Core.GetAiResponse(question);
                //var json = JsonDocument.Parse(cityInfo);                

                //Core.WriteResultToFile(json, path);
                //Cafes foundCafes = json.Deserialize<Cafes>();
                Cafes foundCafes = System.Text.Json.JsonSerializer.Deserialize<Cafes>(Core.OpenJsonStreamResonse(path));

                ArcGISPortal arcGisportal = ArcGISPortalManager.Current.GetActivePortal();
                string token = arcGisportal.GetToken();
                ArcGISOnlineClient = new EsriNL.Net.API.ArcGISPortal(token);
                FeatureLayer featureLayer = new(ArcGISOnlineClient, new Uri(url));
               
                List<Feature> features = new();
                foreach (var cafe in foundCafes.cafes)
                {
                    features.Add(new Feature()
                    {
                        Attributes = new Dictionary<string, object>()
                        {
                            { "Naam", cafe.Naam},
                            { "Beschrijving", cafe.Beschrijving}
                        },
                        Geometry = new Point()
                        {
                            X = cafe.Longitude,
                            Y = cafe.Latitude,
                            SpatialReference = new SpatialReference(4326)
                        }
                    });
                }
                featureLayer.AddFeatures(features);                
            });
        }
    }
}
