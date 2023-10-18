using System.Collections.Generic;

using System.Linq;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using GeoJsonCityBuilder;
using GeoJsonCityBuilder.Data;
using GeoJsonCityBuilder.Editor.Helpers;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace GeoJsonCityBuilder.Editor.Builders
{
    public class BlocksFromGeoJsonBuilder: GameObjectsFromGeoJsonBuilder<BlocksFromGeoJson>
    {
        public BlocksFromGeoJsonBuilder(BlocksFromGeoJson component): base(component) { }

        protected override GameObject AddFeature(Feature feature, int i)
        {
            var origin = Component.worldPosition.SceneOrigin;

            // TODO: handle incorrect feature type (throw exception)
            var geometry = feature.Geometry as Polygon;

            var block = base.AddFeature(feature, i);
            var controller = block.AddComponent<BlockFromPolygon>();

            controller.height = Random.Range(this.Component.heightMin, this.Component.heightMax);

            controller.sideMaterial = this.Component.sideMaterials[Random.Range(0, this.Component.sideMaterials.Count)];
            controller.topMaterial = this.Component.topMaterial;
            controller.bottomMaterial = this.Component.bottomMaterial;
            controller.sideUvUnwrapSettings = this.Component.sideUvUnwrapSettings;
            
            controller.pointedRoof = this.Component.pointedRoofTops;
            controller.raiseFrontAndBackFacadeTop = this.Component.raiseFacades;

            controller.polygon = new List<Vector3>(from coor in geometry.Coordinates[0].Coordinates select new Vector3(coor.ToCoordinate().ToLocalGrid(origin).x, 0, coor.ToCoordinate().ToLocalGrid(origin).y));
            var blockBuilder = new BlockFromPolygonBuilder(controller);
            blockBuilder.Draw();

            return block;
        }
    }
}