using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace GeoJsonCityBuilder.Components
{
    [ExecuteAlways]
    public class BlocksFromGeoJson : MonoBehaviour
    {
        public WorldPositionAnchor worldPositionAnchor;
        public TextAsset geoJsonFile;
        public GameObject basePrefab;
        public string featureTypeFilter;
        public float heightMin;
        public float heightMax;
        public Material topMaterial;
        public List<Material> sideMaterials;
        public Material bottomMaterial;
        public AutoUnwrapSettings sideUvUnwrapSettings = new();
        public bool pointedRoofTops = false;
        public bool raiseFacades = false;
        public string timeStartYearField;
        public string timeEndYearField;
    }
}