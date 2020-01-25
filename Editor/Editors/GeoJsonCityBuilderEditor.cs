//C# Example (LookAtPointEditor.cs)
using UnityEngine;
using UnityEditor;
using GeoJsonCityBuilder;


namespace GeoJsonCityBuilder.Editor
{

    [CustomEditor(typeof(GeoJsonCityBuilder))]
    [CanEditMultipleObjects]
    public class GeoJsonCityBuilderEditor : UnityEditor.Editor
    {
        SerializedProperty geoJsonFile;
        SerializedProperty wallMaterial;
        SerializedProperty treePrefab;

        void OnEnable()
        {
            geoJsonFile = serializedObject.FindProperty("geoJsonFile");
            wallMaterial = serializedObject.FindProperty("wallMaterial");
            treePrefab = serializedObject.FindProperty("treePrefab");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(geoJsonFile);
            EditorGUILayout.PropertyField(wallMaterial);
            EditorGUILayout.PropertyField(treePrefab);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear"))
            {
                var controller = this.serializedObject.targetObject as GeoJsonCityBuilder;

                controller.RemoveAllChildren();
            }
            if (GUILayout.Button("Generate"))
            {
                var controller = this.serializedObject.targetObject as GeoJsonCityBuilder;

                controller.Rebuild();
            }
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }
    }
}