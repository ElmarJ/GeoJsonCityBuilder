using System.Collections.Generic;
using System.Linq;
using GeoJsonCityBuilder.Data;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace GeoJsonCityBuilder
{
    [ExecuteAlways]
    public class BlocksFromGeoJson : GameObjectsFromGeoJson
    {
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