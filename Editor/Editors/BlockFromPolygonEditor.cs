//C# Example (LookAtPointEditor.cs)
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GeoJsonCityBuilder.Editor.Builders;
using GeoJsonCityBuilder.Components;

namespace GeoJsonCityBuilder.Editor.Editors
{

    [CustomEditor(typeof(BlockFromPolygon))]
    [CanEditMultipleObjects]
    public class BlockFromPolygonEditor : UnityEditor.Editor
    {
        private SerializedProperty topMaterial;
        private SerializedProperty bottomMaterial;
        private SerializedProperty sideMaterial;
        private SerializedProperty height;
        private SerializedProperty sideUvUnwrapSettings;
        private SerializedProperty pointedRoof;
        private SerializedProperty pointedRoofHeight;
        private SerializedProperty leanForward;
        private SerializedProperty raiseFrontAndBackFacadeTop;
        private bool showFloorPolygon = false;

        private void OnEnable()
        {
            topMaterial = serializedObject.FindProperty("topMaterial");
            bottomMaterial = serializedObject.FindProperty("bottomMaterial");
            sideMaterial = serializedObject.FindProperty("sideMaterial");
            sideUvUnwrapSettings = serializedObject.FindProperty("sideUvUnwrapSettings");
            height = serializedObject.FindProperty("height");
            pointedRoof = serializedObject.FindProperty("pointedRoof");
            pointedRoofHeight = serializedObject.FindProperty("pointedRoofHeight");
            leanForward = serializedObject.FindProperty("leanForward");
            raiseFrontAndBackFacadeTop = serializedObject.FindProperty("raiseFrontAndBackFacadeTop");
        }

        public override void OnInspectorGUI()
        {
            var controller = serializedObject.targetObject as BlockFromPolygon;

            serializedObject.Update();

            EditorGUILayout.PropertyField(height);
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(topMaterial);
            EditorGUILayout.PropertyField(bottomMaterial);
            EditorGUILayout.PropertyField(sideMaterial);
            EditorGUILayout.PropertyField(sideUvUnwrapSettings);
            EditorGUILayout.PropertyField(pointedRoof);
            EditorGUILayout.PropertyField(pointedRoofHeight);
            EditorGUILayout.PropertyField(raiseFrontAndBackFacadeTop);
            EditorGUILayout.PropertyField(leanForward);

            if (controller.floorPolygon != null)
            {
                showFloorPolygon = EditorGUILayout.BeginFoldoutHeaderGroup(showFloorPolygon, "Polygon");
                if (showFloorPolygon)
                {
                    foreach (var coordinate in controller.floorPolygon)
                    {
                        EditorGUILayout.LabelField(coordinate.ToString());
                    }
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            // EditorGUILayout.PropertyField(floor);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set Test Floor"))
            {
                CreateTestFloor();
            }
            if (GUILayout.Button("Draw"))
            {
                var builder = new BlockFromPolygonBuilder(controller);
                builder.Draw();
            }
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }

        private void CreateTestFloor()
        {
            var controller = target as BlockFromPolygon;

            controller.floorPolygon = new List<Vector3>() {
                new( 5f, 0f, 10f),
                new(-5f, 0f, 10f),
                new(-5f, 0f,-10f),
                new( 5f, 0f,-10f)
            };
        }
    }
}