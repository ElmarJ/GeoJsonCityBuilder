using GeoJSON.Net;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using GeoJsonCityBuilder.Components;
using GeoJsonCityBuilder.Editor.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GeoJsonCityBuilder.Editor.Builders
{
    public class BordersFromGeoJsonBuilder: FeatureCollectionBuilderBase<BordersFromGeoJson>
    {
        public BordersFromGeoJsonBuilder(BordersFromGeoJson component) : base(component)
        {
        }

        private void DeserializeGeoJson()
        {
            DeserializeGeoJson(GeoJSONObjectType.Polygon);
        }

        public override void Rebuild(int seed = 42)
        {
            RemoveAllChildren();
            DeserializeGeoJson();

            UnityEngine.Random.InitState(seed);

            var origin = Component.worldPositionAnchor.SceneOrigin;

            int i = 0;

            foreach (var feature in m_features)
            {
                var geometry = feature.Geometry as Polygon;

                var border = new GameObject();
                border.name = "Border " + i++.ToString();
                border.transform.parent = Component.transform;
                border.transform.position = Component.transform.position;

                var featureComponent = border.AddComponent<GeoJsonFeatureInstance>();
                featureComponent.Properties = new Dictionary<string, object>(feature.Properties);

                var existenceController = border.AddComponent<ExistenceController>();
                existenceController.existencePeriodStart = GetYearFromField(feature, Component.timeStartYearField) ?? -9999;
                existenceController.existencePeriodEnd = GetYearFromField(feature, Component.timeEndYearField) ?? 9999;

                var controller = border.AddComponent<BorderFromPolygon>();
                controller.height = Component.height;
                controller.outerExtension = Component.outerExtension;
                controller.innerExtension = Component.innerExtension;
                controller.material = Component.material;
                controller.sideUvUnwrapSettings = Component.sideUvUnwrapSettings;

                controller.floorPolygon = new List<Vector3>(from coor in geometry.Coordinates[0].Coordinates select new Vector3(coor.ToCoordinate().ToLocalGrid(origin).x, 0, coor.ToCoordinate().ToLocalGrid(origin).y));

                var borderBuilder = new BorderFromPolygonBuilder(controller);
                borderBuilder.Draw();
            }
        }
    }
}