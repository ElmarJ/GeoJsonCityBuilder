using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace GeoJsonCityBuilder.Editor.Builders
{
    public class BlockFromPolygonBuilder: MeshFromPolygonBuilder<BlockFromPolygon>
    {
        bool fourSides;
        Face topFace;
        Edge topFaceShortestEdge = new();
        Edge topFaceShortestEdgeCommon = new();
        Edge topFaceOppositeEdge = new();
        Edge topFaceOppositeEdgeCommon = new();
        Edge frontFaceTopEdge = new();
        Edge backFaceTopEdge = new();

        public BlockFromPolygonBuilder(BlockFromPolygon blockInfo): base(blockInfo)
        {
        }

        private ProBuilderMesh pb;

        public override void Draw()
        {
            var mesh = this.GameObject.GetComponent<ProBuilderMesh>() ?? this.GameObject.AddComponent<ProBuilderMesh>();
            
            // Check if there are enough points to create a polygon:
            if (this.BuilderInfo.polygon.Count < 3)
            {
                return;
            }
            
            // Sanitize the polygon:
            this.SanitizePolygon();

            // Create the mesh:
            mesh.CreateShapeFromPolygon(this.BuilderInfo.polygon, this.BuilderInfo.height, false);

            this.SetMaterials();
            this.SetUVSettings();

            // Store the mesh:
            this.pb.ToMesh();
            this.pb.Refresh();

            // Label the faces of the mesh:
            this.FindSpecialFaces();

            // Lean the building forward:
            this.LeanForward();

            // Add a pointed roof:
            if (this.BuilderInfo.pointedRoof)
            {
                this.AddPointedRoof();
            }
        }

        private void SanitizePolygon()
        {
            // Remove last point if it's the same as the first point:
            var first = this.BuilderInfo.polygon.First();
            var last = this.BuilderInfo.polygon.Last();
            if (first.x == last.x && first.y == last.y && first.z == last.z)
            {
                this.BuilderInfo.polygon.Remove(this.BuilderInfo.polygon.Last());
            }
        }

        private void SetUVSettings()
        {
            var i = 0;
            foreach (var face in this.pb.faces)
            {
                // Skip the bottom and ceiling face:
                if (i > 1)
                {
                    face.uv = this.BuilderInfo.sideUvUnwrapSettings;
                }
                i++;
            }
        }

        private void SetMaterials()
        {
            this.pb = GameObject.GetComponent<ProBuilderMesh>();
            if (this.pb.faceCount > 2)
            {
                this.pb.SetMaterial(this.pb.faces, this.BuilderInfo.sideMaterial);
                this.pb.SetMaterial(new List<Face>() { this.pb.faces[0] }, this.BuilderInfo.topMaterial);
                this.pb.SetMaterial(new List<Face>() { this.pb.faces[1] }, this.BuilderInfo.bottomMaterial);
            }
        }

        private void FindSpecialFaces()
        {
            if (this.pb.faces.Count < 5)
            {
                return;
            }

            this.topFace = this.pb.faces[0];
            var topWingedEdges = WingedEdge.GetWingedEdges(this.pb, new Face[] { this.topFace }, false);

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
                var vertices = this.pb.GetVertices(new List<int>() { wingedEdge.edge.local.a, wingedEdge.edge.local.b });
                var edgeLength = Vector3.Distance(vertices[0].position, vertices[1].position);
                if (shortestDistance == 0 || edgeLength < shortestDistance)
                {
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

            var wingedEdges = WingedEdge.GetWingedEdges(this.pb, this.pb.faces, false);
            foreach (var wingedEdge in wingedEdges)
            {
                if (wingedEdge.edge.common == this.topFaceShortestEdgeCommon && wingedEdge.face != this.topFace)
                {
                    this.frontFaceTopEdge = wingedEdge.edge.local;
                }
                if (wingedEdge.edge.common == this.topFaceOppositeEdgeCommon && wingedEdge.face != this.topFace)
                {
                    this.backFaceTopEdge = wingedEdge.edge.local;
                }
            }
        }

        public void AddPointedRoof()
        {
            if (!this.fourSides)
            {
                return;
            }

            try
            {
                // Optionally, pull up back and front facades:
                if (this.BuilderInfo.raiseFrontAndBackFacadeTop)
                {
                    VertexEditing.SplitVertices(this.pb, this.frontFaceTopEdge);
                    VertexEditing.SplitVertices(this.pb, this.backFaceTopEdge);
                    pb.TranslateVertices(new Edge[] { this.frontFaceTopEdge, this.backFaceTopEdge }, new Vector3(0f, this.BuilderInfo.pointedRoofHeight, 0f));
                }

                // Draw new top-ridge as edge connecting center shortest side and its opposite side
                var connectResult = this.pb.Connect(new Edge[] { this.topFaceShortestEdge, this.topFaceOppositeEdge });
                var newEdge = connectResult.item2[0];

                // Pull this new edge up:
                var extrudedEdges = this.pb.Extrude(new Edge[] { newEdge }, 0f, false, true);
                this.pb.TranslateVertices(connectResult.item2, new Vector3(0f, this.BuilderInfo.pointedRoofHeight, 0f));

                this.pb.ToMesh();
                this.pb.Refresh();
            }
            catch (System.Exception)
            {

            }
        }

        public void LeanForward()
        {

            if (!this.fourSides | this.BuilderInfo.leanForward == 0)
            {
                return;
            }

            this.LeanForwardFromTopEdge(this.topFaceShortestEdgeCommon);
            this.LeanForwardFromTopEdge(this.topFaceOppositeEdgeCommon);
        }

        public void LeanForwardFromTopEdge(Edge edge)
        {
            var edgePoints = this.pb.GetVertices(new List<int>() { edge.a, edge.b });
            var vector = edgePoints[1].position - edgePoints[0].position;
            var transform = new Vector3(vector.z * this.BuilderInfo.leanForward / vector.magnitude, 0, vector.x * this.BuilderInfo.leanForward / vector.magnitude);

            pb.TranslateVertices(new List<Edge>() { this.topFaceShortestEdge }, transform);
            pb.ToMesh();
            pb.Refresh();
        }
    }
}