using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace GeoJsonCityBuilder.Editor
{
    public class BorderFromPolygonBuilder
    {
        public BorderFromPolygon BorderInfo { get; private set; }
        public GameObject GameObject { get; private set; }

        public BorderFromPolygonBuilder(BorderFromPolygon borderInfo)
        {
            this.BorderInfo = borderInfo;
            this.GameObject = borderInfo.gameObject;
        }

        public void Draw()
        {
            this.RemoveAllChildren();

            var innerPolygon = BorderInfo.floorPolygon;
            var outerPolygon = this.GetOuterPolygon();

            var n = innerPolygon.Count;

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

                this.DrawSegment(segmentFloorPolygon);
            }
        }

        private void DrawSegment(List<Vector3> floorPolygon)
        {
            var segmentGo = new GameObject("Segment");
            segmentGo.transform.parent = this.GameObject.transform;

            var mesh = segmentGo.AddComponent<ProBuilderMesh>();
            mesh.CreateShapeFromPolygon(floorPolygon, this.BorderInfo.height, false);

            mesh.SetMaterial(mesh.faces, this.BorderInfo.material);

            foreach (var face in mesh.faces)
            {
                face.uv = this.BorderInfo.sideUvUnwrapSettings;
            }

            mesh.ToMesh();
            mesh.Refresh();
        }

        private List<Vector3> GetOuterPolygon()
        {
            // Better approaches here: https://stackoverflow.com/questions/1109536/an-algorithm-for-inflating-deflating-offsetting-buffering-polygons

            var innerPolygon = this.BorderInfo.floorPolygon;
            var n = innerPolygon.Count;
            var outerPolygon = new List<Vector3>();

            for (int i = 0; i < n; i++)
            {
                var i_previous = i > 0 ? i - 1 : n - 1;
                var i_next = (i + 1) % n;
                outerPolygon.Add(this.FindOuterPoint(innerPolygon[i], innerPolygon[i_previous], innerPolygon[i_next]));
            }
            return outerPolygon;
        }

        private Vector3 FindOuterPoint(Vector3 innerPoint, Vector3 previousInnerPoint, Vector3 nextInnerPoint)
        {
            return FindOuterPointWithVectorMethod(innerPoint, previousInnerPoint, nextInnerPoint);
        }

        private Vector3 FindOuterPointWithVectorMethod(Vector3 innerPoint, Vector3 previousInnerPoint, Vector3 nextInnerPoint)
        {
            var previousVector = innerPoint - previousInnerPoint;
            var nextVector = innerPoint - nextInnerPoint;

            var toPreviousOuterLine = OrthogonalVectorCounterClockwise(innerPoint - previousInnerPoint).normalized * BorderInfo.width;
            var toNextOuterLine = OrthogonalVectorCounterClockwise(nextInnerPoint - innerPoint).normalized * BorderInfo.width;

            var b = innerPoint + toPreviousOuterLine;
            var c = innerPoint + toNextOuterLine;

            var outerPoint = LineLineIntersection(b, previousVector, c, nextVector);
            Debug.LogFormat("Inner Point {0} ,Outer point: {1}, b: {2}, c {3}", innerPoint, outerPoint, b, c);

            return outerPoint;
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

        private Vector3 FindOuterPointWithGeometryMethod(Vector3 innerPoint, Vector3 previousInnerPoint, Vector3 nextInnerPoint)
        {
            var directionFromPrevious = (innerPoint - previousInnerPoint).normalized;
            var directionFromNext = (innerPoint - nextInnerPoint).normalized;

            // TODO: there is probably a more elegant way to do this, but for now this works :)
            //     (more or less - this does not result in correct width)
            var outerPointDirection = directionFromPrevious + directionFromNext / 2;
            var cornerAngle = Vector3.SignedAngle(directionFromPrevious, directionFromNext, Vector3.up);
            var outerPointExtension = cornerAngle < 0 ? BorderInfo.width : BorderInfo.width * -1;
            var outerPoint = innerPoint + outerPointDirection * outerPointExtension;
            return outerPoint;
        }

        private Vector3 FindOuterPointWithLinearEquationMethod(Vector3 innerPoint, Vector3 previousInnerPoint, Vector3 nextInnerPoint)
        {
            var b1 = (innerPoint.z - previousInnerPoint.z) / (innerPoint.x - previousInnerPoint.x);
            var b2 = (innerPoint.z - nextInnerPoint.z) / (innerPoint.x - nextInnerPoint.x);

            var angle = Vector3.SignedAngle(innerPoint - previousInnerPoint, innerPoint - nextInnerPoint, Vector3.down);

            var distance = BorderInfo.width;

            var a1 = distance * System.Math.Sqrt(1 + b1 * b1);
            var a2 = distance * -1 * System.Math.Sqrt(1 + b2 * b2);

            var x_intersection = (a2 - a1) / (b1 - b2);
            var z_intersection = (b1 * x_intersection) + a1;

            var outerVector = new Vector3((float)x_intersection, 0, (float)z_intersection);
            var outerPoint = innerPoint + outerVector;

            Debug.LogFormat("Inner Point {0} ,Outer point: {1}, b1: {2}, b2: {3}, angle: {4}", innerPoint, outerPoint, b1, b2, angle);
            return outerPoint;
        }

        public void RemoveAllChildren()
        {
            if (Application.IsPlaying(this.GameObject))
            {
                foreach (Transform child in this.GameObject.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
            else
            {
                while (this.GameObject.transform.childCount > 0)
                {
                    GameObject.DestroyImmediate(this.GameObject.transform.GetChild(0).gameObject);
                }
            }
        }
    }
}