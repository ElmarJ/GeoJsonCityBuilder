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

        void OnEnable()
        {
            geoJsonFile = serializedObject.FindProperty("geoJsonFile");
            featureTypeFilter = serializedObject.FindProperty("featureTypeFilter");
            prefab = serializedObject.FindProperty("prefab");
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
                var controller = this.serializedObject.targetObject as PrefabsFromGeoJson;

                controller.RemoveAllChildren();
            }
            if (GUILayout.Button("Generate"))
            {
                var controller = this.serializedObject.targetObject as PrefabsFromGeoJson;

                controller.Rebuild();
            }
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }
    }
}