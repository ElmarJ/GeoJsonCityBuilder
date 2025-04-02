using System.Collections.Generic;
using System.Linq;
using GeoJSON.Net;
using GeoJSON.Net.Feature;
using GeoJsonCityBuilder.Components;
using Newtonsoft.Json;
using UnityEngine;

namespace GeoJsonCityBuilder.Editor.Builders
{
    public abstract class FeatureCollectionBuilderBase<T> where T : GeneratorComponentBase
    {
        protected T Component { get; private set; }
        protected List<Feature> m_features;

        protected FeatureCollectionBuilderBase(T component)
        {
            Component = component;
        }
        protected void DeserializeGeoJson(GeoJSONObjectType typeFilter)
        {
            if (Component.geoJsonFile == null)
            {
                Debug.LogError($"GeoJson file not set on {Component.gameObject.name}");
                return;
            }
            var geoJSON = JsonConvert.DeserializeObject<FeatureCollection>(Component.geoJsonFile.text);

            var filteredFeatures =
                from feature in geoJSON.Features
                where feature.Geometry.Type == typeFilter
                    && (Component.excludeProperty is null or "" || !feature.Properties.ContainsKey(Component.excludeProperty) || !(bool)feature.Properties[Component.excludeProperty])
                select feature;

            m_features = filteredFeatures.ToList();
        }

        public void RemoveAllChildren()
        {
            if (Application.IsPlaying(Component.gameObject))
            {
                foreach (Transform child in Component.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
            else
            {
                for (int i = Component.transform.childCount - 1; i >= 0; i--)
                {
                    GameObject.DestroyImmediate(Component.transform.GetChild(0).gameObject);
                }

                if (Component.transform.childCount > 0)
                {
                    Debug.LogError("Failed to remove all children", Component.gameObject);
                }
            }
        }

        public abstract void Rebuild(int seed = 42);

        protected static int? GetYearFromField(Feature feature, string field)
        {
            object value = feature.Properties.ContainsKey(field) ? feature.Properties[field] : null;
            int? year = value switch
            {
                string text => text.Length >= 4 && int.TryParse(text[..4], out int number) ? number : null,
                long y => (int)y,
                int y => y,
                _ => null
            };

            return year;
        }

    }
}