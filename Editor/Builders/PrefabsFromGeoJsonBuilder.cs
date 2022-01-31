using System.Collections.Generic;

using System.Linq;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using GeoJsonCityBuilder.Data;
using UnityEditor;
using UnityEngine;
using GeoJSON.Net.Feature;

namespace GeoJsonCityBuilder.Editor
{
    public class PrefabsFromGeoJsonBuilder
    {
        public PrefabsFromGeoJson Component
        {
            get;
            private set;
        }

        List<Point> m_geometries;

        public PrefabsFromGeoJsonBuilder(PrefabsFromGeoJson component)
        {
            this.Component = component;
        }

        private void DeserializeGeoJson()
        {
            var geoJSON = JsonConvert.DeserializeObject<FeatureCollection>(this.Component.geoJsonFile.text);
            var filteredGeometries =
                from feature in geoJSON.Features
                where this.Component.featureTypeFilter == "" || feature.Properties["Type"].ToString() == this.Component.featureTypeFilter
                select feature.Geometry as Point;
            this.m_geometries = filteredGeometries.ToList();
        }

        public void RemoveAllChildren()
        {
            if (Application.IsPlaying(this.Component.gameObject))
            {
                foreach (Transform child in this.Component.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
            else
            {
                while (this.Component.transform.childCount > 0)
                {
                    GameObject.DestroyImmediate(this.Component.transform.GetChild(0).gameObject);
                }
            }
        }

        public void Rebuild()
        {
            this.RemoveAllChildren();
            this.DeserializeGeoJson();

            foreach (var geometry in this.m_geometries)
            {
                var go = Object.Instantiate(this.Component.prefab, this.Component.transform);

                // Todo: solve this, we shouldn't assign to positionComponent.
                var worldOrigin = this.Component.worldPosition.SceneOrigin;

                var position = geometry.Coordinates.ToCoordinate().ToLocalPosition(worldOrigin, go.transform.position.y);
                go.transform.localPosition = position;
                go.transform.Rotate(0f, Random.Range(0f, 360f), 0, Space.Self);
            }
        }
    }
}