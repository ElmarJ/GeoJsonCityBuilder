using System.Collections.Generic;

using System.Linq;
using GeoJsonCityBuilder.Data.GeoJSON;
using UnityEngine;

namespace GeoJsonCityBuilder
{

    [ExecuteAlways]
    [RequireComponent(typeof(PositionOnWorldCoordinates))]
    public class BlocksFromGeoJson : MonoBehaviour
    {
        public TextAsset geoJsonFile;
        public string featureTypeFilter;
        public float heightMin;
        public float heightMax;
        public Material topMaterial;
        public Material sideMaterial;
        public Material bottomMaterial;

        Coordinate m_origin;
        List<Feature> m_features;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (!Application.IsPlaying(gameObject))
            {
                m_origin = GetComponent<PositionOnWorldCoordinates>().Origin;
                DeserializeGeoJson();
            }
        }

        private void DeserializeGeoJson()
        {
            var geoJSON = new GeoJSONObject(geoJsonFile.text);
            var filteredFeatures =
                from feature in geoJSON.FeatureCollection.Features
                select feature;
            
            if (featureTypeFilter != null && featureTypeFilter != "") {
                filteredFeatures =
                    from feature in filteredFeatures
                    where feature.Properties.Type == featureTypeFilter
                    select feature;
            }
            m_features = filteredFeatures.ToList();
        }

        void DeserializeGeoJsonIfNecessary()
        {
            if (m_features == null)
            {
                DeserializeGeoJson();
            }
        }

        public void RemoveAllChildren()
        {
            if (Application.IsPlaying(gameObject))
            {
                foreach (Transform child in transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
            else
            {
                while (transform.childCount > 0)
                {
                    GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
                }
            }
        }

        public void Rebuild()
        {
            RemoveAllChildren();
            DeserializeGeoJson();

            int i = 0;

            foreach (var feature in m_features)
            {
                var geometry = feature.Geometry as PolygonGeometry;

                var block = new GameObject(featureTypeFilter + i++.ToString());
                block.transform.parent = transform;

                var controller = block.AddComponent<BlockFromFeature>();
                controller.height = feature.Properties.Height == null || feature.Properties.Height == 0 ? Random.Range(heightMin, heightMax) : feature.Properties.Height.Value;

                controller.sideMaterial = sideMaterial;
                controller.topMaterial = topMaterial;
                controller.bottomMaterial = bottomMaterial;

                controller.floor = new List<Vector3>(from coor in geometry.Coordinates[0] select new Vector3(coor.ToLocalGrid(m_origin).x, 0, coor.ToLocalGrid(m_origin).y));

                controller.Draw();
            }
        }
    }
}