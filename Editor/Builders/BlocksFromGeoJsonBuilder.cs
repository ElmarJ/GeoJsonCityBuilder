using System;
using System.Collections.Generic;

using System.Linq;
using GeoJSON.Net;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using GeoJsonCityBuilder;
using GeoJsonCityBuilder.Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace GeoJsonCityBuilder.Editor
{
    public class BlocksFromGeoJsonBuilder
    {
        List<Feature> m_features;

        public BlocksFromGeoJson Component { get; private set; }

        public BlocksFromGeoJsonBuilder(BlocksFromGeoJson blocksFromGeoJsonComponent)
        {
            this.Component = blocksFromGeoJsonComponent;
        }

        private void DeserializeGeoJson()
        {
            if(this.Component.geoJsonFile == null)
            {
                Debug.LogError($"GeoJson file not set on {this.Component.gameObject.name}");
                return;
            }
            var geoJSON = JsonConvert.DeserializeObject<FeatureCollection>(this.Component.geoJsonFile.text);
            var filteredFeatures =
                from feature in geoJSON.Features
                where feature.Geometry.Type == GeoJSONObjectType.Polygon
                select feature;

            if (this.Component.featureTypeFilter != null && this.Component.featureTypeFilter != "")
            {
                filteredFeatures =
                    from feature in filteredFeatures
                    where feature.Properties["type"].ToString() == this.Component.featureTypeFilter
                    select feature;
            }
            this.m_features = filteredFeatures.ToList();
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
                for(int i = this.Component.transform.childCount - 1; i >= 0; i--)
                {
                    GameObject.DestroyImmediate(this.Component.transform.GetChild(0).gameObject);
                }

                if (this.Component.transform.childCount > 0)
                {
                    Debug.LogError("Failed to remove all children");
                }
            }
        }

        public void Rebuild(int seed = 42)
        {
            this.RemoveAllChildren();
            this.DeserializeGeoJson();

            UnityEngine.Random.InitState(seed);

            var origin = Component.worldPositionAnchor.SceneOrigin;

            int i = 0;

            foreach (var feature in this.m_features)
            {
                var geometry = feature.Geometry as Polygon;

                var block = this.Component.basePrefab ? GameObject.Instantiate(this.Component.basePrefab) : new GameObject(); 
                block.name = this.Component.featureTypeFilter + i++.ToString();
                block.transform.parent = this.Component.transform;
                block.transform.position = this.Component.transform.position;

                var featureComponent = block.AddComponent<GeoJsonFeatureInstance>();
                featureComponent.Properties = new Dictionary<string, object>(feature.Properties);
                foreach (var property in feature.Properties)
                {
                    if (property.Value != null)
                    {
                        featureComponent.Properties[property.Key] = property.Value;
                    }
                }


                var existenceController = block.AddComponent<ExistenceController>();
                existenceController.existencePeriodStart = feature.Properties.ContainsKey(this.Component.timeStartYearField) && feature.Properties[this.Component.timeStartYearField] != null ? (long)feature.Properties[this.Component.timeStartYearField] : -9999;
                existenceController.existencePeriodEnd = feature.Properties.ContainsKey(this.Component.timeEndYearField) && feature.Properties[this.Component.timeEndYearField] != null ? (long)feature.Properties[this.Component.timeEndYearField] : 9999;

                var controller = block.AddComponent<BlockFromPolygon>();

                double height = !feature.Properties.ContainsKey("height") || feature.Properties["height"] == null  ? 0 : (double)feature.Properties["height"];
                controller.height = height == 0 ? UnityEngine.Random.Range(this.Component.heightMin, this.Component.heightMax) : (float)height;

                controller.sideMaterial = this.Component.sideMaterials[UnityEngine.Random.Range(0, this.Component.sideMaterials.Count)];
                controller.topMaterial = this.Component.topMaterial;
                controller.bottomMaterial = this.Component.bottomMaterial;
                controller.sideUvUnwrapSettings = this.Component.sideUvUnwrapSettings;
                controller.pointedRoof = this.Component.pointedRoofTops;
                controller.raiseFrontAndBackFacadeTop = this.Component.raiseFacades;

                controller.floorPolygon = new List<Vector3>(from coor in geometry.Coordinates[0].Coordinates select new Vector3(coor.ToCoordinate().ToLocalGrid(origin).x, 0, coor.ToCoordinate().ToLocalGrid(origin).y));
                var blockBuilder = new BlockFromPolygonBuilder(controller);
                blockBuilder.Draw();
            }
        }
    }
}