using System.Collections.Generic;
using System.Linq;
using GeoJsonCityBuilder.Components;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace GeoJsonCityBuilder.Editor.Builders
{
    public class BlockFromPolygonBuilder
    {
        public BlockFromPolygon BlockInfo { get; private set; }
        public GameObject GameObject { get; private set; }

        private Face topFace;
        private Edge topFaceShortestEdge = new();
        private Edge topFaceShortestEdgeCommon = new();
        private Edge topFaceOppositeEdge = new();
        private Edge topFaceOppositeEdgeCommon = new();
        private Edge frontFaceTopEdge = new();
        private Edge backFaceTopEdge = new();

        public BlockFromPolygonBuilder(BlockFromPolygon blockInfo)
        {
            BlockInfo = blockInfo;
            GameObject = blockInfo.gameObject;
        }

        private ProBuilderMesh pb;
        private bool fourSides = false;
        public void Draw()
        {
            if (!GameObject.TryGetComponent<ProBuilderMesh>(out var mesh))
            {
                mesh = GameObject.AddComponent<ProBuilderMesh>();
            }

            var first = BlockInfo.floorPolygon.First();
            var last = BlockInfo.floorPolygon.Last();
            if (first.x == last.x && first.y == last.y && first.z == last.z)
            {
                var success = BlockInfo.floorPolygon.Remove(BlockInfo.floorPolygon.Last());
                if (!success)
                {
                    Debug.LogWarning($"Could not remove duplicate point on [{this.BlockInfo.name}]");
                }
            }

            var result = mesh.CreateShapeFromPolygon(BlockInfo.floorPolygon, BlockInfo.height, false);

            if (result.status != ActionResult.Status.Success)
            {
                Debug.LogWarning($"Could not create mesh for [{this.BlockInfo.name}]: {result.notification}");
            }

            pb = GameObject.GetComponent<ProBuilderMesh>();
            if (pb.faceCount > 2)
            {
                pb.SetMaterial(pb.faces, BlockInfo.sideMaterial);
                pb.SetMaterial(new List<Face>() { pb.faces[0] }, BlockInfo.topMaterial);
                pb.SetMaterial(new List<Face>() { pb.faces[1] }, BlockInfo.bottomMaterial);
            }

            var i = 0;
            foreach (var face in pb.faces)
            {
                // Skip the bottom and ceiling face:
                if (i > 1)
                {
                    face.uv = BlockInfo.sideUvUnwrapSettings;
                }
                i++;
            }
            pb.ToMesh();
            pb.Refresh();

            FindSpecialSides();

            LeanForward();

            if (BlockInfo.pointedRoof)
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

            topFace = pb.faces[0];
            var topWingedEdges = WingedEdge.GetWingedEdges(pb, new Face[] { topFace }, false);

            // For now, this only works on blocks with a building with 4 sides.
            if (topFace.edges.Count != 4)
            {
                return;
            }

            fourSides = true;
            float shortestDistance = 0f;

            // find shortest side:
            foreach (var wingedEdge in topWingedEdges)
            {
                var vertices = pb.GetVertices(new List<int>() { wingedEdge.edge.local.a, wingedEdge.edge.local.b });
                var edgeLength = Vector3.Distance(vertices[0].position, vertices[1].position);
                if (shortestDistance == 0 || edgeLength < shortestDistance)
                {
                    shortestDistance = edgeLength;
                    topFaceShortestEdge = wingedEdge.edge.local;
                    topFaceShortestEdgeCommon = wingedEdge.edge.common;
                }
            }

            // search the opposite edge (i.e. the edge that shares no corners / vertices with the shortest edge)
            foreach (var wingedEdge in topWingedEdges)
            {
                var edge = wingedEdge.edge.local;

                if (edge.a != topFaceShortestEdge.a && edge.a != topFaceShortestEdge.b && edge.b != topFaceShortestEdge.a && edge.b != topFaceShortestEdge.b)
                {
                    topFaceOppositeEdge = edge;
                    topFaceOppositeEdgeCommon = wingedEdge.edge.common;
                    break;
                }
            }

            var wingedEdges = WingedEdge.GetWingedEdges(pb, pb.faces, false);
            foreach (var wingedEdge in wingedEdges)
            {
                if (wingedEdge.edge.common == topFaceShortestEdgeCommon && wingedEdge.face != topFace)
                {
                    frontFaceTopEdge = wingedEdge.edge.local;
                }
                if (wingedEdge.edge.common == topFaceOppositeEdgeCommon && wingedEdge.face != topFace)
                {
                    backFaceTopEdge = wingedEdge.edge.local;
                }
            }
        }

        public void AddPointedRoof()
        {
            if (!fourSides)
            {
                return;
            }

            try
            {
                // Optionally, pull up back and front facades:
                if (BlockInfo.raiseFrontAndBackFacadeTop)
                {
                    VertexEditing.SplitVertices(pb, frontFaceTopEdge);
                    VertexEditing.SplitVertices(pb, backFaceTopEdge);
                    pb.TranslateVertices(new Edge[] { frontFaceTopEdge, backFaceTopEdge }, new Vector3(0f, BlockInfo.pointedRoofHeight, 0f));
                }

                // Draw new top-ridge as edge connecting center shortest side and its opposite side
                var connectResult = pb.Connect(new Edge[] { topFaceShortestEdge, topFaceOppositeEdge });
                var newEdge = connectResult.item2[0];

                // Pull this new edge up:
                var extrudedEdges = pb.Extrude(new Edge[] { newEdge }, 0f, false, true);
                pb.TranslateVertices(connectResult.item2, new Vector3(0f, BlockInfo.pointedRoofHeight, 0f));

                pb.ToMesh();
                pb.Refresh();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public void LeanForward()
        {

            if (!fourSides | BlockInfo.leanForward == 0)
            {
                return;
            }

            LeanForwardFromTopEdge(topFaceShortestEdgeCommon);
            LeanForwardFromTopEdge(topFaceOppositeEdgeCommon);
        }

        public void LeanForwardFromTopEdge(Edge edge)
        {
            var edgePoints = pb.GetVertices(new List<int>() { edge.a, edge.b });
            var vector = edgePoints[1].position - edgePoints[0].position;
            var transform = new Vector3(vector.z * BlockInfo.leanForward / vector.magnitude, 0, vector.x * BlockInfo.leanForward / vector.magnitude);

            pb.TranslateVertices(new List<Edge>() { topFaceShortestEdge }, transform);
            pb.ToMesh();
            pb.Refresh();
        }
    }
}