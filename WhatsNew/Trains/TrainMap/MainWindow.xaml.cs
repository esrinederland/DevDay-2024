using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.RealTime;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI.Controls;
using Esri.ArcGISRuntime.UI;
using System;
using System.Linq;
using System.Text;
using System.Windows;

namespace TrainMap
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private async void GeoViewTapped(object sender, GeoViewInputEventArgs e)
		{
			e.Handled = true;
			try
			{
				TrainMapView.DismissCallout();

				// If no dynamic entity layer is present in the map, return.
				DynamicEntityLayer? layer = TrainMapView.Map?.OperationalLayers.OfType<DynamicEntityLayer>().FirstOrDefault();
				if (layer is null) 
				{
					return;
				}

				// Identify the tapped observation.
				IdentifyLayerResult results = await TrainMapView.IdentifyLayerAsync(layer, e.Position, 2d, false);
				if (results.GeoElements.Count == 0 || results.GeoElements[0] is not DynamicEntityObservation observation)
				{
					return;
				}

				// Get the dynamic entity from the observation.
				DynamicEntity? dynamicEntity = observation.GetDynamicEntity();
				if (dynamicEntity is null)
				{
					return;
				}

				// Build a string for observation attributes.
				StringBuilder stringBuilder = new ();
				foreach (string attribute in dynamicEntity.Attributes.Keys)
				{
					// Get the value.
					string? value = dynamicEntity.Attributes[attribute]?.ToString();

					// Account for when an attribute has an empty value.
					if (!string.IsNullOrEmpty(value))
					{
						stringBuilder.AppendLine(attribute + ": " + value);
					}
				}

				// The standard callout takes care of moving when the dynamic entity changes.
				CalloutDefinition calloutDef = new (stringBuilder.ToString().TrimEnd());
				if (layer.Renderer?.GetSymbol(dynamicEntity) is Symbol symbol)
				{
					await calloutDef.SetIconFromSymbolAsync(symbol);
				}

				// Show the callout for the tapped dynamic entity.
				TrainMapView.ShowCalloutForGeoElement(dynamicEntity, e.Position, calloutDef);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error identifying dynamic entity.");
			}
		}
	}
}
