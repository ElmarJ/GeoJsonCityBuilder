using System;
using UnityEngine;

namespace GeoJsonCityBuilder.Data
{
    [Serializable]
    public struct Coordinate
    {
        // earth radius in meters:
        const int R = 6371000;

        public float Lon;
        public float Lat;

        public Coordinate(float lon, float lat)
        {
            this.Lon = lon;
            this.Lat = lat;
        }

        public Vector2 ToLocalGrid(Coordinate gridOrigin) => MeterVectorFromCoordinates(gridOrigin, this);
        public Coordinate LocalGridTransform(Vector2 transformVector) => CoordinateFromMeterVector(this, transformVector);
        
        public Vector3 ToLocalPosition(Coordinate gridOrigin, float height)
        {
            var planePosition = ToLocalGrid(gridOrigin);
            return new Vector3(planePosition.x, height, planePosition.y);
        }

        // Converts Degrees to Radians
        static double Rad(double degrees) => degrees * Math.PI / 180;
        static double Deg(double rad) => rad * 180 / Math.PI;

        public static double MeterDistanceFromCoordinates(Coordinate coordinate1, Coordinate coordinate2)
        {
            var dLat = Rad(coordinate2.Lat - coordinate1.Lat);
            var dLon = Rad(coordinate2.Lon - coordinate1.Lon);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(Rad(coordinate1.Lat)) * Math.Cos(Rad(coordinate2.Lat)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c;

            return d;
        }

        public static Vector2 MeterVectorFromCoordinates(Coordinate coordinate1, Coordinate coordinate2)
        {
            var dLon = Rad(coordinate2.Lon - coordinate1.Lon);
            var dLat = Rad(coordinate2.Lat - coordinate1.Lat);

            var aLon = Math.Pow(Math.Cos(Rad(coordinate1.Lat)), 2) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var cLon = 2 * Math.Atan2(Math.Sqrt(aLon), Math.Sqrt(1 - aLon));
            var metersLon = R * cLon;

            var aLat = Math.Sin(dLat / 2) * Math.Sin(dLat / 2);
            var cLat = 2 * Math.Atan2(Math.Sqrt(aLat), Math.Sqrt(1 - aLat));
            var metersLat = R * cLat;

            if (coordinate1.Lat > coordinate2.Lat)
            {
                metersLat *= -1;
            }

            if (coordinate1.Lon > coordinate2.Lon)
            {
                metersLon *= -1;
            }

            return new Vector2((float)metersLon, (float)metersLat);
        }

        public static Coordinate CoordinateFromMeterVector(Coordinate origin, Vector2 meterVector)
        {
            // Inverse function of MeterVectorFromCoordinates()
            var metersLon = meterVector.x;
            var metersLat = meterVector.y;

            var cLon = metersLon / R;
            var aLon = Math.Tan(Math.Pow(cLon / 2, 2) / Math.Pow(1 + (cLon / 2), 2));

            var cLat = metersLat / R;
            var aLat = Math.Tan(Math.Pow(cLat / 2, 2) / Math.Pow(1 + (cLat / 2), 2));

            var dLat = (2 * Math.Asin(Math.Sqrt(aLat)));
            if (meterVector.x > 0)
            {
                dLat *= -1;
            }
            var lat = origin.Lat - Deg(dLat);

            var dLon = Math.Asin(Math.Sqrt(aLon / Math.Pow(Math.Cos(Rad(lat)), 2))) * 2;
            if (meterVector.y > 0)
            {
                dLon *= -1;
            }
            var lon = origin.Lon - Deg(dLon);
            
            return new Coordinate((float)lon, (float)lat);
        }

        public override string ToString()
        {
            return this.Lat.ToString() + " N, " + this.Lon.ToString() + " W";
        }
    }
}