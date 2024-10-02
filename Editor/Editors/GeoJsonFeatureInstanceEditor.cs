//C# Example (LookAtPointEditor.cs)
using GeoJsonCityBuilder.Components;
using UnityEditor;
using System.Linq;

namespace GeoJsonCityBuilder.Editor.Editors
{

    [CustomEditor(typeof(GeoJsonFeatureInstance))]
    [CanEditMultipleObjects]
    public class GeoJsonFeatureInstanceEditor : UnityEditor.Editor
    {
        private bool showCustomFeatures = false;

        public override void OnInspectorGUI()
        {
            var featureComponent = serializedObject.targetObject as GeoJsonFeatureInstance;
            {
                if (featureComponent.Properties != null)
                {
                    showCustomFeatures = EditorGUILayout.BeginFoldoutHeaderGroup(showCustomFeatures, "Properties");
                    if (showCustomFeatures)
                    {
                        foreach (var property in from property in featureComponent.Properties
                                                 where property.Value != null
                                                 select property)
                        {
                            EditorGUILayout.LabelField(property.Key, property.Value switch { string s => $"\"{s}\"", null => "", object o => o.ToString() });
                        }
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                }
            }
        }
    }
}