using System;
using System.Collections.Generic;

namespace GeoJsonCityBuilder.Data.GeoJSON {
    public class FeatureCollection
    {
        public string Type { get; set; }
        public List<Feature> Features { get; } = new List<Feature>();

    }
}