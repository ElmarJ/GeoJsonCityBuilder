using System.Collections.Generic;

using System.Linq;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using GeoJsonCityBuilder.Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace GeoJsonCityBuilder.Editor
{
    public class BordersFromGeoJsonBuilder: GameObjectsFromGeoJsonBuilder<BordersFromGeoJson>
    {
        public BordersFromGeoJsonBuilder(BordersFromGeoJson component): base(component) { }

        protected override void BuildFromFeatures()
        {
            var origin = Component.worldPosition.SceneOrigin;

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

                controller.polygon = new List<Vector3>(from coor in geometry.Coordinates[0].Coordinates select new Vector3(coor.ToCoordinate().ToLocalGrid(origin).x, 0, coor.ToCoordinate().ToLocalGrid(origin).y));

                var borderBuilder = new BorderFromPolygonBuilder(controller);
                borderBuilder.Draw();
            }
        }
    }
}