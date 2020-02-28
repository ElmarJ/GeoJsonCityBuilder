using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace GeoJsonCityBuilder
{
    public class BlockFromFeature : MonoBehaviour
    {
        public IList<Vector3> floor;

        public Material topMaterial;
        public Material bottomMaterial;
        public Material sideMaterial;
        public float height = 10f;

        public void Draw()
        {
            var mesh = gameObject.AddComponent<ProBuilderMesh>();
            if (gameObject.GetComponent<MeshCollider>() == null)
            {
                gameObject.AddComponent<MeshCollider>();
            }

            var first = floor.First();
            var last = floor.Last();
            if (first.x == last.x && first.y == last.y && first.z == last.z)
            {
                floor.Remove(floor.Last());
            }

            mesh.CreateShapeFromPolygon(floor, height, false);

            var pb = gameObject.GetComponent<ProBuilderMesh>();
            if (pb.faceCount > 2)
            {
                pb.SetMaterial(pb.faces, sideMaterial);
                pb.SetMaterial(new List<Face>() { pb.faces[0] }, topMaterial);
                pb.SetMaterial(new List<Face>() { pb.faces[1] }, bottomMaterial);
                pb.ToMesh();
                pb.Refresh();
            }

            // AddPointedRoof();
        }

        public void AddPointedRoof() {
            const float roofHeight = 3f;
            var pb = gameObject.GetComponent<ProBuilderMesh>();            
            if (pb.faces.Count < 5)
            {
                return;
            }
            var topFace = pb.faces[0];
            var wingedEdges = WingedEdge.GetWingedEdges(pb, new Face[] {topFace}, false);


            // For now, this only works on blocks with a building with 4 sides.
            if (topFace.edges.Count != 4)
            {
                return;
            }

            float shortestDistance = 0f;
            Edge shortestEdge = new Edge();
            Edge oppositeEdge = new Edge();

            // find shortest side:
            foreach (var wingedEdge in wingedEdges)
            {
                var vertices = pb.GetVertices(new List<int>() {wingedEdge.edge.common.a, wingedEdge.edge.common.b});
                var edgeLength = Vector3.Distance(vertices[0].position, vertices[1].position);
                if (shortestDistance == 0 || edgeLength < shortestDistance) {
                    shortestDistance = edgeLength;
                    shortestEdge = wingedEdge.edge.local;
                }
            }

            // search the opposite edge (i.e. the edge that shares no corners / vertices with the shortest edge)
            foreach (var wingedEdge in wingedEdges)
            {
                var edge = wingedEdge.edge.local;

                if (edge.a != shortestEdge.a && edge.a != shortestEdge.b && edge.b != shortestEdge.a && edge.b != shortestEdge.b)
                {
                    oppositeEdge = edge;
                    break;
                }
            }
            try {
                var connectResult = pb.Connect(new Edge[] {shortestEdge, oppositeEdge});
                var newEdge = connectResult.item2[0];
                var extrudedEdges = pb.Extrude(new Edge[] {newEdge}, 0f, false, true);           
                pb.TranslateVertices(connectResult.item2, new Vector3(0f, roofHeight, 0f));
                pb.ToMesh();
                pb.Refresh();
            }
            catch (System.Exception ex) {
                
            }
        }
    }
}