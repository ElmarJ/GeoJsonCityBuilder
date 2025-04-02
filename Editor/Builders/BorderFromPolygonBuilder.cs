using System;
using System.Collections.Generic;
using System.Linq;
using GeoJsonCityBuilder.Components;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace GeoJsonCityBuilder.Editor.Builders
{
    public class BorderFromPolygonBuilder
    {
        public BorderFromPolygon BorderInfo { get; private set; }
        public GameObject GameObject { get; private set; }

        private List<LinkedCorner> corners;
        private int n;

        public BorderFromPolygonBuilder(BorderFromPolygon borderInfo)
        {
            BorderInfo = borderInfo;
            GameObject = borderInfo.gameObject;
        }

        public void Draw()
        {
            RemoveAllChildren();
            PopulateCornerList();

            var angles =
                from corner in corners
                select Vector3.SignedAngle(corner.current - corner.previous, corner.next - corner.current, Vector3.up);

            var clockwise = angles.Sum() > 0;

            var outerPolygon =
                (from innerCorner in corners
                 select FindOuterPoint(innerCorner, BorderInfo.outerExtension, clockwise)).ToList();

            var innerPolygon =
                (from innerCorner in corners
                 select FindOuterPoint(innerCorner, BorderInfo.innerExtension, !clockwise)).ToList();

            for (int i = 0; i < n; i++)
            {
                var i_next = (i + 1) % n;
                var segmentFloorPolygon = new List<Vector3>()
                {
                    innerPolygon[i],
                    outerPolygon[i],
                    outerPolygon[i_next],
                    innerPolygon[i_next]
                };

                DrawSegment(segmentFloorPolygon, $"Segment {i + 1}");
            }
        }

        private void PopulateCornerList()
        {
            var basePolygon = BorderInfo.floorPolygon;

            n = basePolygon.Count;
            corners = new List<LinkedCorner>();

            for (int i = 0; i < n; i++)
            {
                corners.Add(new LinkedCorner()
                {
                    current = basePolygon[i],
                    previous = basePolygon[i > 0 ? i - 1 : n - 1],
                    next = basePolygon[(i + 1) % n]
                });
            }
        }

        private void DrawSegment(List<Vector3> floorPolygon, string name)
        {
            var segmentGo = new GameObject(name ?? "");
            segmentGo.transform.parent = GameObject.transform;

            var mesh = segmentGo.AddComponent<ProBuilderMesh>();
            var result = mesh.CreateShapeFromPolygon(floorPolygon, BorderInfo.height, false);

            if (result.status != ActionResult.Status.Success)
            {
                Debug.LogWarning($"Could not create mesh for [{this.BorderInfo.name}] [{name}]: {result.notification}", GameObject);
                return;
            }

            mesh.SetMaterial(mesh.faces, BorderInfo.material);

            foreach (var face in mesh.faces)
            {
                face.uv = BorderInfo.sideUvUnwrapSettings;
            }

            mesh.ToMesh();
            mesh.Refresh();
        }

        private struct LinkedCorner
        {
            public Vector3 current;
            public Vector3 previous;
            public Vector3 next;
        }

        private Vector3 FindOuterPoint(LinkedCorner innerCorner, float extension, bool clockwise = true)
        {
            if (extension == 0)
            {
                return innerCorner.current;
            }

            var current = innerCorner.current;
            var previous = clockwise ? innerCorner.previous : innerCorner.next;
            var next = clockwise ? innerCorner.next : innerCorner.previous;

            var previousVector = current - previous;
            var nextVector = current - next;

            var toPreviousOuterLine = OrthogonalVectorCounterClockwise(current - previous).normalized * extension;
            var toNextOuterLine = OrthogonalVectorCounterClockwise(next - current).normalized * extension;

            var projectedInnerOnOuterPreviousLine = current + toPreviousOuterLine;
            var projectedInnerOnOuterNextLine = current + toNextOuterLine;

            try
            {
                return LineLineIntersection(projectedInnerOnOuterPreviousLine, previousVector, projectedInnerOnOuterNextLine, nextVector);
            }
            catch (InvalidOperationException)
            {
                // If no intersection exists, lines ar probably (almost) in same direction. In that case we simply return a known point on the outer line.
                return projectedInnerOnOuterPreviousLine;
            }
        }

        // Taken from https://stackoverflow.com/questions/59449628/check-when-two-vector3-lines-intersect-unity3d
        public static Vector3 LineLineIntersection(Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
        {

            Vector3 lineVec3 = linePoint2 - linePoint1;
            Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

            float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

            //is coplanar, and not parallel
            if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
            {
                float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
                return linePoint1 + (lineVec1 * s);
            }
            else
            {
                throw new InvalidOperationException("Lines do not intersect");
            }
        }

        private static Vector3 OrthogonalVectorCounterClockwise(Vector3 originalVector)
        {
            return new Vector3(originalVector.z * -1, originalVector.y, originalVector.x);
        }

        public void RemoveAllChildren()
        {
            if (Application.IsPlaying(GameObject))
            {
                foreach (Transform child in GameObject.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
            else
            {
                while (GameObject.transform.childCount > 0)
                {
                    GameObject.DestroyImmediate(GameObject.transform.GetChild(0).gameObject);
                }
            }
        }
    }
}