//C# Example (LookAtPointEditor.cs)
using UnityEngine;
using UnityEditor;


namespace GeoJsonCityBuilder.Editor.Builders
{
    [CustomEditor(typeof(PrefabsFromGeoJson))]
    [CanEditMultipleObjects]
    public class PrefabsFromGeoJsonEditor : UnityEditor.Editor
    {
        SerializedProperty geoJsonFile;
        SerializedProperty featureTypeFilter;
        SerializedProperty worldPosition;
        SerializedProperty prefab;
        SerializedProperty timeStartYearField;
        SerializedProperty timeEndYearField;


        PrefabsFromGeoJsonBuilder builder;

        void OnEnable()
        {
            geoJsonFile = serializedObject.FindProperty("geoJsonFile");
            featureTypeFilter = serializedObject.FindProperty("featureTypeFilter");
            worldPosition = serializedObject.FindProperty("worldPosition");
            prefab = serializedObject.FindProperty("prefab");
            timeStartYearField = serializedObject.FindProperty("timeStartYearField");
            timeEndYearField = serializedObject.FindProperty("timeEndYearField");

            builder = new PrefabsFromGeoJsonBuilder(this.serializedObject.targetObject as PrefabsFromGeoJson);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(geoJsonFile);
            EditorGUILayout.PropertyField(worldPosition);
            EditorGUILayout.PropertyField(featureTypeFilter);
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