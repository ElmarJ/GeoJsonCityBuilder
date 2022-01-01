using System;
using System.Collections.Generic;

namespace GeoJsonCityBuilder.Data.GeoJSON
{
    [Serializable]
    public class PointGeometry : Geometry
    {
        public Coordinate Coordinate { get; set; }
    }
}