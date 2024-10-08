using UnityEngine;
using UnityEngine.ProBuilder;

namespace GeoJsonCityBuilder.Components
{
    [ExecuteAlways]
    public class BordersFromGeoJson : MonoBehaviour
    {
        public WorldPositionAnchor worldPositionAnchor;
        public TextAsset geoJsonFile;
        public string excludeProperty;
        public float height;
        public float outerExtension;
        public float innerExtension;
        public Material material;
        public AutoUnwrapSettings sideUvUnwrapSettings = new();
        public string timeStartYearField;
        public string timeEndYearField;

    }
}