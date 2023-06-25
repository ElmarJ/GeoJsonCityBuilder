//C# Example (LookAtPointEditor.cs)
using UnityEngine;
using UnityEditor;


namespace GeoJsonCityBuilder.Editor.Editors
{

    [CustomEditor(typeof(GeoJsonFeatureInstance))]
    [CanEditMultipleObjects]
    public class GeoJsonFeatureInstanceEditor : UnityEditor.Editor
    {
        bool showCustomFeatures = false;

        void OnEnable()
        {
        }

        public override void OnInspectorGUI() 
        {
            var featureComponent = this.serializedObject.targetObject as GeoJsonFeatureInstance;
            {
                if (featureComponent.Properties != null)
                {
                    showCustomFeatures = EditorGUILayout.BeginFoldoutHeaderGroup(showCustomFeatures, "Properties");
                    if (showCustomFeatures)
                    {
                        foreach (var property in featureComponent.Properties)
                        {
                            EditorGUILayout.LabelField(string.Format("{0}: {1}", property.Key, property.Value));
                        }
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                }
            }
        }
    }
}