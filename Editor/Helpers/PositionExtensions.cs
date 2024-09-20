using GeoJSON.Net.Geometry;
using GeoJsonCityBuilder.Data;

namespace GeoJsonCityBuilder.Editor.Helpers
{
    public static class PositionExtensions
    {
        public static Coordinate ToCoordinate(this IPosition position)
        {
            return new Coordinate((float)position.Longitude, (float)position.Latitude);
        }
    }
}