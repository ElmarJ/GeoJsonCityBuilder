using GeoJSON.Net;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using GeoJsonCityBuilder.Components;
using GeoJsonCityBuilder.Editor.Helpers;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;

namespace GeoJsonCityBuilder.Editor.Builders
{
    public class BlocksFromGeoJsonBuilder: FeatureCollectionBuilderBase<BlocksFromGeoJson>
    {
        public BlocksFromGeoJsonBuilder(BlocksFromGeoJson component) : base(component)
        {
        }

        private void DeserializeGeoJson()
        {
            this.DeserializeGeoJson(GeoJSONObjectType.Polygon);
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

                if(geometry == null)
                {
                    Debug.LogWarning($"Feature {feature.Properties["id"] ?? feature.Properties["name"] ?? ""} is not a polygon. Skipping.");
                    continue;
                }

                var block = Component.basePrefab ? (GameObject)PrefabUtility.InstantiatePrefab(Component.basePrefab) : new GameObject();
                block.name = $"Block {i++}";
                block.transform.parent = Component.transform;
                block.transform.position = Component.transform.position;

                var featureComponent = block.AddComponent<GeoJsonFeatureInstance>();
                featureComponent.Properties = new Dictionary<string, object>(feature.Properties);

                var existenceController = block.AddComponent<ExistenceController>();
                existenceController.existencePeriodStart = GetYearFromField(feature, Component.timeStartYearField) ?? -9999;
                existenceController.existencePeriodEnd = GetYearFromField(feature, Component.timeEndYearField) ?? 9999;

                // Set GameObject as static if it is present in all years
                if (existenceController.existencePeriodStart == -9999 && existenceController.existencePeriodEnd == 9999)
                {
                    block.isStatic = true;
                }

                var controller = block.AddComponent<BlockFromPolygon>();

                double height = !feature.Properties.ContainsKey(Component.heightProperty) || feature.Properties[Component.heightProperty] == null ? 0 : (double)feature.Properties[Component.heightProperty];
                controller.height = height == 0 ? UnityEngine.Random.Range(Component.heightMin, Component.heightMax) : (float)height;

                controller.sideMaterial = Component.sideMaterials[UnityEngine.Random.Range(0, Component.sideMaterials.Count)];
                controller.topMaterial = Component.topMaterial;
                controller.bottomMaterial = Component.bottomMaterial;
                controller.sideUvUnwrapSettings = Component.sideUvUnwrapSettings;
                controller.pointedRoof = Component.pointedRoofTops;
                controller.raiseFrontAndBackFacadeTop = Component.raiseFacades;

                controller.floorPolygon = new List<Vector3>(from coor in geometry.Coordinates[0].Coordinates select new Vector3(coor.ToCoordinate().ToLocalGrid(origin).x, 0, coor.ToCoordinate().ToLocalGrid(origin).y));
                var blockBuilder = new BlockFromPolygonBuilder(controller);
                blockBuilder.Draw();
            }
        }
    }
}