using System;
using UnityEngine;

namespace GeoJsonCityBuilder.Data {
    [Serializable]
    public class Coordinate
    {
        public float Lon;
        public float Lat;

        public Coordinate(float lon, float lat) {
            this.Lon = lon;
            this.Lat = lat;
        }

        public Vector2 ToLocalGrid(Coordinate gridOrigin) => MeterVectorFromCoordinates(gridOrigin, this);

        // Converts Degrees to Radians
        static double Rad(double degrees) => degrees * Math.PI / 180;

        public static double MeterDistanceFromCoordinates(Coordinate coordinate1, Coordinate coordinate2) {
            const int R = 6371000; // km - earth diameter
            
            var dLat = Rad(coordinate2.Lat - coordinate1.Lat);
            var dLon = Rad(coordinate2.Lon - coordinate1.Lon);             

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(Rad(coordinate1.Lat)) * Math.Cos(Rad(coordinate2.Lat)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2); 
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a)); 
            var d = R * c;

            return d;
        }

        public static Vector2 MeterVectorFromCoordinates(Coordinate coordinate1, Coordinate coordinate2) {
            const int R = 6371000; // km - earth diameter
            
            var dLon = Rad(coordinate2.Lon - coordinate1.Lon);             
            var dLat = Rad(coordinate2.Lat - coordinate1.Lat);

            var aLon = Math.Pow(Math.Cos(Rad(coordinate1.Lat)), 2) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2); 
            var cLon = 2 * Math.Atan2(Math.Sqrt(aLon), Math.Sqrt(1 - aLon)); 
            var metersLon = R * cLon;

            var aLat = Math.Sin(dLat / 2) * Math.Sin(dLat / 2); 
            var cLat = 2 * Math.Atan2(Math.Sqrt(aLat), Math.Sqrt(1 - aLat)); 
            var metersLat = R * cLat;

            if (coordinate1.Lat > coordinate2.Lat) {
                metersLat = metersLat * -1;
            }

            if (coordinate1.Lon > coordinate2.Lon) {
                metersLon = metersLon * -1;
            }

            return new Vector2((float)metersLon, (float)metersLat);
        }
    }
}