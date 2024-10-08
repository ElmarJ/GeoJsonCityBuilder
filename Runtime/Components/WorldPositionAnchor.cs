﻿using GeoJsonCityBuilder.Data;
using UnityEngine;

namespace GeoJsonCityBuilder.Components
{
    public class WorldPositionAnchor : MonoBehaviour
    {
        public Coordinate SceneOrigin;

        public Coordinate Coordinates
        {
            get
            {
                return SceneOrigin.LocalGridTransform(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.z));
            }
            set
            {
                // When setting the coordinate, replace the origin of the coordinate system
                //    with the implied origin (i.e. the coordinates of the current Unity origin)
                //    which can be found by walking backwards from our current position to origin.
                var pos = this.gameObject.transform.position;
                this.SceneOrigin = value.LocalGridTransform(new Vector2(-1 * pos.x, -1 * pos.z));
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(transform.position, 1);
            Gizmos.DrawIcon(transform.position, "BuildSettings.Web", true);
        }
    }
}