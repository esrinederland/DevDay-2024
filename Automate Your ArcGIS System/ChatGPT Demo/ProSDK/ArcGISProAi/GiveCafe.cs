using AiCore;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = ArcGIS.Desktop.Framework.Dialogs.MessageBox;

namespace ArcGISProAi
{
    internal class GiveCafe : MapTool
    {
        public GiveCafe()
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
            await QueuedTask.Run(() =>
            {
                // Convert the screen point to map coordinates
                var mapPoint = MapView.Active.ScreenToMap(args.ClientPoint);
                var point = GeometryEngine.Instance.Project(mapPoint, SpatialReferences.WGS84);
                var cityInfo = Core.GetAiResponse($"Geef de namen van 5 bekende cafes die liggen in deze plaats {(point as MapPoint).Y}, {(point as MapPoint).X}");

                MessageBox.Show(cityInfo, "Welke bekende cafe's?", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }
    }
}
