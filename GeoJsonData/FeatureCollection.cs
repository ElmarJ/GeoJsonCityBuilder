using System;
using System.Collections.ObjectModel;

namespace Data.GeoJSON {
    public class FeatureCollection
    {
        public string Type { get; set; }
        public Collection<Feature> Features { get; } = new Collection<Feature>();

    }
}