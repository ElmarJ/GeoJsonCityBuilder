using System.Collections.Generic;
using Defective.JSON;
using System.Linq;

namespace GeoJsonCityBuilder.Data.GeoJSON
{
    public class GeoJSONObject
    {
        public FeatureCollection FeatureCollection { get; private set; }

        public GeoJSONObject(JSONObject jsonObject)
        {
            this.FeatureCollection = DeserializeGeoJSON(jsonObject);
        }

        public GeoJSONObject(string json)
            : this(new JSONObject(json))
        {
        }

        private static FeatureCollection DeserializeGeoJSON(JSONObject jsonFeatureCollection)
        {
            var featureCollection = new FeatureCollection
            {
                Type = jsonFeatureCollection["type"].ToString()
            };

            foreach (var jsonFeature in jsonFeatureCollection["features"].list)
            {
                var feature = new Feature
                {
                    Type = jsonFeature["type"].stringValue
                };
                feature.Properties.Type = jsonFeature["properties"]["type"]?.stringValue;
                feature.Properties.Height = jsonFeature["properties"]["height"]?.floatValue;
                feature.Properties.ExistencePeriodStartYear = jsonFeature["properties"]["exist_period_start"]?.intValue;
                feature.Properties.ExistencePeriodEndYear = jsonFeature["properties"]["exist_period_end"]?.intValue;

                var geometryType = jsonFeature["geometry"]["type"];

                feature.Geometry = geometryType.stringValue switch
                {
                    "Polygon" => ParsePolygonGeometry(jsonFeature["geometry"]["coordinates"].list),
                    "MultiPolygon" => ParseMultiPolygonGeometry(jsonFeature["geometry"]["coordinates"].list),
                    "Point" => new PointGeometry{Coordinate = new Coordinate(jsonFeature["geometry"]["coordinates"][0].floatValue, jsonFeature["geometry"]["coordinates"][1].floatValue)},
                    _ => null,
                };

                featureCollection.Features.Add(feature);
            }

            return featureCollection;
        }

        private static PolygonGeometry ParsePolygonGeometry(List<JSONObject> jsonPolygons)
        {
            var geometry = new PolygonGeometry();
            geometry.Coordinates.AddRange(
                from jsonPolygon in jsonPolygons
                select (
                    from jsonCoordinate in jsonPolygon.list
                    select new Coordinate(jsonCoordinate[0].floatValue, jsonCoordinate[1].floatValue))
                    .ToList()
                );
            return geometry;
        }

        private static MultiPolygonGeometry ParseMultiPolygonGeometry(List<JSONObject> jsonCoordinates)
        {
            var geometry = new MultiPolygonGeometry();
            geometry.Geometries.AddRange(
                from polygonCoordinates in jsonCoordinates
                select ParsePolygonGeometry(polygonCoordinates.list));
            return geometry;
        }
    }
}