//C# Example (LookAtPointEditor.cs)
using GeoJsonCityBuilder.Components;
using GeoJsonCityBuilder.Editor.Builders;
using UnityEditor;
using UnityEngine;

namespace GeoJsonCityBuilder.Editor.Editors
{

    [CustomEditor(typeof(BlocksFromGeoJson))]
    [CanEditMultipleObjects]
    public class BlocksFromGeoJsonEditor : UnityEditor.Editor
    {
        private SerializedProperty geoJsonFile;
        private SerializedProperty worldPositionAnchor;
        private SerializedProperty basePrefab;
        private SerializedProperty featureTypeFilter;
        private SerializedProperty heightMin;
        private SerializedProperty heightMax;
        private SerializedProperty topMaterial;
        private SerializedProperty sideMaterials;
        private SerializedProperty sideUvUnwrapSettings;
        private SerializedProperty bottomMaterial;
        private SerializedProperty pointedRoofTops;
        private SerializedProperty raiseFacades;
        private SerializedProperty timeStartYearField;
        private SerializedProperty timeEndYearField;
        private BlocksFromGeoJsonBuilder builder;

        private void OnEnable()
        {
            geoJsonFile = serializedObject.FindProperty("geoJsonFile");
            worldPositionAnchor = serializedObject.FindProperty("worldPositionAnchor");
            basePrefab = serializedObject.FindProperty("basePrefab");
            featureTypeFilter = serializedObject.FindProperty("featureTypeFilter");
            heightMin = serializedObject.FindProperty("heightMin");
            heightMax = serializedObject.FindProperty("heightMax");
            topMaterial = serializedObject.FindProperty("topMaterial");
            sideMaterials = serializedObject.FindProperty("sideMaterials");
            sideUvUnwrapSettings = serializedObject.FindProperty("sideUvUnwrapSettings");
            bottomMaterial = serializedObject.FindProperty("bottomMaterial");
            pointedRoofTops = serializedObject.FindProperty("pointedRoofTops");
            raiseFacades = serializedObject.FindProperty("raiseFacades");
            timeStartYearField = serializedObject.FindProperty("timeStartYearField");
            timeEndYearField = serializedObject.FindProperty("timeEndYearField");

            builder = new BlocksFromGeoJsonBuilder(TargetObject);
        }

        public BlocksFromGeoJson TargetObject => serializedObject.targetObject as BlocksFromGeoJson;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(geoJsonFile);
            EditorGUILayout.PropertyField(worldPositionAnchor);
            EditorGUILayout.PropertyField(basePrefab);
            EditorGUILayout.PropertyField(featureTypeFilter);
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(heightMin);
            EditorGUILayout.PropertyField(heightMax);
            EditorGUILayout.PropertyField(pointedRoofTops);
            EditorGUILayout.PropertyField(raiseFacades);
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(sideMaterials, true);
            EditorGUILayout.PropertyField(sideUvUnwrapSettings);
            EditorGUILayout.PropertyField(bottomMaterial);
            EditorGUILayout.PropertyField(topMaterial);
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