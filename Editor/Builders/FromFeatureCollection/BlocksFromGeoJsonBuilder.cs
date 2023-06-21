using System.Collections.Generic;

using System.Linq;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using GeoJsonCityBuilder;
using GeoJsonCityBuilder.Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace GeoJsonCityBuilder.Editor
{
    public class BlocksFromGeoJsonBuilder: GameObjectsFromGeoJsonBuilder<BlocksFromGeoJson>
    {
        public BlocksFromGeoJsonBuilder(BlocksFromGeoJson component): base(component) { }

        protected override void BuildFromFeatures()
        {
            var origin = Component.worldPosition.SceneOrigin;

            int i = 0;

            foreach (var feature in this.m_features)
            {
                // TODO: handle incorrect feature type (throw exception)
                var geometry = feature.Geometry as Polygon;

                var block = this.Component.prefab ? GameObject.Instantiate(this.Component.prefab) : new GameObject();
                block.name = this.Component.featureTypeFilter + i++.ToString();
                block.transform.parent = this.Component.transform;
                block.transform.position = this.Component.transform.position;

                var featureComponent = block.AddComponent<GeoJsonFeatureInstance>();
                featureComponent.Properties = new Dictionary<string, object>(feature.Properties);


                var existenceController = block.AddComponent<ExistenceController>();
                existenceController.existencePeriodStart = feature.Properties.ContainsKey(this.Component.timeStartYearField) && feature.Properties[this.Component.timeStartYearField] != null ? (long)feature.Properties[this.Component.timeStartYearField] : -9999;
                existenceController.existencePeriodEnd = feature.Properties.ContainsKey(this.Component.timeEndYearField) && feature.Properties[this.Component.timeEndYearField] != null ? (long)feature.Properties[this.Component.timeEndYearField] : 9999;

                var controller = block.AddComponent<BlockFromPolygon>();

                double height = !feature.Properties.ContainsKey("height") || feature.Properties["height"] == null ? 0 : (double)feature.Properties["height"];
                controller.height = height == 0 ? Random.Range(this.Component.heightMin, this.Component.heightMax) : (float)height;

                controller.sideMaterial = this.Component.sideMaterials[Random.Range(0, this.Component.sideMaterials.Count)];
                controller.topMaterial = this.Component.topMaterial;
                controller.bottomMaterial = this.Component.bottomMaterial;
                controller.sideUvUnwrapSettings = this.Component.sideUvUnwrapSettings;
                controller.pointedRoof = this.Component.pointedRoofTops;
                controller.raiseFrontAndBackFacadeTop = this.Component.raiseFacades;

                controller.polygon = new List<Vector3>(from coor in geometry.Coordinates[0].Coordinates select new Vector3(coor.ToCoordinate().ToLocalGrid(origin).x, 0, coor.ToCoordinate().ToLocalGrid(origin).y));
                var blockBuilder = new BlockFromPolygonBuilder(controller);
                blockBuilder.Draw();
            }
        }
    }
}