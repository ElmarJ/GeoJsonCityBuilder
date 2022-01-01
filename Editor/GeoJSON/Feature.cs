using System;

namespace GeoJsonCityBuilder.Data.GeoJSON
{
    public class Feature
    {
        public string Type { get; set; }
        public Properties Properties { get; } = new Properties();

        public Geometry Geometry { get; set; }
    }
}