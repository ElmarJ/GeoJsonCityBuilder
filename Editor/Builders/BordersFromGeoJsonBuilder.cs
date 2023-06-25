using System.Collections.Generic;

using System.Linq;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using GeoJsonCityBuilder.Data;
using GeoJsonCityBuilder.Editor.Helpers;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace GeoJsonCityBuilder.Editor.Builders
{
    public class BordersFromGeoJsonBuilder: GameObjectsFromGeoJsonBuilder<BordersFromGeoJson>
    {
        public BordersFromGeoJsonBuilder(BordersFromGeoJson component): base(component) { }

        protected override GameObject AddFeature(Feature feature, int i)
        {
            var border = base.AddFeature(feature, i);
            
            var origin = Component.worldPosition.SceneOrigin;
            var geometry = feature.Geometry as Polygon;

            var controller = border.AddComponent<BorderFromPolygon>();
            controller.height = this.Component.height;
            controller.outerExtension = this.Component.outerExtension;
            controller.innerExtension = this.Component.innerExtension;
            controller.material = this.Component.material;
            controller.sideUvUnwrapSettings = this.Component.sideUvUnwrapSettings;

            controller.polygon = new List<Vector3>(from coor in geometry.Coordinates[0].Coordinates select new Vector3(coor.ToCoordinate().ToLocalGrid(origin).x, 0, coor.ToCoordinate().ToLocalGrid(origin).y));

            var borderBuilder = new BorderFromPolygonBuilder(controller);
            borderBuilder.Draw();

            return border;
        }
    }
}