using ArcGIS.Core.Geometry;
using ArcGIS.Core.Internal.Geometry;

namespace ArcGISProAi
{
    public class Cafe
    {
        public string Naam { get; set; }

        public string Beschrijving { get; set; }
        
        public string Adres { get; set; }

        public double[] Locatie { get; set; }

        public double Latitude => Locatie[0];
        public double Longitude => Locatie[1];

        public MapPoint GetPoint()
        {            
            return MapPointBuilder.CreateMapPoint(Longitude, Latitude, SpatialReferences.WGS84);
        }
    }
}
