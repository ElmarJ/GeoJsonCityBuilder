using GeoJSON.Net;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using GeoJsonCityBuilder.Components;
using GeoJsonCityBuilder.Editor.Helpers;
using Newtonsoft.Json;
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
                where feature.Geometry.Type == GeoJSONObjectType.Polygon
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

            Random.InitState(seed);

            var origin = Component.worldPositionAnchor.SceneOrigin;

            int i = 0;

            foreach (var feature in m_features)
            {
                var geometry = feature.Geometry as Polygon;

                var border = new GameObject();
                border.name = "Border " + i++.ToString();
                border.transform.parent = Component.transform;
                border.transform.position = Component.transform.position;

                var existenceController = border.AddComponent<ExistenceController>();
                existenceController.existencePeriodStart = feature.Properties.ContainsKey("exist.period.start") && feature.Properties["exist.period.start"] != null ? (long)feature.Properties["exist.period.start"] : -9999;
                existenceController.existencePeriodEnd = feature.Properties.ContainsKey("exist.period.end") && feature.Properties["exist.period.end"] != null ? (long)feature.Properties["exist.period.end"] : 9999;

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