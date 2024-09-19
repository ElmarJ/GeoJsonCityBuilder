using System.Collections.Generic;

using System.Linq;
using GeoJSON.Net;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using GeoJsonCityBuilder.Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace GeoJsonCityBuilder.Editor
{
    public class BordersFromGeoJsonBuilder
    {
        List<Feature> m_features;

        public BordersFromGeoJson Component { get; private set; }

        public BordersFromGeoJsonBuilder(BordersFromGeoJson bordersFromGeoJsonComponent)
        {
            this.Component = bordersFromGeoJsonComponent;
        }

        private void DeserializeGeoJson()
        {
            var geoJSON = JsonConvert.DeserializeObject<FeatureCollection>(this.Component.geoJsonFile.text);
            var features =
                from feature in geoJSON.Features
                where feature.Geometry.Type == GeoJSONObjectType.Polygon
                select feature;

            this.m_features = features.ToList();
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

        public void Rebuild(int seed = 42)
        {
            this.RemoveAllChildren();
            this.DeserializeGeoJson();

            Random.InitState(seed);

            var origin = Component.worldPositionAnchor.SceneOrigin;

            int i = 0;

            foreach (var feature in this.m_features)
            {
                var geometry = feature.Geometry as Polygon;

                var border = new GameObject(); 
                border.name = "Border " + i++.ToString();
                border.transform.parent = this.Component.transform;
                border.transform.position = this.Component.transform.position;

                var existenceController = border.AddComponent<ExistenceController>();
                existenceController.existencePeriodStart = feature.Properties.ContainsKey("exist.period.start") && feature.Properties["exist.period.start"] != null ? (long)feature.Properties["exist.period.start"] : -9999;
                existenceController.existencePeriodEnd = feature.Properties.ContainsKey("exist.period.end") && feature.Properties["exist.period.end"] != null ? (long)feature.Properties["exist.period.end"] : 9999;

                var controller = border.AddComponent<BorderFromPolygon>();
                controller.height = this.Component.height;
                controller.outerExtension = this.Component.outerExtension;
                controller.innerExtension = this.Component.innerExtension;
                controller.material = this.Component.material;
                controller.sideUvUnwrapSettings = this.Component.sideUvUnwrapSettings;

                controller.floorPolygon = new List<Vector3>(from coor in geometry.Coordinates[0].Coordinates select new Vector3(coor.ToCoordinate().ToLocalGrid(origin).x, 0, coor.ToCoordinate().ToLocalGrid(origin).y));

                var borderBuilder = new BorderFromPolygonBuilder(controller);
                borderBuilder.Draw();
            }
        }
    }
}