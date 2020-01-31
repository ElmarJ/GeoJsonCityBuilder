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
    public class GeoJsonCityBuilder : MonoBehaviour
    {
        public TextAsset geoJsonFile;
        public Material wallMaterial;
        public Material streetMaterial;
        public GameObject treePrefab;

        private Coordinate origin;

        private FeatureCollection dataFromJson;

        // Start is called before the first frame update
        void Start()
        {
            origin = GetComponent<PositionOnWorldCoordinates>().Origin;
        }

        // Update is called once per frame
        void Update()
        {
            if (!Application.IsPlaying(gameObject))
            {
                var geoJSON = new GeoJSONObject(geoJsonFile.text);
                dataFromJson = geoJSON.FeatureCollection;
            }
        }

        void DeserializeGeoJsonIfNecessary()
        {
            if (dataFromJson == null)
            {
                var geoJSON = new GeoJSONObject(geoJsonFile.text);
                dataFromJson = geoJSON.FeatureCollection;
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
            DeserializeGeoJsonIfNecessary();

            CreateTerrain(dataFromJson.Features, 2000f);

            int i = 0;

            foreach (var feature in dataFromJson.Features)
            {
                if (feature.Properties.Type == "Building")
                {
                    var polygon = feature.Geometry as PolygonGeometry;
                    var building = new GameObject("Building " + i++.ToString());

                    building.transform.parent = transform;

                    var controller = building.AddComponent<BuildingController>();
                    controller.height = controller.height + Random.Range(-2f, 2f);
                    controller.wallMaterial = wallMaterial;
                    controller.floor = new List<Vector3>(from coor in polygon.Coordinates[0] select new Vector3(coor.ToLocalGrid(origin).x, 0, coor.ToLocalGrid(origin).y));

                    controller.DrawBuilding();
                }

                if (feature.Properties.Type == "Tree")
                {
                    var tree = Instantiate(treePrefab, transform);

                    var point = feature.Geometry as PointGeometry;

                    var positionComponent = tree.AddComponent<PositionOnWorldCoordinates>();
                    positionComponent.Origin = point.Coordinate;

                    positionComponent.Recalculate();
                }
            }

            EditorApplication.QueuePlayerLoopUpdate();
        }

        private void DrawCanals(IEnumerable<Feature> features)
        {
            var canalPolygons = from feature in features where feature.Properties.Type == "Canal" select feature.Geometry as PolygonGeometry;
            foreach (var polygon in canalPolygons)
            {
                var points = from coor in polygon.Coordinates[0] select new Vector3(coor.ToLocalGrid(origin).x, 0.5f, coor.ToLocalGrid(origin).y);
                //            canalController.DrawCanal(points.ToArray(), 2f);
            }
        }

        private void CreateTerrain(List<Feature> features, float sizeBorderToCenter)
        {
            var canalPolygons = (from feature in features where feature.Properties.Type == "Canal" select feature.Geometry as PolygonGeometry).ToList();

            Poly2Mesh.Polygon poly = new Poly2Mesh.Polygon();
            poly.outside = new List<Vector3>() {
                new Vector3(sizeBorderToCenter * -1, 0, sizeBorderToCenter),
                new Vector3(sizeBorderToCenter, 0, sizeBorderToCenter),
                new Vector3(sizeBorderToCenter, 0, sizeBorderToCenter * -1),
                new Vector3(sizeBorderToCenter * -1, 0, sizeBorderToCenter * -1),
            };
            foreach (var polygon in canalPolygons)
            {
                var points = from coor in polygon.Coordinates[0] select new Vector3(coor.ToLocalGrid(origin).x, 0f, coor.ToLocalGrid(origin).y);

                poly.holes.Add(new List<Vector3>(points));
            }
    
            // Set up game object with mesh;
            GameObject go = Poly2Mesh.CreateGameObject(poly);
            go.name = "Street";
            go.transform.parent = transform;
            
            // Probuilderize:
            var filter = go.GetComponent<MeshFilter>();
            var mesh = go.AddComponent<ProBuilderMesh>();
            var importer = new MeshImporter(mesh);
            importer.Import(filter.sharedMesh);
            mesh.ToMesh();

            // Add collider:
            var collider = go.AddComponent<MeshCollider>();

            // Set material and use auto UV using world space (to prevent stretching):
            foreach(var face in mesh.faces)
            {
                face.manualUV = false;
            }
            go.GetComponent<MeshRenderer>().material = this.streetMaterial;
            
            mesh.Refresh();
        }
    }
}