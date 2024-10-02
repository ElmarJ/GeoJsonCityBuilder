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
    public class BordersFromGeoJsonBuilder
    {
        private List<Feature> m_features;

        public BordersFromGeoJson Component { get; private set; }

        public BordersFromGeoJsonBuilder(BordersFromGeoJson bordersFromGeoJsonComponent)
        {
            Component = bordersFromGeoJsonComponent;
        }

        private void DeserializeGeoJson()
        {
            var geoJSON = JsonConvert.DeserializeObject<FeatureCollection>(Component.geoJsonFile.text);
            var features =
                from feature in geoJSON.Features
                where feature.Geometry.Type == GeoJSONObjectType.Polygon && 
                    (Component.excludeProperty is null or "" || !feature.Properties.ContainsKey(Component.excludeProperty) || !(bool)feature.Properties[Component.excludeProperty])
                select feature;

            m_features = features.ToList();
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
        private static int? GetYearFromField(Feature feature, string field)
        {
            object value = feature.Properties.ContainsKey(field) ? feature.Properties[field] : null;
            int? year = value switch
            {
                string text => text.Length >= 4 && int.TryParse(text[..4], out int number) ? number : null,
                long y => (int)y,
                int y => y,
                _ => null
            };

            return year;
        }
    }
}