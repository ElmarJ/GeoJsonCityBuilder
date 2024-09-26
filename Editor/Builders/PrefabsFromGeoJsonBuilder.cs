using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using GeoJsonCityBuilder.Components;
using GeoJsonCityBuilder.Editor.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GeoJsonCityBuilder.Editor.Builders
{
    public class PrefabsFromGeoJsonBuilder
    {
        public PrefabsFromGeoJson Component
        {
            get;
            private set;
        }

        private List<Feature> m_features;

        public PrefabsFromGeoJsonBuilder(PrefabsFromGeoJson component)
        {
            Component = component;
        }

        private void DeserializeGeoJson()
        {
            var geoJSON = JsonConvert.DeserializeObject<FeatureCollection>(Component.geoJsonFile.text);
            var filteredFeatures =
                from feature in geoJSON.Features
                where feature.Geometry.Type == GeoJSON.Net.GeoJSONObjectType.Point && (Component.featureTypeFilter == "" || (feature.Properties.ContainsKey("type")
                                                                 && feature.Properties["type"] != null
                                                                 && feature.Properties["type"].ToString() == Component.featureTypeFilter))
                select feature as Feature;
            m_features = filteredFeatures.ToList();
        }

        public void RemoveAllChildren()
        {
            if (Application.IsPlaying(Component.gameObject))
            {
                foreach (Transform child in Component.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
            else
            {
                while (Component.transform.childCount > 0)
                {
                    GameObject.DestroyImmediate(Component.transform.GetChild(0).gameObject);
                }
            }
        }

        public void Rebuild(int seed = 42)
        {
            RemoveAllChildren();
            DeserializeGeoJson();

            Random.InitState(seed);

            foreach (Feature feature in m_features)
            {
                var point = feature.Geometry as Point;
                var go = (GameObject)PrefabUtility.InstantiatePrefab(Component.prefab, Component.transform);

                // Todo: solve this, we shouldn't assign to positionComponent.
                var worldOrigin = Component.worldPosition.SceneOrigin;

                var position = point.Coordinates.ToCoordinate().ToLocalPosition(worldOrigin, go.transform.position.y);
                go.transform.localPosition = position;
                go.transform.Rotate(0f, Random.Range(0f, 360f), 0, Space.Self);

                var featureComponent = go.AddComponent<GeoJsonFeatureInstance>();
                featureComponent.Properties = new Dictionary<string, object>(feature.Properties);
                foreach (var property in feature.Properties)
                {
                    if (property.Value != null)
                    {
                        featureComponent.Properties[property.Key] = property.Value;
                    }
                }

                var existenceController = go.AddComponent<ExistenceController>();
                existenceController.existencePeriodStart = feature.Properties.ContainsKey(Component.timeStartYearField) && feature.Properties[Component.timeStartYearField] != null ? (long)feature.Properties[Component.timeStartYearField] : -9999;
                existenceController.existencePeriodEnd = feature.Properties.ContainsKey(Component.timeEndYearField) && feature.Properties[Component.timeEndYearField] != null ? (long)feature.Properties[Component.timeEndYearField] : 9999;
            }
        }
    }
}