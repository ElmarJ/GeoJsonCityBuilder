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
    public class PrefabsFromGeoJson : MonoBehaviour
    {
        public TextAsset geoJsonFile;
        public WorldPositionAnchor worldPosition;

        public string featureTypeFilter;

        public GameObject prefab;
    }
}