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
    public class BorderFromPolygon : MeshFromPolygon
    {
        public Material material;
        public float height = 1f;
        public float outerExtension = 1f;
        public float innerExtension = 1f;
    }
}