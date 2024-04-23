using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Security;
using System;
using System.Windows;

namespace TrainMap
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			/* Authentication for ArcGIS location services:
             * Use of ArcGIS location services, including basemaps and geocoding, requires either:
             * 1) ArcGIS identity (formerly "named user"): An account that is a member of an organization in ArcGIS Online or ArcGIS Enterprise
             *    giving your application permission to access the content and location services authorized to an existing ArcGIS user's account.
             *    You'll get an identity by signing into the ArcGIS Portal.
             * 2) API key: A permanent token that grants your application access to ArcGIS location services.
             *    Create a new API key or access existing API keys from your ArcGIS for Developers
             *    dashboard (https://links.esri.com/arcgis-api-keys) then call .UseApiKey("[Your ArcGIS location services API Key]")
             *    in the initialize call below. */

			/* Licensing:
             * Production deployment of applications built with the ArcGIS Maps SDK requires you to license ArcGIS functionality.
             * For more information see https://links.esri.com/arcgis-runtime-license-and-deploy.
             * You can set the license string by calling .UseLicense(licenseString) in the initialize call below 
             * or retrieve a license dynamically after signing into a portal:
             * ArcGISRuntimeEnvironment.SetLicense(await myArcGISPortal.GetLicenseInfoAsync()); */
			try
			{
				// Initialize the ArcGIS Maps SDK runtime before any components are created.
				ArcGISRuntimeEnvironment.Initialize(config => config
				  .UseLicense("runtimelite,1000,rud2435118586,none,E9PJD4SZ8LYBLMZ59168")
				  .UseApiKey("AAPKb9ee517e3c2d4444ae585fd2e401a0c47G1HpSbZCpP8katOSuiHD0TcpAPtUh06KNAhB_Q8FzSQPR4SDmaXgOPeyTcLC2TX")
				  .ConfigureAuthentication(auth => auth.UseDefaultChallengeHandler())
				);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "ArcGIS Maps SDK runtime initialization failed.");

				// Exit application
				Shutdown();
			}
		}
	}
}
