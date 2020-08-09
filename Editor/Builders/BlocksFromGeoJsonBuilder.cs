using System.Collections.Generic;

using System.Linq;
using GeoJsonCityBuilder.Data;
using GeoJsonCityBuilder.Data.GeoJSON;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace GeoJsonCityBuilder.Editor
{
    public class BlocksFromGeoJsonBuilder {
        
        Coordinate m_origin;
        List<Feature> m_features;

        public BlocksFromGeoJson Component { get; private set; }

        public BlocksFromGeoJsonBuilder(BlocksFromGeoJson blocksFromGeoJsonComponent)
        {
            this.Component = blocksFromGeoJsonComponent;
            this.m_origin = Component.GetComponent<PositionOnWorldCoordinates>().Origin;
        }


            private void DeserializeGeoJson()
        {
            var geoJSON = new GeoJSONObject(this.Component.geoJsonFile.text);
            var filteredFeatures =
                from feature in geoJSON.FeatureCollection.Features
                select feature;
            
            if (this.Component.featureTypeFilter != null && this.Component.featureTypeFilter != "") {
                filteredFeatures =
                    from feature in filteredFeatures
                    where feature.Properties.Type == this.Component.featureTypeFilter
                    select feature;
            }
            this.m_features = filteredFeatures.ToList();
        }

        void DeserializeGeoJsonIfNecessary()
        {
            if (this.m_features == null)
            {
                DeserializeGeoJson();
            }
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
                var geometry = feature.Geometry as PolygonGeometry;

                var block = new GameObject(this.Component.featureTypeFilter + i++.ToString());
                block.transform.parent = this.Component.transform;
                block.transform.position = this.Component.transform.position;

                var controller = block.AddComponent<BlockFromPolygon>();
                controller.height = feature.Properties.Height == null || feature.Properties.Height == 0 ? Random.Range(this.Component.heightMin, this.Component.heightMax) : feature.Properties.Height.Value;
                
                controller.sideMaterial = this.Component.sideMaterials[Random.Range(0, this.Component.sideMaterials.Count)];
                controller.topMaterial = this.Component.topMaterial;
                controller.bottomMaterial = this.Component.bottomMaterial;
                controller.sideUvUnwrapSettings = this.Component.sideUvUnwrapSettings;
                controller.pointedRoof = this.Component.pointedRoofTops;
                controller.raiseFrontAndBackFacadeTop = this.Component.raiseFacades;

                controller.floorPolygon = new List<Vector3>(from coor in geometry.Coordinates[0] select new Vector3(coor.ToLocalGrid(m_origin).x, 0, coor.ToLocalGrid(m_origin).y));

                var blockBuilder = new BlockFromPolygonBuilder(controller);
                blockBuilder.Draw();
            }
        }
    }
}