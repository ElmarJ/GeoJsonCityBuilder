using GeoJsonCityBuilder.Components;
using GeoJsonCityBuilder.Editor.Builders;
using UnityEditor;
using UnityEngine;

namespace GeoJsonCityBuilder.Editor.Editors
{
    [CustomEditor(typeof(BordersFromGeoJson))]
    [CanEditMultipleObjects]
    public class BordersFromGeoJsonEditor : UnityEditor.Editor
    {
        private SerializedProperty geoJsonFile;
        private SerializedProperty excludeProperty;
        private SerializedProperty worldPositionAnchor;
        private SerializedProperty height;
        private SerializedProperty outerExtension;
        private SerializedProperty innerExtension;
        private SerializedProperty material;
        private SerializedProperty timeStartYearField;
        private SerializedProperty timeEndYearField;
        private BordersFromGeoJsonBuilder builder;

        private void OnEnable()
        {
            geoJsonFile = serializedObject.FindProperty("geoJsonFile");
            excludeProperty = serializedObject.FindProperty("excludeProperty");
            worldPositionAnchor = serializedObject.FindProperty("worldPositionAnchor");
            height = serializedObject.FindProperty("height");
            outerExtension = serializedObject.FindProperty("outerExtension");
            innerExtension = serializedObject.FindProperty("innerExtension");
            material = serializedObject.FindProperty("material");
            timeStartYearField = serializedObject.FindProperty("timeStartYearField");
            timeEndYearField = serializedObject.FindProperty("timeEndYearField");

            builder = new BordersFromGeoJsonBuilder(TargetObject);
        }

        public BordersFromGeoJson TargetObject { get => serializedObject.targetObject as BordersFromGeoJson; }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(geoJsonFile);
            EditorGUILayout.PropertyField(worldPositionAnchor);
            EditorGUILayout.PropertyField(excludeProperty);
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(height);
            EditorGUILayout.PropertyField(outerExtension);
            EditorGUILayout.PropertyField(innerExtension);
            EditorGUILayout.PropertyField(material);
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