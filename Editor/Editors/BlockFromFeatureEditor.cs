//C# Example (LookAtPointEditor.cs)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

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
        SerializedProperty pointedRoof;
        SerializedProperty pointedRoofHeight;


        void OnEnable()
        {
            topMaterial = serializedObject.FindProperty("topMaterial");
            bottomMaterial = serializedObject.FindProperty("bottomMaterial");
            sideMaterial = serializedObject.FindProperty("sideMaterial");
            sideUvUnwrapSettings = serializedObject.FindProperty("sideUvUnwrapSettings");
            height = serializedObject.FindProperty("height");
            pointedRoof = serializedObject.FindProperty("pointedRoof");
            pointedRoofHeight = serializedObject.FindProperty("pointedRoofHeight");
        }

        public override void OnInspectorGUI()
        {
            var controller = this.serializedObject.targetObject as BlockFromFeature;

            serializedObject.Update();
            EditorGUILayout.PropertyField(height);
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(topMaterial);
            EditorGUILayout.PropertyField(bottomMaterial);
            EditorGUILayout.PropertyField(sideMaterial);
            EditorGUILayout.PropertyField(sideUvUnwrapSettings);
            EditorGUILayout.PropertyField(pointedRoof);
            EditorGUILayout.PropertyField(pointedRoofHeight);
            if (controller.floor != null) {
                EditorGUILayout.BeginFoldoutHeaderGroup(false, "Polygon");
                foreach(var coordinate in (this.serializedObject.targetObject as BlockFromFeature).floor)
                {
                    EditorGUILayout.LabelField(coordinate.ToString());
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            // EditorGUILayout.PropertyField(floor);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set Test Floor"))
            {
                this.CreateTestFloor();
            }
            if (GUILayout.Button("Draw"))
            {
                controller.Draw();
            }
            if (GUILayout.Button("Add Rooftop"))
            {
                controller.AddPointedRoof();
            }
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }

        private void CreateTestFloor() {
            var controller = this.target as BlockFromFeature;

            controller.floor = new List<Vector3>() {
                new Vector3( 5f, 0f, 10f),
                new Vector3(-5f, 0f, 10f),
                new Vector3(-5f, 0f,-10f),
                new Vector3( 5f, 0f,-10f)
            };
        }
    }
}