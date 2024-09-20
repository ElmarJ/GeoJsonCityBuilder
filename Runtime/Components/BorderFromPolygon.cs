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
    public class BorderFromPolygon : MonoBehaviour
    {

        public List<Vector3> floorPolygon = new();
        public Material material;
        public float height = 1f;
        public float outerExtension = 1f;
        public float innerExtension = 1f;
        public AutoUnwrapSettings sideUvUnwrapSettings = new();
    }
}