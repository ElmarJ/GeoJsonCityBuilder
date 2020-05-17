using System.Collections.Generic;

using System.Linq;
using GeoJsonCityBuilder.Data.GeoJSON;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace GeoJsonCityBuilder
{

    [ExecuteAlways]
    [RequireComponent(typeof(PositionOnWorldCoordinates))]
    public class PrefabsFromGeoJson : MonoBehaviour
    {
        public TextAsset geoJsonFile;
        public string featureTypeFilter;

        public GameObject prefab;

        Coordinate m_origin;
        List<PointGeometry> m_geometries;

        // Start is called before the first frame update
        void Start()
        {
            m_origin = GetComponent<PositionOnWorldCoordinates>().Origin;
        }

        private void DeserializeGeoJson()
        {
            var geoJSON = new GeoJSONObject(geoJsonFile.text);
            var filteredGeometries =
                from feature in geoJSON.FeatureCollection.Features
                where featureTypeFilter == "" || feature.Properties.Type == featureTypeFilter
                select feature.Geometry as PointGeometry;
            m_geometries = filteredGeometries.ToList();
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

            foreach (var geometry in m_geometries)
            {
                var go = Instantiate(prefab, transform);
                

                var positionComponent = go.AddComponent<PositionOnWorldCoordinates>();
                positionComponent.Origin = geometry.Coordinate;

                positionComponent.Recalculate();

                go.transform.Rotate(0f, Random.Range(0f, 360f), 0, Space.Self);
            }
        }
    }
}