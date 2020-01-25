using System;
using System.Collections.Generic;

namespace Data.GeoJSON {
    [Serializable]
    public class PointGeometry: Geometry
    {
        public Coordinate Coordinate { get; set; }
    }
}