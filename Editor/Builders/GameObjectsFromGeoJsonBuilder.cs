using System.Collections.Generic;

using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using GeoJSON.Net.Feature;

namespace GeoJsonCityBuilder.Editor.Builders
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

        private void BuildFromFeatures()
        {
            int i = 1;
            foreach (Feature feature in this.m_features)
            {
                var go = AddFeature(feature, i++);
                ApplyBindings(go, feature);
            }
        }

        protected virtual GameObject AddFeature(Feature feature, int i)
        {
            var gameObject = this.Component.prefab ? GameObject.Instantiate(this.Component.prefab) : new GameObject();
            gameObject.name = this.Component.featureTypeFilter + i.ToString();
            gameObject.transform.parent = this.Component.transform;
            gameObject.transform.position = this.Component.transform.position;

            var featureComponent = gameObject.AddComponent<GeoJsonFeatureInstance>();
            featureComponent.Properties = new Dictionary<string, object>(feature.Properties);

            var existenceController = gameObject.AddComponent<ExistenceController>();
            existenceController.existencePeriodStart = feature.Properties.ContainsKey(this.Component.timeStartYearField) && feature.Properties[this.Component.timeStartYearField] != null ? (long)feature.Properties[this.Component.timeStartYearField] : -9999;
            existenceController.existencePeriodEnd = feature.Properties.ContainsKey(this.Component.timeEndYearField) && feature.Properties[this.Component.timeEndYearField] != null ? (long)feature.Properties[this.Component.timeEndYearField] : 9999;

            return gameObject;
        }

        private void ApplyBindings(GameObject gameObject, Feature feature)
        {
            foreach (var binding in this.Component.geoJsonBindings)
            {
                var component = gameObject.GetComponent(binding.componentName);
                var serializedObject = new SerializedObject(component);
                var property = serializedObject.FindProperty(binding.componentField);

                if (feature.Properties.ContainsKey(binding.geoJsonProperty) && feature.Properties[binding.geoJsonProperty] != null)
                {
                    property.boxedValue = feature.Properties[binding.geoJsonProperty];
                }
                else
                {
                    // property.stringValue = binding.defaultValue;
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}