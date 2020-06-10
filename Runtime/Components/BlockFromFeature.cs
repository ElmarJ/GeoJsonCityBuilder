using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace GeoJsonCityBuilder
{
    // Require removed, because it caused a build-error
    //   (build apparently tries to remove all
    //    components without checking component-dependencies)
    // [RequireComponent(typeof(ProBuilderMesh))]
    [RequireComponent(typeof(MeshCollider))]
    public class BlockFromFeature : MonoBehaviour
    {
        public IList<Vector3> floor;

        public Material topMaterial;
        public Material bottomMaterial;
        public Material sideMaterial;
        public float height = 10f;

        public bool pointedRoof = false;
        public float pointedRoofHeight = 3f;

        public float leanForward = 0f;

        Edge shortestEdge = new Edge();
        Edge shortestEdgeWall = new Edge();
        Edge oppositeEdge = new Edge();
        Edge oppositeEdgeWall = new Edge();

        bool fourSides = false;

        public AutoUnwrapSettings sideUvUnwrapSettings = new AutoUnwrapSettings();

        public ProBuilderMesh pb;

        public void Draw()
        {
            var mesh = gameObject.GetComponent<ProBuilderMesh>();
            if (mesh == null) {
                mesh = gameObject.AddComponent<ProBuilderMesh>();
            }

            var first = floor.First();
            var last = floor.Last();
            if (first.x == last.x && first.y == last.y && first.z == last.z)
            {
                floor.Remove(floor.Last());
            }

            mesh.CreateShapeFromPolygon(floor, height, false);

            pb = gameObject.GetComponent<ProBuilderMesh>();
            if (pb.faceCount > 2)
            {
                pb.SetMaterial(pb.faces, sideMaterial);
                pb.SetMaterial(new List<Face>() { pb.faces[0] }, topMaterial);
                pb.SetMaterial(new List<Face>() { pb.faces[1] }, bottomMaterial);
            }

            var i = 0;
            foreach (var face in pb.faces)
            {
                // Skip the bottom and ceiling face:
                if (i>1)
                {
                    face.uv = this.sideUvUnwrapSettings;
                }
                i++;
            }
                pb.ToMesh();
                pb.Refresh();

            FindSpecialSides();

            LeanForward();

            if (pointedRoof)
            {
                AddPointedRoof();
            }

        }

        public void FindSpecialSides()
        {
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

            this.fourSides = true;
            float shortestDistance = 0f;


            // find shortest side:
            foreach (var wingedEdge in wingedEdges)
            {
                var vertices = pb.GetVertices(new List<int>() {wingedEdge.edge.local.a, wingedEdge.edge.local.b});
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
        }


        public void AddPointedRoof() {
            if (!fourSides)
            {
                return;
            }

            try {
                var connectResult = pb.Connect(new Edge[] {shortestEdge, oppositeEdge});
                var newEdge = connectResult.item2[0];
                var extrudedEdges = pb.Extrude(new Edge[] {newEdge}, 0f, false, true);           
                pb.TranslateVertices(connectResult.item2, new Vector3(0f, pointedRoofHeight, 0f));

                VertexEditing.SplitVertices(pb, connectResult.item2[0]);
                pb.TranslateVertices(new Edge[] {shortestEdgeWall, oppositeEdgeWall}, new Vector3(0f, pointedRoofHeight, 0f));

                pb.ToMesh();
                pb.Refresh();
            }
            catch (System.Exception) {
                
            }
        }

        public void LeanForward() {

            if (!fourSides | leanForward == 0)
            {
                return;
            }

            LeanForwardFromTopEdge(shortestEdge);
            LeanForwardFromTopEdge(oppositeEdge);
        }

        public void LeanForwardFromTopEdge(Edge edge)
        {
            var edgePoints = pb.GetVertices(new List<int>() {edge.a, edge.b});
            var vector = edgePoints[1].position - edgePoints[0].position;
            var dist = vector.magnitude;
            var transform = new Vector3(vector.z * leanForward / vector.magnitude, 0, vector.x * leanForward / vector.magnitude);

            pb.TranslateVertices(new List<Edge>(){shortestEdge}, transform);
            pb.ToMesh();
            pb.Refresh();
        }

        private Face findWallBelow(Edge edge)
        {
            // find the wall-face below, to get its normal (because we want to stretch in that direction);
            var i = 0;
            foreach (var face in pb.faces) {
                // first we get the bottom and ceiling, we want to skip those
                if (i > 1)
                {
                    if (face.edges.Contains(edge))
                    {
                        return face;
                    }
                }
                i++;
            }

            return null;
        }
    }
}