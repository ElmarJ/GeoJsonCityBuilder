using System;
using System.Collections.Generic;

namespace GeoJsonCityBuilder.Data.GeoJSON {
    [Serializable]
    public abstract class Geometry
    {
        public string Type { get; set; }
    }
}