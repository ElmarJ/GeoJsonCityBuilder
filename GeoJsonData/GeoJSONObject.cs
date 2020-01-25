using System.Collections.Generic;
using Data.GeoJSON;

public class GeoJSONObject
{
    public FeatureCollection FeatureCollection { get; private set; }

    public GeoJSONObject(JSONObject jsonObject)
    {
        this.FeatureCollection = DeserializeGeoJSON(jsonObject);
    }

    public GeoJSONObject(string json)
        : this(new JSONObject(json)) {
        }

    private static FeatureCollection DeserializeGeoJSON(JSONObject jsonFeatureCollection)
    {
        var featureCollection = new FeatureCollection();
        featureCollection.Type = jsonFeatureCollection["type"].ToString();

        foreach (var jsonFeature in jsonFeatureCollection["features"].list)
        {
            var feature = new Feature();

            feature.Type = jsonFeature["type"].str;
            feature.Properties.Type = jsonFeature["properties"]["type"]?.str;

            var geometryType = jsonFeature["geometry"]["type"];
            

            if (geometryType.str == "Polygon")
            {
                var geometry = new PolygonGeometry();

                foreach (var jsonCoordinateList in jsonFeature["geometry"]["coordinates"].list)
                {
                    var coordinateList = new List<Coordinate>();
                    foreach (var jsonCoordinate in jsonCoordinateList.list)
                    {
                        var coordinate = new Coordinate(
                            jsonCoordinate[0].f,
                            jsonCoordinate[1].f
                        );
                        coordinateList.Add(coordinate);
                    }
                    geometry.Coordinates.Add(coordinateList);
                }

                feature.Geometry = geometry;
            }
            if (geometryType.str == "Point")
            {
                var geometry = new PointGeometry();

                geometry.Coordinate = new Coordinate(
                    jsonFeature["geometry"]["coordinates"][0].f,
                    jsonFeature["geometry"]["coordinates"][1].f
                );

                feature.Geometry = geometry;
            }

            featureCollection.Features.Add(feature);            
        }

        return featureCollection;
    }
}