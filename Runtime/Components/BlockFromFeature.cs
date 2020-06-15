using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace GeoJsonCityBuilder
{
    // Require removed, because it caused a Unity build-error
    //   with HDRP (build apparently tries to remove all
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
        public bool raiseFrontAndBackFacadeTop = false;
        public float leanForward = 0f;
        Face topFace;
        Face frontFace;
        Face backFace;
        Edge topFaceShortestEdge = new Edge();
        Edge topFaceShortestEdgeCommon = new Edge();
        Edge topFaceOppositeEdge = new Edge();
        Edge topFaceOppositeEdgeCommon = new Edge();
        Edge frontFaceTopEdge = new Edge();
        Edge backFaceTopEdge = new Edge();

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

            this.topFace = pb.faces[0];
            var topWingedEdges = WingedEdge.GetWingedEdges(pb, new Face[] {this.topFace}, false);

            // For now, this only works on blocks with a building with 4 sides.
            if (this.topFace.edges.Count != 4)
            {
                return;
            }

            this.fourSides = true;
            float shortestDistance = 0f;

            // find shortest side:
            foreach (var wingedEdge in topWingedEdges)
            {
                var vertices = pb.GetVertices(new List<int>() {wingedEdge.edge.local.a, wingedEdge.edge.local.b});
                var edgeLength = Vector3.Distance(vertices[0].position, vertices[1].position);
                if (shortestDistance == 0 || edgeLength < shortestDistance) {
                    shortestDistance = edgeLength;
                    this.topFaceShortestEdge = wingedEdge.edge.local;
                    this.topFaceShortestEdgeCommon = wingedEdge.edge.common;
                }
            }


            // search the opposite edge (i.e. the edge that shares no corners / vertices with the shortest edge)
            foreach (var wingedEdge in topWingedEdges)
            {
                var edge = wingedEdge.edge.local;

                if (edge.a != this.topFaceShortestEdge.a && edge.a != this.topFaceShortestEdge.b && edge.b != this.topFaceShortestEdge.a && edge.b != this.topFaceShortestEdge.b)
                {
                    this.topFaceOppositeEdge = edge;
                    this.topFaceOppositeEdgeCommon = wingedEdge.edge.common;
                    break;
                }
            }

            var wingedEdges = WingedEdge.GetWingedEdges(pb, pb.faces, false);
            foreach (var wingedEdge in wingedEdges) {
                if (wingedEdge.edge.common == topFaceShortestEdgeCommon && wingedEdge.face != topFace) {
                    this.frontFace = wingedEdge.face;
                    this.frontFaceTopEdge = wingedEdge.edge.local;
                }
                if (wingedEdge.edge.common == topFaceOppositeEdgeCommon && wingedEdge.face != topFace) {
                    this.backFace = wingedEdge.face;
                    this.backFaceTopEdge = wingedEdge.edge.local;
                }
            }
        }


        public void AddPointedRoof() {
            if (!fourSides)
            {
                return;
            }

            try {
                // Optionally, pull up back and front facades:
                if (this.raiseFrontAndBackFacadeTop) {
                    VertexEditing.SplitVertices(pb, this.frontFaceTopEdge);
                    VertexEditing.SplitVertices(pb, this.backFaceTopEdge);
                    pb.TranslateVertices(new Edge[] {this.frontFaceTopEdge, this.backFaceTopEdge}, new Vector3(0f, pointedRoofHeight, 0f));
                }
                
                // Draw new top-ridge as edge connecting center shortest side and its opposite side
                var connectResult = pb.Connect(new Edge[] {this.topFaceShortestEdge, this.topFaceOppositeEdge});
                var newEdge = connectResult.item2[0];

                // Pull this new edge up:
                var extrudedEdges = pb.Extrude(new Edge[] {newEdge}, 0f, false, true);           
                pb.TranslateVertices(connectResult.item2, new Vector3(0f, pointedRoofHeight, 0f));

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

            LeanForwardFromTopEdge(this.topFaceShortestEdgeCommon);
            LeanForwardFromTopEdge(this.topFaceOppositeEdgeCommon);
        }

        public void LeanForwardFromTopEdge(Edge edge)
        {
            var edgePoints = pb.GetVertices(new List<int>() {edge.a, edge.b});
            var vector = edgePoints[1].position - edgePoints[0].position;
            var dist = vector.magnitude;
            var transform = new Vector3(vector.z * leanForward / vector.magnitude, 0, vector.x * leanForward / vector.magnitude);

            pb.TranslateVertices(new List<Edge>(){this.topFaceShortestEdge}, transform);
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