using System.Collections.Generic;
using System.Linq;
using GeoJsonCityBuilder.Data;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace GeoJsonCityBuilder
{
    [ExecuteAlways]
    public class BordersFromGeoJson : GameObjectsFromGeoJson
    {
        public float height;
        public float outerExtension;
        public float innerExtension;
        public Material material;
        public AutoUnwrapSettings sideUvUnwrapSettings = new AutoUnwrapSettings();
    }
}