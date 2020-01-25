using System.Collections;
using System.Collections.Generic;
using GeoJsonCityBuilder.Data.GeoJSON;
using UnityEngine;

namespace GeoJsonCityBuilder
{

    // [ExecuteInEditMode]
    public class PositionOnWorldCoordinates : MonoBehaviour
    {
        // 52.367
        // 4.905

        public Coordinate Origin;
        private PositionOnWorldCoordinates parentComponent;


        void Awake()
        {
        }

        // Start is called before the first frame update
        void Start()
        {
            this.Recalculate();
        }

        public void Recalculate()
        {
            var parent = gameObject.transform.parent;
            if (parent == null) return;

            var parentComponent = parent.gameObject.GetComponent<PositionOnWorldCoordinates>();

            // Only continue if our parent also has this component (and thus global coordinates)
            // else we are the root ourselves, and don't need to move as  we _define_ the origin
            if (parentComponent == null) return;

            var planeLocation = Coordinate.MeterVectorFromCoordinates(parentComponent.Origin, this.Origin);
            var height = this.gameObject.transform.position.y;

            this.gameObject.transform.localPosition = new Vector3(planeLocation.x, height, planeLocation.y);
        }
    }
}