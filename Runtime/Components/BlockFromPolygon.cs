using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace GeoJsonCityBuilder.Components
{
    // Require removed, because it caused a Unity build-error
    //   with HDRP (build apparently tries to remove all
    //    components without checking component-dependencies)
    // [RequireComponent(typeof(ProBuilderMesh))]
    [RequireComponent(typeof(MeshCollider))]
    public class BlockFromPolygon : MonoBehaviour
    {
        public List<Vector3> floorPolygon = new();
        public Material topMaterial;
        public Material bottomMaterial;
        public Material sideMaterial;
        public float height = 10f;
        public bool pointedRoof = false;
        public float pointedRoofHeight = 3f;
        public bool raiseFrontAndBackFacadeTop = false;
        public float leanForward = 0f;
        public AutoUnwrapSettings sideUvUnwrapSettings = new();
    }
}