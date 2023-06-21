using UnityEngine;
using UnityEditor;

namespace GeoJsonCityBuilder.Editor
{
    [CustomEditor(typeof(BordersFromGeoJson))]
    [CanEditMultipleObjects]
    public class BordersFromGeoJsonEditor : UnityEditor.Editor
    {
        SerializedProperty geoJsonFile;
        SerializedProperty worldPosition;
        SerializedProperty height;
        SerializedProperty outerExtension;
        SerializedProperty innerExtension;
        SerializedProperty material;
        BordersFromGeoJsonBuilder builder;

        void OnEnable()
        {
            geoJsonFile = serializedObject.FindProperty("geoJsonFile");
            worldPosition = serializedObject.FindProperty("worldPosition");
            height = serializedObject.FindProperty("height");
            outerExtension = serializedObject.FindProperty("outerExtension");
            innerExtension = serializedObject.FindProperty("innerExtension");
            material = serializedObject.FindProperty("material");

            builder = new BordersFromGeoJsonBuilder(this.TargetObject);
        }

        public BordersFromGeoJson TargetObject { get => this.serializedObject.targetObject as BordersFromGeoJson; }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(geoJsonFile);
            EditorGUILayout.PropertyField(worldPosition);
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(height);
            EditorGUILayout.PropertyField(outerExtension);
            EditorGUILayout.PropertyField(innerExtension);
            EditorGUILayout.PropertyField(material);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear"))
            {
                this.builder.RemoveAllChildren();
            }
            if (GUILayout.Button("Generate"))
            {
                this.builder.Rebuild();
                
            }
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }
    }
}