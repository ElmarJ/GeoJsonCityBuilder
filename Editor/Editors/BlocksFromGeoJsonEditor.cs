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
        SerializedProperty sideMaterial1;
        SerializedProperty sideMaterial2;
        SerializedProperty sideMaterial3;
        SerializedProperty sideMaterial4;
        SerializedProperty sideMaterial5;
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
            sideMaterial1 = serializedObject.FindProperty("sideMaterial1");
            sideMaterial2 = serializedObject.FindProperty("sideMaterial2");
            sideMaterial3 = serializedObject.FindProperty("sideMaterial3");
            sideMaterial4 = serializedObject.FindProperty("sideMaterial4");
            sideMaterial5 = serializedObject.FindProperty("sideMaterial5");
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
            EditorGUILayout.PropertyField(topMaterial);
            EditorGUILayout.PropertyField(sideMaterial1);
            EditorGUILayout.PropertyField(sideMaterial2);
            EditorGUILayout.PropertyField(sideMaterial3);
            EditorGUILayout.PropertyField(sideMaterial4);
            EditorGUILayout.PropertyField(sideMaterial5);
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