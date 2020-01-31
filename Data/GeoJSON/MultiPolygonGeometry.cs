using System;
using System.Collections.Generic;

namespace GeoJsonCityBuilder.Data.GeoJSON {
    [Serializable]
    public class MultiPolygonGeometry: Geometry
    {
        public List<PolygonGeometry> Geometries { get; } = new List<PolygonGeometry>();
    }
}