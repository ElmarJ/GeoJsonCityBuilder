using System.Collections.Generic;
using System.Linq;
using GeoJsonCityBuilder.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace GeoJsonCityBuilder
{
    [ExecuteAlways]
    public class GameObjectsFromGeoJson : MonoBehaviour
    {
        public TextAsset geoJsonFile;
        public WorldPositionAnchor worldPosition;
        public string featureTypeFilter;
        public GameObject prefab;
        public string timeStartYearField;
        public string timeEndYearField;
        public List<GeoJsonBinding> geoJsonBindings = new();
    }
}