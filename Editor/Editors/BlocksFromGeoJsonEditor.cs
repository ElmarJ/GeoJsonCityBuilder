//C# Example (LookAtPointEditor.cs)
using UnityEngine;
using UnityEditor;


namespace GeoJsonCityBuilder.Editor.Editors
{

    [CustomEditor(typeof(BlocksFromGeoJson))]
    [CanEditMultipleObjects]
    public class BlocksFromGeoJsonEditor : UnityEditor.Editor
    {
        SerializedProperty geoJsonFile;
        SerializedProperty worldPosition;
        SerializedProperty prefab;
        SerializedProperty featureTypeFilter;
        SerializedProperty heightMin;
        SerializedProperty heightMax;
        SerializedProperty topMaterial;
        SerializedProperty sideMaterials;
        SerializedProperty sideUvUnwrapSettings;

        SerializedProperty bottomMaterial;
        SerializedProperty pointedRoofTops;
        SerializedProperty raiseFacades;

        SerializedProperty timeStartYearField;
        SerializedProperty timeEndYearField;

        Builders.BlocksFromGeoJsonBuilder builder;

        void OnEnable()
        {
            geoJsonFile = serializedObject.FindProperty("geoJsonFile");
            worldPosition = serializedObject.FindProperty("worldPosition");
            prefab = serializedObject.FindProperty("prefab");
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

            builder = new Builders.BlocksFromGeoJsonBuilder(this.TargetObject);
        }

        public BlocksFromGeoJson TargetObject { get => this.serializedObject.targetObject as BlocksFromGeoJson; }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(geoJsonFile);
            EditorGUILayout.PropertyField(worldPosition);
            EditorGUILayout.PropertyField(prefab);
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
