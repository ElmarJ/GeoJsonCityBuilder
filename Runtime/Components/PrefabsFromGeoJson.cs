﻿using UnityEngine;

namespace GeoJsonCityBuilder.Components
{
    [ExecuteAlways]
    public class PrefabsFromGeoJson : MonoBehaviour
    {
        public TextAsset geoJsonFile;
        public WorldPositionAnchor worldPosition;
        public string featureTypeFilter;
        public GameObject prefab;
        public string timeStartYearField;
        public string timeEndYearField;

    }
}