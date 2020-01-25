using System;
using GeoJsonCityBuilder.Data.GeoJSON;

namespace GeoJsonCityBuilder.Data.Neighborhood
{
    [Serializable]
    public class Building
    {
        public Coordinate[] GroundPolygon;
        public float Height;

        public static Building FromGeoJSON(Feature feature) => throw new NotImplementedException();
    }
}