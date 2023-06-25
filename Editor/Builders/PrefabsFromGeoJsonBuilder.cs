using System.Collections.Generic;

using System.Linq;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using GeoJsonCityBuilder.Data;
using UnityEditor;
using UnityEngine;
using GeoJSON.Net.Feature;
using GeoJsonCityBuilder.Editor.Helpers;

namespace GeoJsonCityBuilder.Editor.Builders
{
    public class PrefabsFromGeoJsonBuilder : GameObjectsFromGeoJsonBuilder<PrefabsFromGeoJson>
    {
        public PrefabsFromGeoJsonBuilder(PrefabsFromGeoJson component) : base(component) { }

        protected override GameObject AddFeature(Feature feature, int i)
        {
            var point = feature.Geometry as Point;
            var go = base.AddFeature(feature, i);

            // Todo: solve this, we shouldn't assign to positionComponent.
            var worldOrigin = this.Component.worldPosition.SceneOrigin;

            var position = point.Coordinates.ToCoordinate().ToLocalPosition(worldOrigin, go.transform.position.y);
            go.transform.localPosition = position;
            go.transform.Rotate(0f, Random.Range(0f, 360f), 0, Space.Self);

            return go;
        }
    }
}