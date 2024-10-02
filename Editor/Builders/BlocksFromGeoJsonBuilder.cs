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
    public class BlocksFromGeoJsonBuilder
    {
        private List<Feature> m_features;

        public BlocksFromGeoJson Component { get; private set; }

        public BlocksFromGeoJsonBuilder(BlocksFromGeoJson blocksFromGeoJsonComponent)
        {
            Component = blocksFromGeoJsonComponent;
        }

        private void DeserializeGeoJson()
        {
            if (Component.geoJsonFile == null)
            {
                Debug.LogError($"GeoJson file not set on {Component.gameObject.name}");
                return;
            }
            var geoJSON = JsonConvert.DeserializeObject<FeatureCollection>(Component.geoJsonFile.text);

            var filteredFeatures =
                from feature in geoJSON.Features
                where feature.Geometry.Type == GeoJSONObjectType.Polygon
                    && (Component.featureTypeFilter is null or "" || feature.Properties["type"].ToString() == Component.featureTypeFilter)
                select feature;

            m_features = filteredFeatures.ToList();
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
                for (int i = Component.transform.childCount - 1; i >= 0; i--)
                {
                    GameObject.DestroyImmediate(Component.transform.GetChild(0).gameObject);
                }

                if (Component.transform.childCount > 0)
                {
                    Debug.LogError("Failed to remove all children");
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

                var block = Component.basePrefab ? (GameObject)PrefabUtility.InstantiatePrefab(Component.basePrefab) : new GameObject();
                block.name = Component.featureTypeFilter + i++.ToString();
                block.transform.parent = Component.transform;
                block.transform.position = Component.transform.position;

                var featureComponent = block.AddComponent<GeoJsonFeatureInstance>();
                featureComponent.Properties = new Dictionary<string, object>(feature.Properties);

                var existenceController = block.AddComponent<ExistenceController>();
                existenceController.existencePeriodStart = GetYearFromField(feature, Component.timeStartYearField) ?? -9999;
                existenceController.existencePeriodEnd = GetYearFromField(feature, Component.timeEndYearField) ?? 9999;

                var controller = block.AddComponent<BlockFromPolygon>();

                double height = !feature.Properties.ContainsKey("height") || feature.Properties["height"] == null ? 0 : (double)feature.Properties["height"];
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