using System.Collections.Generic;

using System.Linq;
using GeoJsonCityBuilder.Data.GeoJSON;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace GeoJsonCityBuilder.Editor
{
    public class PrefabsFromGeoJsonBuilder {
        public PrefabsFromGeoJson Component
        {
            get; 
            private set;
        }

        Coordinate m_origin;
        List<PointGeometry> m_geometries;

        public PrefabsFromGeoJsonBuilder(PrefabsFromGeoJson component)
        {
            this.Component = component;
            this.m_origin = component.GetComponent<PositionOnWorldCoordinates>().Origin;

        }

        private void DeserializeGeoJson()
        {
            var geoJSON = new GeoJSONObject(this.Component.geoJsonFile.text);
            var filteredGeometries =
                from feature in geoJSON.FeatureCollection.Features
                where this.Component.featureTypeFilter == "" || feature.Properties.Type == this.Component.featureTypeFilter
                select feature.Geometry as PointGeometry;
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

                var positionComponent = go.AddComponent<PositionOnWorldCoordinates>();
                positionComponent.Origin = geometry.Coordinate;

                positionComponent.Recalculate();

                go.transform.Rotate(0f, Random.Range(0f, 360f), 0, Space.Self);
            }
        }
    }
}