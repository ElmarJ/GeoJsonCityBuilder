using System.Collections.Generic;

using System.Linq;
using GeoJsonCityBuilder.Data.GeoJSON;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace GeoJsonCityBuilder
{

    [ExecuteAlways]
    [RequireComponent(typeof(PositionOnWorldCoordinates))]
    public class BlocksFromGeoJson : MonoBehaviour
    {
        public TextAsset geoJsonFile;
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