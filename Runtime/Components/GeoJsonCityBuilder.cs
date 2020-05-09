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
        public Material waterMaterial;
        public Material streetMaterial;
        public GameObject treePrefab;

        private Coordinate origin;

        private FeatureCollection dataFromJson;
        
        // Start is called before the first frame update
        void Start()
        {
            origin = GetComponent<PositionOnWorldCoordinates>().Origin;
        }
        void DeserializeGeoJson()
        {
            var geoJSON = new GeoJSONObject(geoJsonFile.text);
            dataFromJson = geoJSON.FeatureCollection;
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
            GameObject terrainObject = Poly2Mesh.CreateGameObject(poly);
            terrainObject.name = "Street";
            terrainObject.transform.parent = transform;
            
            // Probuilderize:
            var importer = new MeshImporter(terrainObject);
            importer.Import();

            // Add collider:
            var collider = terrainObject.AddComponent<MeshCollider>();

            // Set material and use auto UV using world space (to prevent stretching):

            var terrainMesh = terrainObject.GetComponent<ProBuilderMesh>();
            foreach(var face in terrainMesh.faces)
            {
                face.manualUV = false;
            }
            terrainObject.GetComponent<MeshRenderer>().material = this.streetMaterial;

            terrainMesh.ToMesh();            
            terrainMesh.Refresh();


            // Now add the canal containers:
            foreach (var polygon in canalPolygons)
            {
                var canalObject = new GameObject("Canal");
                canalObject.transform.parent = transform;
                var canalMesh = canalObject.AddComponent<ProBuilderMesh>();
                canalObject.AddComponent<MeshCollider>();
                
                var points = (from coor in polygon.Coordinates[0] select new Vector3(coor.ToLocalGrid(origin).x, 0f, coor.ToLocalGrid(origin).y)).ToList();

                var first = points.First();
                var last = points.Last();
                if (first.x == last.x && first.y == last.y && first.z == last.z)
                {
                    points.Remove(points.Last());
                }

                const float canalDepth = 2f;
                
                canalMesh.CreateShapeFromPolygon(points, -1 * canalDepth, false);
                
                // Find the top face:
                Face ceiling = null;
                foreach(var face in canalMesh.faces)
                {
                    ceiling = face;
                    foreach(var vertex in canalMesh.GetVertices(face.distinctIndexes))
                    {
                        if (vertex.position.y < 0)
                        {
                            ceiling = null;
                            break;
                        }
                    }
                    if (ceiling != null)
                    {
                        break;
                    }
                }


                // Delete the top face:
                canalMesh.DeleteFace(ceiling);
                


                // Find the bottom face:
                Face bottom = null;
                foreach(var face in canalMesh.faces)
                {
                    bottom = face;
                    foreach(var vertex in canalMesh.GetVertices(face.distinctIndexes))
                    {
                        if (vertex.position.y == 0)
                        {
                            bottom = null;
                            break;
                        }
                    }
                    if (bottom != null)
                    {
                        break;
                    }
                }

                foreach(var face in canalMesh.faces)
                {
                    face.Reverse();
                }

                canalMesh.ToMesh();

                canalMesh.SetMaterial(canalMesh.faces, this.wallMaterial);
                canalMesh.SetMaterial(new List<Face>() { bottom }, this.waterMaterial);
                //canalMesh.GetComponent<MeshRenderer>().material = this.wallMaterial;


                canalMesh.Refresh();

            }


        }
    }
}