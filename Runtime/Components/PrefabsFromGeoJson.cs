using UnityEngine;

namespace GeoJsonCityBuilder.Components
{
    [ExecuteAlways]
    public class PrefabsFromGeoJson : GeneratorComponentBase
    {
        public GameObject prefab;
        public WorldPositionAnchor worldPosition;
    }
}