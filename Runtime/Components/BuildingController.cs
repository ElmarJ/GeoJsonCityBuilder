using System.Collections.Generic;
using System.Linq;
using Data.GeoJSON;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class BuildingController : MonoBehaviour
{
    public IList<Vector3> floor;

    public Material wallMaterial;
    public float height = 10f;
    private void Start()
    {
    }

    public void DrawBuilding()
    {
        var mesh = gameObject.AddComponent<ProBuilderMesh>();
        if (gameObject.GetComponent<MeshCollider>() == null)
        {
            gameObject.AddComponent<MeshCollider>();
        }

        var first = floor.First();
        var last = floor.Last();
        if (first.x == last.x && first.y == last.y && first.z == last.z)
        {
            floor.Remove(floor.Last());
        }

        mesh.CreateShapeFromPolygon(floor, height, false);
        gameObject.GetComponent<MeshRenderer>().material = this.wallMaterial;

    }
}