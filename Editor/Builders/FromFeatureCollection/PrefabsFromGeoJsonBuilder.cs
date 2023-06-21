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
    public class PrefabsFromGeoJsonBuilder: GameObjectsFromGeoJsonBuilder<PrefabsFromGeoJson>
    {
        public PrefabsFromGeoJsonBuilder(PrefabsFromGeoJson component): base(component) { }

        protected override void BuildFromFeatures()
        {
            foreach (Feature feature in this.m_features)
            {
                var point = feature.Geometry as Point;
                var go = Object.Instantiate(this.Component.prefab, this.Component.transform);

                // Todo: solve this, we shouldn't assign to positionComponent.
                var worldOrigin = this.Component.worldPosition.SceneOrigin;
                
                var position = point.Coordinates.ToCoordinate().ToLocalPosition(worldOrigin, go.transform.position.y);
                go.transform.localPosition = position;
                go.transform.Rotate(0f, Random.Range(0f, 360f), 0, Space.Self);

                var featureComponent = go.AddComponent<GeoJsonFeatureInstance>();
                featureComponent.Properties = new Dictionary<string, object>(feature.Properties);

                var existenceController = go.AddComponent<ExistenceController>();
                existenceController.existencePeriodStart = feature.Properties.ContainsKey(this.Component.timeStartYearField) && feature.Properties[this.Component.timeStartYearField] != null ? (long)feature.Properties[this.Component.timeStartYearField] : -9999;
                existenceController.existencePeriodEnd = feature.Properties.ContainsKey(this.Component.timeEndYearField) && feature.Properties[this.Component.timeEndYearField] != null ? (long)feature.Properties[this.Component.timeEndYearField] : 9999;
            }
        }
    }
}