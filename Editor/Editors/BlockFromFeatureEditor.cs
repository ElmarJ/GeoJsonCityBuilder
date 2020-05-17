//C# Example (LookAtPointEditor.cs)
using UnityEngine;
using UnityEditor;


namespace GeoJsonCityBuilder.Editor
{

    [CustomEditor(typeof(BlockFromFeature))]
    [CanEditMultipleObjects]
    public class BlockFromFeatureJsonEditor : UnityEditor.Editor
    {
        SerializedProperty topMaterial;
        SerializedProperty bottomMaterial;
        SerializedProperty sideMaterial;
        SerializedProperty height;
        SerializedProperty sideUvUnwrapSettings;

        void OnEnable()
        {
            topMaterial = serializedObject.FindProperty("topMaterial");
            bottomMaterial = serializedObject.FindProperty("bottomMaterial");
            sideMaterial = serializedObject.FindProperty("sideMaterial");
            sideUvUnwrapSettings = serializedObject.FindProperty("sideUvUnwrapSettings");
            height = serializedObject.FindProperty("height");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(height);
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(topMaterial);
            EditorGUILayout.PropertyField(bottomMaterial);
            EditorGUILayout.PropertyField(sideMaterial);
            EditorGUILayout.PropertyField(sideUvUnwrapSettings);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Draw"))
            {
                var controller = this.serializedObject.targetObject as BlockFromFeature;

                controller.Draw();
            }
            if (GUILayout.Button("Add Rooftop"))
            {
                var controller = this.serializedObject.targetObject as BlockFromFeature;

                controller.AddPointedRoof();
            }
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }
    }
}