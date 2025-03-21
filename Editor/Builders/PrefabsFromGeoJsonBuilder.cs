using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using GeoJsonCityBuilder.Components;
using GeoJsonCityBuilder.Editor.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GeoJsonCityBuilder.Editor.Builders
{
    public class PrefabsFromGeoJsonBuilder: FeatureCollectionBuilderBase<PrefabsFromGeoJson>
    {
        public PrefabsFromGeoJsonBuilder(PrefabsFromGeoJson component) : base(component)
        {
        }

        private void DeserializeGeoJson()
        {
            this.DeserializeGeoJson(GeoJSON.Net.GeoJSONObjectType.Point);
        }

        public override void Rebuild(int seed = 42)
        {
            RemoveAllChildren();
            DeserializeGeoJson();

            UnityEngine.Random.InitState(seed);

            foreach (Feature feature in m_features)
            {
                var point = feature.Geometry as Point;

                if(point == null)
                {
                    Debug.LogWarning($"Feature {feature.Properties["id"] ?? feature.Properties["name"] ?? ""} is not a polygon. Skipping.");
                    continue;
                }

                var go = (GameObject)PrefabUtility.InstantiatePrefab(Component.prefab, Component.transform);

                // Todo: solve this, we shouldn't assign to position Component.
                var worldOrigin = Component.worldPosition.SceneOrigin;

                var position = point.Coordinates.ToCoordinate().ToLocalPosition(worldOrigin, go.transform.position.y);
                go.transform.localPosition = position;

                var rotation = !feature.Properties.ContainsKey(Component.rotationProperty) || feature.Properties[Component.rotationProperty] == null ? UnityEngine.Random.Range(0f, 360f) : (float)feature.Properties[Component.rotationProperty];
                go.transform.Rotate(0f, rotation, 0, Space.Self);

                var featureComponent = go.AddComponent<GeoJsonFeatureInstance>();
                featureComponent.Properties = new Dictionary<string, object>(feature.Properties);

                var existenceController = go.AddComponent<ExistenceController>();
                existenceController.existencePeriodStart = GetYearFromField(feature, Component.timeStartYearField) ?? -9999;
                existenceController.existencePeriodEnd = GetYearFromField(feature, Component.timeEndYearField) ?? 9999;

                // Set GameObject as static if it is present in all years
                if (existenceController.existencePeriodStart == -9999 && existenceController.existencePeriodEnd == 9999)
                {
                    go.isStatic = true;
                }
            }
        }
    }
}