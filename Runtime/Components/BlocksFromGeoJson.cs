using System.Collections.Generic;
using System.Linq;
using GeoJsonCityBuilder.Data;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace GeoJsonCityBuilder
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
        public AutoUnwrapSettings sideUvUnwrapSettings = new AutoUnwrapSettings();
        public bool pointedRoofTops = false;
        public bool raiseFacades = false;
    }
}