using CreateWebMaps.Logic.Settings;
using EsriNL.Net.API;

namespace CreateWebMaps.Logic
{
    public static class ArcGISLogic
    {
        public static IArcGISClient GetClient()
        {
            string? arcgisUserName = Environment.GetEnvironmentVariable(Constants.ArcGISUserName);
            string? arcgisPassword = Environment.GetEnvironmentVariable(Constants.ArcGISPassword);
            if (arcgisPassword == null || arcgisUserName == null)
            {
                throw new Exception("ArcGIS Username and or password not provided in the settings.");
            }
            ArcGISPortal arcgis = new(arcgisUserName, arcgisPassword);
            return arcgis;
        }
    }
}
