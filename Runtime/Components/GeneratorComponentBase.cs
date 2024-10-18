using UnityEngine;
using UnityEngine.ProBuilder;

namespace GeoJsonCityBuilder.Components
{
    public abstract class GeneratorComponentBase : MonoBehaviour
    {
        public WorldPositionAnchor worldPositionAnchor;
        public TextAsset geoJsonFile;
        public string excludeProperty;
        public string timeStartYearField;
        public string timeEndYearField;

    }
}