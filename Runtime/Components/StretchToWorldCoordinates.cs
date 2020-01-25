using GeoJsonCityBuilder.Data.GeoJSON;
using UnityEngine;

namespace GeoJsonCityBuilder
{

    // [ExecuteInEditMode]
    [RequireComponent(typeof(PositionOnWorldCoordinates))]
    [RequireComponent(typeof(StretchToPoint))]
    public class StretchToWorldCoordinates : MonoBehaviour
    {
        PositionOnWorldCoordinates parentPositionComponent;
        StretchToPoint stretchComponent;

        public float lat;
        public float lon;
        public Coordinate EndCoordinate
        {
            get => new Coordinate(lon, lat);
            set
            {
                this.lat = value.Lat;
                this.lon = value.Lon;
            }
        }

        void Awake()
        {
            parentPositionComponent = transform.parent?.gameObject.GetComponent<PositionOnWorldCoordinates>();
            stretchComponent = gameObject.GetComponent<StretchToPoint>();
        }

        void Start()
        {
            Recalculate();
        }

        public void Recalculate()
        {
            if (this.parentPositionComponent != null)
            {
                stretchComponent.endPoint = this.EndCoordinate.ToLocalGrid(this.parentPositionComponent.Origin);
                stretchComponent.PerformStretch();
            }
        }
    }
}