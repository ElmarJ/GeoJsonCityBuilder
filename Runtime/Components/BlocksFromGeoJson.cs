using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace GeoJsonCityBuilder.Components
{
    [ExecuteAlways]
    public class BlocksFromGeoJson : GeneratorComponentBase
    {
        public GameObject basePrefab;
        public string heightProperty;
        public float heightMin;
        public float heightMax;
        public Material topMaterial;
        public List<Material> sideMaterials;
        public Material bottomMaterial;
        public AutoUnwrapSettings sideUvUnwrapSettings = new();
        public bool pointedRoofTops = false;
        public bool raiseFacades = false;
    }
}