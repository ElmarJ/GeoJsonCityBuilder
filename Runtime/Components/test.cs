using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace GeoJsonCityBuilder
{

    public class Encadrement : MonoBehaviour
    {
        public float largeur = 0.2f;
        public float epaisseur = 0.02f;
        public float profondeur = 0.1f;

        void Start()
        {
            ProBuilderMesh pb = GetComponent<ProBuilderMesh>();

            // 1 - backface extrusion
            pb.Extrude(new Face[] { pb.faces[0] }, ExtrudeMethod.IndividualFaces, 0.025f);
            pb.ToMesh();
            pb.Refresh();

            // 2 - borders extrusion
            pb.Extrude(new Face[] { pb.faces[6], pb.faces[7], pb.faces[8], pb.faces[9] }, ExtrudeMethod.IndividualFaces, largeur);
            pb.ToMesh();
            pb.Refresh();

            // 3 - borders 2nd extrusion
            pb.Extrude(new Face[] { pb.faces[6], pb.faces[7], pb.faces[8], pb.faces[9] }, ExtrudeMethod.IndividualFaces, epaisseur);
            pb.ToMesh();
            pb.Refresh();

            // 4 - sides extrusion towards front
            pb.Extrude(new Face[] { pb.faces[18], pb.faces[20], pb.faces[22], pb.faces[24] }, ExtrudeMethod.IndividualFaces, profondeur - 0.025f);
            pb.ToMesh();
            pb.Refresh();

        }
    }
}