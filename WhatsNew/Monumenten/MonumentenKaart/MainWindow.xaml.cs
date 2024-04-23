using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Mapping.Popups;
using Esri.ArcGISRuntime.Reduction;
using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MonumentenKaart
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private FeatureLayer? MonumentenLayer { get; set; }

		/// <summary>
		/// Default constructor, loading the map and initializing the MapView. 
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();
				
			// Load the map. 
			Map monumentenMap = new(new Uri(@"https://esrinederland.maps.arcgis.com/apps/mapviewer/index.html?webmap=13106b9983b24f63a8dfd223b2b05d65"));

			// Load the map
			monumentenMap.LoadAsync().Wait();

			// Add the map to the MapView.
			mvMonumenten.Map = monumentenMap;

			// Store the "monumenten" FeatureLayer. 
			MonumentenLayer = mvMonumenten.Map?.OperationalLayers.OfType<FeatureLayer>().First();

			// In case you need to show more field values in your aggregation pop-up, add them here. 
			if (MonumentenLayer?.FeatureReduction is AggregationFeatureReduction featureReduction)
			{
				featureReduction.AggregateFields.Add(new AggregateField("juridische_status", "juridische_status", AggregateStatisticType.Mode));
				featureReduction.AggregateFields.Add(new AggregateField("subcategorie", "subcategorie", AggregateStatisticType.Mode));
				featureReduction.AggregateFields.Add(new AggregateField("rijksmonumenturl", "rijksmonumenturl", AggregateStatisticType.Mode));
			}
		}

		/// <summary>
		/// Handle the MapView click event and show the (aggregate) pop-up.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void mvMonumenten_GeoViewTapped(object sender, GeoViewInputEventArgs e)
		{
			if (MonumentenLayer == null)
			{ 
				return;
			}

			// Clear any previously selected features.
			MonumentenLayer.ClearSelection();

			// Identify the tapped observation.
			var results = await mvMonumenten.IdentifyLayerAsync(MonumentenLayer, e.Position, 3, false);

			// Return if no pop-ups are found.
			if (results.GeoElements.Count == 0 || results.Popups.Count == 0)
			{
				return;
			}

			if (results.Popups.FirstOrDefault() is Popup popup)
			{
				//Set the pop-up on the pop-up viewer.;
				PopupViewer.Popup = popup;
			}

			// If the tapped observation is an AggregateGeoElement then select it.
			if (results.GeoElements.FirstOrDefault() is AggregateGeoElement aggregateGeoElement)
			{
				// Select the AggregateGeoElement.
				aggregateGeoElement.IsSelected = true;

				// Get the contained GeoElements.
				IReadOnlyList<GeoElement> geoElements = await aggregateGeoElement.GetGeoElementsAsync();

				// Set the GeoElements as an items source and set the visibility.
				GeoElementsGrid.ItemsSource = geoElements;
				GeoElementsPanel.Visibility = Visibility.Visible;
			}
			else if (results.GeoElements.FirstOrDefault() is ArcGISFeature feature)
			{
				// If the tapped observation is not an AggregateGeoElement select the feature.
				MonumentenLayer.SelectFeature(feature);
			}

			// Make the pop-up visible.
			PopupBackground.Visibility = Visibility.Visible;
		}

		/// <summary>
		/// Hide and nullify the opened pop-up when user left clicks.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PopupBackground_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			PopupBackground.Visibility = Visibility.Collapsed;
			GeoElementsPanel.Visibility = Visibility.Collapsed;
			PopupViewer.Popup = null;
			GeoElementsGrid.ItemsSource = null;
		}
	}
}
