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
        SerializedProperty featureTypeFilter;
        SerializedProperty heightMin;
        SerializedProperty heightMax;
        SerializedProperty topMaterial;
        SerializedProperty sideMaterials;
        SerializedProperty sideUvUnwrapSettings;

        SerializedProperty bottomMaterial;
        SerializedProperty pointedRoofTops;

        void OnEnable()
        {
            geoJsonFile = serializedObject.FindProperty("geoJsonFile");
            featureTypeFilter = serializedObject.FindProperty("featureTypeFilter");
            heightMin = serializedObject.FindProperty("heightMin");
            heightMax = serializedObject.FindProperty("heightMax");
            topMaterial = serializedObject.FindProperty("topMaterial");
            sideMaterials = serializedObject.FindProperty("sideMaterials");
            sideUvUnwrapSettings = serializedObject.FindProperty("sideUvUnwrapSettings");
            bottomMaterial = serializedObject.FindProperty("bottomMaterial");
            pointedRoofTops = serializedObject.FindProperty("pointedRoofTops");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(geoJsonFile);
            EditorGUILayout.PropertyField(featureTypeFilter);
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(heightMin);
            EditorGUILayout.PropertyField(heightMax);
            EditorGUILayout.PropertyField(pointedRoofTops);
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(sideMaterials, true);
            EditorGUILayout.PropertyField(sideUvUnwrapSettings);
            EditorGUILayout.PropertyField(bottomMaterial);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear"))
            {
                var controller = this.serializedObject.targetObject as BlocksFromGeoJson;

                controller.RemoveAllChildren();
            }
            if (GUILayout.Button("Generate"))
            {
                var controller = this.serializedObject.targetObject as BlocksFromGeoJson;

                controller.Rebuild();
            }
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }
    }
}