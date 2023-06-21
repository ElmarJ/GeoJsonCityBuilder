using System.Collections.Generic;

using System.Linq;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using GeoJsonCityBuilder.Data;
using UnityEditor;
using UnityEngine;
using GeoJSON.Net.Feature;

namespace GeoJsonCityBuilder.Editor
{
    public abstract class GameObjectsFromGeoJsonBuilder<T> where T: GameObjectsFromGeoJson
    {
        public T Component
        {
            get;
            private set;
        }

        protected List<Feature> m_features;

        public GameObjectsFromGeoJsonBuilder(T component)
        {
            this.Component = component;
        }

        private void DeserializeGeoJson()
        {
            var geoJSON = JsonConvert.DeserializeObject<FeatureCollection>(this.Component.geoJsonFile.text);
            var filteredFeatures =
                from feature in geoJSON.Features
                where this.Component.featureTypeFilter == "" || (feature.Properties.ContainsKey("type")
                                                                 && feature.Properties["type"] != null
                                                                 && feature.Properties["type"].ToString() == this.Component.featureTypeFilter)
                select feature as Feature;
            this.m_features = filteredFeatures.ToList();
        }

        public void RemoveAllChildren()
        {
            if (Application.IsPlaying(this.Component.gameObject))
            {
                foreach (Transform child in this.Component.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
            else
            {
                while (this.Component.transform.childCount > 0)
                {
                    GameObject.DestroyImmediate(this.Component.transform.GetChild(0).gameObject);
                }
            }
        }

        public void Rebuild()
        {
            this.RemoveAllChildren();
            this.DeserializeGeoJson();
            this.BuildFromFeatures();
        }

        protected abstract void BuildFromFeatures();
    }
}