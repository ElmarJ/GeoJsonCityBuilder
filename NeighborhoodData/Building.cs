using System;
using Data.GeoJSON;

namespace Data.Neighborhood
{
    [Serializable]
    public class Building
    {
        public Coordinate[] GroundPolygon;
        public float Height;

        public static Building FromGeoJSON(Feature feature) => throw new NotImplementedException();
    }
}