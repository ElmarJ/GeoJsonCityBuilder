//C# Example (LookAtPointEditor.cs)
using UnityEngine;
using UnityEditor;


namespace GeoJsonCityBuilder.Editor
{
    [CustomEditor(typeof(PrefabsFromGeoJson))]
    [CanEditMultipleObjects]
    public class PrefabsFromGeoJsonEditor : UnityEditor.Editor
    {
        SerializedProperty geoJsonFile;
        SerializedProperty featureTypeFilter;
        SerializedProperty prefab;

        PrefabsFromGeoJsonBuilder builder;

        void OnEnable()
        {
            geoJsonFile = serializedObject.FindProperty("geoJsonFile");
            featureTypeFilter = serializedObject.FindProperty("featureTypeFilter");
            prefab = serializedObject.FindProperty("prefab");

            builder = new PrefabsFromGeoJsonBuilder(this.serializedObject.targetObject as PrefabsFromGeoJson);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(geoJsonFile);
            EditorGUILayout.PropertyField(featureTypeFilter);
            EditorGUILayout.PropertyField(prefab);
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