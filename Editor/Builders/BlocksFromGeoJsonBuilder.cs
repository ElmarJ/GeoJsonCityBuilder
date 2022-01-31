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
    public class BlocksFromGeoJsonBuilder
    {
        readonly Coordinate m_origin;
        List<Feature> m_features;

        public BlocksFromGeoJson Component { get; private set; }

        public BlocksFromGeoJsonBuilder(BlocksFromGeoJson blocksFromGeoJsonComponent)
        {
            this.Component = blocksFromGeoJsonComponent;
            this.m_origin = Component.worldPositionAnchor.SceneOrigin;
        }

        private void DeserializeGeoJson()
        {
            var geoJSON = JsonConvert.DeserializeObject<FeatureCollection>(this.Component.geoJsonFile.text);
            var filteredFeatures =
                from feature in geoJSON.Features
                select feature;

            if (this.Component.featureTypeFilter != null && this.Component.featureTypeFilter != "")
            {
                filteredFeatures =
                    from feature in filteredFeatures
                    where feature.Properties["Type"].ToString() == this.Component.featureTypeFilter
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
                while (this.Component.transform.childCount > 0)
                {
                    GameObject.DestroyImmediate(this.Component.transform.GetChild(0).gameObject);
                }
            }
        }

        public void Rebuild()
        {
            this.RemoveAllChildren();
            this.DeserializeGeoJson();

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


                var existenceController = block.AddComponent<ExistenceController>();
                existenceController.existencePeriodStart = feature.Properties.ContainsKey("ExistencePeriodStartYear") ? (long)feature.Properties["ExistencePeriodStartYear"] : -9999;
                existenceController.existencePeriodEnd = feature.Properties.ContainsKey("ExistencePeriodEndYear") ? (long)feature.Properties["ExistencePeriodEndYear"] : 9999;

                var controller = block.AddComponent<BlockFromPolygon>();

                controller.height = !feature.Properties.ContainsKey("Height") || (float)feature.Properties["Height"] == 0 ? Random.Range(this.Component.heightMin, this.Component.heightMax) : (long)feature.Properties["Height"];

                controller.sideMaterial = this.Component.sideMaterials[Random.Range(0, this.Component.sideMaterials.Count)];
                controller.topMaterial = this.Component.topMaterial;
                controller.bottomMaterial = this.Component.bottomMaterial;
                controller.sideUvUnwrapSettings = this.Component.sideUvUnwrapSettings;
                controller.pointedRoof = this.Component.pointedRoofTops;
                controller.raiseFrontAndBackFacadeTop = this.Component.raiseFacades;

                controller.floorPolygon = new List<Vector3>(from coor in geometry.Coordinates[0].Coordinates select new Vector3(coor.ToCoordinate().ToLocalGrid(m_origin).x, 0, coor.ToCoordinate().ToLocalGrid(m_origin).y));
                var blockBuilder = new BlockFromPolygonBuilder(controller);
                blockBuilder.Draw();
            }
        }
    }
}