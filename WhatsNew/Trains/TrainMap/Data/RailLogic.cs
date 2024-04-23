using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using System;
using System.IO;

namespace TrainMap.Data
{
	public static class RailLogic
	{
		public static FeatureLayer GetRailFeatureLayer()
		{
			FeatureLayer? SpoortakFeatureLayer = null;

			// Create the off-line layer package path
			if (File.Exists(Settings.RailLayerPath))
			{
				// Open the off-line layer package. 
				Geodatabase railDatabase = Geodatabase.OpenAsync(Settings.RailLayerPath).Result;
				if (railDatabase != null)
				{
					GeodatabaseFeatureTable? railFeatureTabel = railDatabase.GetGeodatabaseFeatureTable("Functionelespoortak");
					if (railFeatureTabel != null)
					{
						SpoortakFeatureLayer = new(railFeatureTabel)
						{
							RenderingMode = FeatureRenderingMode.Dynamic
						};
					}
				}
			}

			// In case SpoortakFeatureLayer is null, do a fall back to AGOL rail service.
			SpoortakFeatureLayer ??= new FeatureLayer(new Uri(Settings.RailLayerUrl));

			// Load the service.
			SpoortakFeatureLayer.LoadAsync().Wait();

			// Create a simple NS Yellow railway renderer.
			SpoortakFeatureLayer.Renderer = new SimpleRenderer(new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, Settings.RailColor, 4));

			// Return the rail layer.
			return SpoortakFeatureLayer;
		}
	}
}
