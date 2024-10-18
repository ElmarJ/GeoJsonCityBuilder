using UnityEngine;
using UnityEngine.ProBuilder;

namespace GeoJsonCityBuilder.Components
{
    [ExecuteAlways]
    public class BordersFromGeoJson : GeneratorComponentBase
    {
        public float height;
        public float outerExtension;
        public float innerExtension;
        public Material material;
        public AutoUnwrapSettings sideUvUnwrapSettings = new();
    }
}