using System;
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
    public class MeshFromPolygon : MonoBehaviour
    {
        public List<Vector3> polygon = new List<Vector3>();
        public AutoUnwrapSettings sideUvUnwrapSettings = new AutoUnwrapSettings();
    }
}