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
            var innerPolygon = this.BorderInfo.floorPolygon;
            var n = innerPolygon.Count;
            var outerPolygon = new List<Vector3>();

            for (int i = 0; i < n; i++)
            {
                var i_previous = i > 0 ? i - 1 : n - 1;
                var i_next = (i + 1) % n;
                outerPolygon.Add(this.GetOuterPoint(innerPolygon[i], innerPolygon[i_previous], innerPolygon[i_next]));
            }
            return outerPolygon;
        }

        private Vector3 GetOuterPoint(Vector3 innerPoint, Vector3 previousInnerPoint, Vector3 nextInnerPoint)
        {
            var directionFromPrevious = (innerPoint - previousInnerPoint).normalized;
            var directionFromNext = (innerPoint - nextInnerPoint).normalized;

            // TODO: there is probably a more elegant way to do this, but for now this works :)
            var outerPointDirection = directionFromPrevious + directionFromNext / 2;
            var cornerAngle = Vector3.SignedAngle(directionFromPrevious, directionFromNext, Vector3.up);
            var outerPointExtension = cornerAngle < 0 ? BorderInfo.width : BorderInfo.width * -1;
            var outerPoint = innerPoint + outerPointDirection * outerPointExtension;
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