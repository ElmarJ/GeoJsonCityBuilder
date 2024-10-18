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
                var go = (GameObject)PrefabUtility.InstantiatePrefab(Component.prefab, Component.transform);

                // Todo: solve this, we shouldn't assign to positionComponent.
                var worldOrigin = Component.worldPosition.SceneOrigin;

                var position = point.Coordinates.ToCoordinate().ToLocalPosition(worldOrigin, go.transform.position.y);
                go.transform.localPosition = position;
                go.transform.Rotate(0f, UnityEngine.Random.Range(0f, 360f), 0, Space.Self);

                var featureComponent = go.AddComponent<GeoJsonFeatureInstance>();
                featureComponent.Properties = new Dictionary<string, object>(feature.Properties);

                var existenceController = go.AddComponent<ExistenceController>();
                existenceController.existencePeriodStart = GetYearFromField(feature, Component.timeStartYearField) ?? -9999;
                existenceController.existencePeriodEnd = GetYearFromField(feature, Component.timeEndYearField) ?? 9999;
            }
        }
    }
}