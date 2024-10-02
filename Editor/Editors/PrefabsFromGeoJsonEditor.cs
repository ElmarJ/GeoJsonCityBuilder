//C# Example (LookAtPointEditor.cs)
using GeoJsonCityBuilder.Components;
using GeoJsonCityBuilder.Editor.Builders;
using UnityEditor;
using UnityEngine;

namespace GeoJsonCityBuilder.Editor.Editors
{
    [CustomEditor(typeof(PrefabsFromGeoJson))]
    [CanEditMultipleObjects]
    public class PrefabsFromGeoJsonEditor : UnityEditor.Editor
    {
        private SerializedProperty geoJsonFile;
        private SerializedProperty excludeProperty;
        private SerializedProperty worldPosition;
        private SerializedProperty prefab;
        private SerializedProperty timeStartYearField;
        private SerializedProperty timeEndYearField;
        private PrefabsFromGeoJsonBuilder builder;

        private void OnEnable()
        {
            geoJsonFile = serializedObject.FindProperty("geoJsonFile");
            excludeProperty = serializedObject.FindProperty("excludeProperty");
            worldPosition = serializedObject.FindProperty("worldPosition");
            prefab = serializedObject.FindProperty("prefab");
            timeStartYearField = serializedObject.FindProperty("timeStartYearField");
            timeEndYearField = serializedObject.FindProperty("timeEndYearField");

            builder = new PrefabsFromGeoJsonBuilder(serializedObject.targetObject as PrefabsFromGeoJson);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(geoJsonFile);
            EditorGUILayout.PropertyField(worldPosition);
            EditorGUILayout.PropertyField(excludeProperty);
            EditorGUILayout.PropertyField(prefab);
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(timeStartYearField);
            EditorGUILayout.PropertyField(timeEndYearField);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear"))
            {
                builder.RemoveAllChildren();
            }
            if (GUILayout.Button("Generate"))
            {
                builder.Rebuild();
            }
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }
    }
}