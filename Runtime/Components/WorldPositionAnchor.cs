using System.Collections;
using System.Collections.Generic;
using GeoJsonCityBuilder.Data;
using UnityEngine;

namespace GeoJsonCityBuilder
{
    public class WorldPositionAnchor : MonoBehaviour
    {
        public Coordinate Origin;

        public Coordinate Coordinates
        {
            get
            {
                return Origin.LocalGridTransform(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.z));
            }
            set
            {
                // When setting the coordinate, replace the origin of the coordinate system
                //    with the implied origin (i.e. the coordinates of the current Unity origin)
                //    which can be found by walking backwards from our current position to origin.
                var pos = this.gameObject.transform.position;
                this.Origin = value.LocalGridTransform(new Vector2(-1 * pos.x, -1 * pos.z));
            }
        }
    }
}