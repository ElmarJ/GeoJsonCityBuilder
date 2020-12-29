using System;
using System.Collections.Generic;

namespace GeoJsonCityBuilder.Data.GeoJSON {
    [Serializable]
    public class Properties
    {
        public string Type { get; set; }
        public float? Height { get; set; }

        public long? ExistencePeriodStartYear { get; set; }
        public long? ExistencePeriodEndYear { get; set; }
    }
}