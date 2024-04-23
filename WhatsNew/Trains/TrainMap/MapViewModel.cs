using Esri.ArcGISRuntime.ArcGISServices;
using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Mapping.Labeling;
using Esri.ArcGISRuntime.Symbology;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TrainMap.Data;
using System.Drawing;
using System.Collections.Generic;

namespace TrainMap
{
	/// <summary>
	/// Provides map data to an application
	/// </summary>
	public class MapViewModel : INotifyPropertyChanged
	{
		public MapViewModel()
		{
			// Create the map, with a gray basemap and center the map on the center of the Netherlands.
			_map = new Map(SpatialReferences.WebMercator)
			{
				InitialViewpoint = new Viewpoint(new MapPointBuilder(5.388311, 52.155726, SpatialReferences.Wgs84).ToGeometry(), 1500000),
				Basemap = new Basemap(BasemapStyle.ArcGISLightGrayBase)
			};

			FeatureLayer railFeatureLayer = RailLogic.GetRailFeatureLayer();
			if (railFeatureLayer != null)
			{
				_map.OperationalLayers.Add(railFeatureLayer);
			}

			TrainDataSource customSource = new (TimeSpan.FromSeconds(4));

			// Create the dynamic entity layer using the custom data source.
			DynamicEntityLayer dynamicEntityLayer = new DynamicEntityLayer(customSource);

			// Set up the track display properties.
			SetupTrackDisplayProperties(dynamicEntityLayer);

			// Set up the dynamic entity labeling.
			SetupLabeling(dynamicEntityLayer);

			// Add the dynamic entity layer to the map.
			_map.OperationalLayers.Add(dynamicEntityLayer);
		}

		private Map _map;

		/// <summary>
		/// Gets or sets the map
		/// </summary>
		public Map Map
		{
			get => _map;
			set { _map = value; OnPropertyChanged(); }
		}

		/// <summary>
		/// Raises the <see cref="MapViewModel.PropertyChanged" /> event
		/// </summary>
		/// <param name="propertyName">The name of the property that has changed</param>
		protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		private static void SetupLabeling(DynamicEntityLayer layer)
		{
			// Define the label expression to be used, in this case we will use the "treinNummer" for each of the dynamic entities.
			ArcadeLabelExpression arcadeLabelExpression = new (new ArcadeExpression("$feature.treinNummer + ' - ' + Round($feature.snelheid, 0) + 'km/u' "));

			// Set the text symbol color and size for the labels.
			TextSymbol labelSymbol = new ()
			{
				Color = Color.Black,
				Size = 14d
			};

			// Set the label position and visibility.
			LabelDefinition labelDef = new (arcadeLabelExpression, labelSymbol)
			{
				Placement = LabelingPlacement.PointAboveCenter,
				LabelOverlapStrategy = LabelOverlapStrategy.Exclude,
				MinScale = 1200000
			};

			// Add the label definition to the dynamic entity layer and enable labels.
			layer.LabelDefinitions.Add(labelDef);
			layer.LabelsEnabled = true;

			// Create a unique value renderer
			layer.Renderer = new UniqueValueRenderer(["type"], new List<UniqueValue>() {
			{
				new UniqueValue("SPR", "SPR", new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, Settings.BlueColor, 16), "SPR")
			},
			{
				new UniqueValue("IC", "IC", new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, Settings.YellowColor, 16), "IC")
			},
			{
				new UniqueValue("ARR", "ARR", new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, Settings.RedColor, 16), "ARR")
			}}, "none", new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, Color.Gray, 16));
		}

		private static void SetupTrackDisplayProperties(DynamicEntityLayer layer)
		{
			// Set up the track display properties, these properties will be used to configure the appearance of the track line and previous observations.
			layer.TrackDisplayProperties.ShowPreviousObservations = true;
			layer.TrackDisplayProperties.ShowTrackLine = true;
			layer.TrackDisplayProperties.MaximumObservations = 8;
		}
	}
}
