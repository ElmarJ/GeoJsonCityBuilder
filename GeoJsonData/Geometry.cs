using System;
using System.Collections.Generic;

namespace Data.GeoJSON {
    [Serializable]
    public abstract class Geometry
    {
        public string Type { get; set; }
    }
}