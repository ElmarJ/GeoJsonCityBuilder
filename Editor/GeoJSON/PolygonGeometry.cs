using System;
using System.Collections.Generic;

namespace GeoJsonCityBuilder.Data.GeoJSON {
    [Serializable]
    public class PolygonGeometry: Geometry
    {
        public List<List<Coordinate>> Coordinates { get; } = new List<List<Coordinate>>();
    }
}