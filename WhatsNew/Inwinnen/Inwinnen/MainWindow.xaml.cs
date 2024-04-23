using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Mapping.FeatureForms;
using Esri.ArcGISRuntime.UI.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Inwinnen
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private GeometryEditor CurrentGeometryEditor { get; set; }

		private ServiceFeatureTable InspectionsFeatureTable { get; set; }

		/// <summary>
		/// Default constructor
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();

			// Load the forms map and add it to the MapView
			Map formsMaps = new(new Uri(@"https://esrinederland.maps.arcgis.com/apps/mapviewer/index.html?webmap=64b6091115ed4b19a4221d36ad0b4db7"));
			formsMaps.LoadAsync().Wait();
			mvFormMaps.Map = formsMaps;

			// Create a GeometryEditor class for drawing.
			CurrentGeometryEditor = new GeometryEditor();
			mvFormMaps.GeometryEditor = CurrentGeometryEditor;

			// Load all layer for the snap settings to load correctly 
			foreach (Layer layer in mvFormMaps.Map.OperationalLayers)
			{
				layer.LoadAsync().Wait(5000); 
			}

			// Load the SnapSettings (layers where the editor can snap to)
			CurrentGeometryEditor.SnapSettings.SyncSourceSettings();
			foreach (var source in CurrentGeometryEditor.SnapSettings.SourceSettings)
			{
				// In this case, enable all layers.
				source.IsEnabled = true;
			}

			// Set snapping on and with a range of 30px
			CurrentGeometryEditor.SnapSettings.IsEnabled = true;
			CurrentGeometryEditor.SnapSettings.Tolerance = 30;

			// Get the inspections Feature Service Table for storing the result later.
			if (mvFormMaps.Map.OperationalLayers.FirstOrDefault(item => item.Name == "InspectiesConnect2024") is FeatureLayer featureLayer &&
				featureLayer.FeatureTable is ServiceFeatureTable serviceFeatureTable)
			{
				InspectionsFeatureTable = serviceFeatureTable;
			}
			else
			{
				InspectionsFeatureTable = new ServiceFeatureTable();
			}
		}

		private void AddPoint_Click(object sender, RoutedEventArgs e)
		{
			if (!CurrentGeometryEditor.IsStarted)
			{
				CurrentGeometryEditor.Start(GeometryType.Point);
			}
		}

		/// <summary>
		/// Finish the drawing, get the feature and show the form.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void Ok_Click(object sender, RoutedEventArgs e)
		{
			// Check if the user created a geometry
			if (!CurrentGeometryEditor.IsStarted)
			{
				MessageBox.Show("Start met het tekenen van een geometry");
				return;
			}

			// Check if you can add a feature to the layer
			if (!InspectionsFeatureTable.CanAdd())
			{
				MessageBox.Show("Inspectielaag is niet geschikt voor het toevoegen van items.");
				return;
			}

			// Get the drawn Geometry by stopping the editor
			Geometry? geometry = CurrentGeometryEditor.Stop();

			// Get the template for creating a ArcGISFeature
			FeatureTemplate? template = InspectionsFeatureTable.FeatureTemplates.FirstOrDefault();
			if (template == null)
			{
				MessageBox.Show("Kan geen template vinden.");
				return;
			}

			// Create the ArcGISFeature
			ArcGISFeature feature = InspectionsFeatureTable.CreateFeature(template, geometry);
			await InspectionsFeatureTable.AddFeatureAsync(feature);

			// Check for a feature form definition
			if (InspectionsFeatureTable.Layer is FeatureLayer inspectionsFeatureLayer && inspectionsFeatureLayer.FeatureFormDefinition != null)
			{
				// Create the FeatureForm, based on the FromDefinition and the ArcGISFeature
				FeatureForm featureForm = new FeatureForm(feature, inspectionsFeatureLayer.FeatureFormDefinition);

				// Set the just found form on the FeatureFormView, and present the view
				ffvMain.FeatureForm = featureForm;

				// Show the container as well
				spMainForm.Visibility = Visibility.Visible;
			}
			else
			{
				MessageBox.Show("Kan geen feature form vinden.");
			}
		}

		/// <summary>
		///	Cancel the current form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			ffvMain.FeatureForm?.DiscardEdits();
			ClearForm();
		}

		/// <summary>
		/// Save the geometry and form data to ArcGIS. 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void btnSave_Click(object sender, RoutedEventArgs e)
		{
			// Save the form data to the feature.
			ArcGISFeature? feature = ffvMain.FeatureForm?.Feature;
			if (feature == null)
			{
				MessageBox.Show("Geen feature beschikbaar om op te slaan.");
				return;
			}

			// Check if we are allowed to update the feature.
			if (InspectionsFeatureTable.CanUpdate(feature))
			{
				// Update the feature with the form data
				await InspectionsFeatureTable.UpdateFeatureAsync(feature);

				IReadOnlyList<EditResult> result = await InspectionsFeatureTable.ApplyEditsAsync();
				if (result != null && !result.Any(item => item.CompletedWithErrors))
				{
					ClearForm();
				}
				else
				{ 
					MessageBox.Show($"Opslaan is mislukt {feature}");
				}
			}
		}

		/// <summary>
		/// Clear the form and disable the buttons.
		/// </summary>
		private void ClearForm()
		{
			ffvMain.FeatureForm = null;
			spMainForm.Visibility = Visibility.Collapsed;
		}
	}
}