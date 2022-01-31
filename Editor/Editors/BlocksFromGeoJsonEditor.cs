//C# Example (LookAtPointEditor.cs)
using UnityEngine;
using UnityEditor;


namespace GeoJsonCityBuilder.Editor
{

    [CustomEditor(typeof(BlocksFromGeoJson))]
    [CanEditMultipleObjects]
    public class BlocksFromGeoJsonEditor : UnityEditor.Editor
    {
        SerializedProperty geoJsonFile;
        SerializedProperty worldPositionAnchor;
        SerializedProperty basePrefab;
        SerializedProperty featureTypeFilter;
        SerializedProperty heightMin;
        SerializedProperty heightMax;
        SerializedProperty topMaterial;
        SerializedProperty sideMaterials;
        SerializedProperty sideUvUnwrapSettings;

        SerializedProperty bottomMaterial;
        SerializedProperty pointedRoofTops;
        SerializedProperty raiseFacades;

        BlocksFromGeoJsonBuilder builder;

        void OnEnable()
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

            builder = new BlocksFromGeoJsonBuilder(this.TargetObject);
        }

        public BlocksFromGeoJson TargetObject { get => this.serializedObject.targetObject as BlocksFromGeoJson; }

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