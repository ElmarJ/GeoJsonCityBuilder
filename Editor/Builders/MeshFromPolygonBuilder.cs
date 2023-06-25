using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace GeoJsonCityBuilder.Editor.Builders
{
    public abstract class MeshFromPolygonBuilder<T> where T:MeshFromPolygon
    {
        public T BuilderInfo { get; private set; }
        public GameObject GameObject { get; private set; }

        public MeshFromPolygonBuilder(T builderInfo)
        {
            this.BuilderInfo = builderInfo;
            this.GameObject = builderInfo.gameObject;
        }

        private ProBuilderMesh pb;

        public abstract void Draw();
    }
}