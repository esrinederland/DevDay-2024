using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArcGISProAi
{
    internal class MoreInfo : MapTool
    {
        public MoreInfo()
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
            Element selectedGraphic = await GetSelectedGraphic(args);

            // Check if any graphics are selected
            if (selectedGraphic != null)
            {
                var popupContent = new PopupContent(selectedGraphic.CustomProperties[0].Value, selectedGraphic.CustomProperties[0].Key);
                MapView.Active.ShowCustomPopup(new List<PopupContent>() { popupContent });
            }
        }

        private static async Task<Element> GetSelectedGraphic(MapViewMouseButtonEventArgs args)
        {
            return await QueuedTask.Run(() =>
            {
                if (MapView.Active == null)
                    return null;

                // Get the graphics overlay
                var graphicsOverlay = MapView.Active.Map.GetLayersAsFlattenedList().OfType<GraphicsLayer>().FirstOrDefault();
                if (graphicsOverlay == null)
                {
                    MessageBox.Show("No graphics overlay found", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    return null;
                }

                // Convert the screen point to map coordinates
                var mapPoint = MapView.Active.ClientToMap(args.ClientPoint);
                
                // Get the graphics at the mouse position
                return graphicsOverlay.GetElements().FirstOrDefault(graphic =>
                {
                    return (graphic.CustomProperties.Any()) &&
                         // Check if the mouse is within the extent of the graphic
                         (GeometryEngine.Instance.Intersects(GeometryEngine.Instance.Buffer(mapPoint, 0.0002), graphic.GetGeometry()));
                });
            });
        }
    }
}
