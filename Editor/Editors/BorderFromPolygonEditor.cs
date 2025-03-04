//C# Example (LookAtPointEditor.cs)
using System.Collections.Generic;
using GeoJsonCityBuilder.Components;
using GeoJsonCityBuilder.Editor.Builders;
using UnityEditor;
using UnityEngine;

namespace GeoJsonCityBuilder.Editor.Editors
{

    [CustomEditor(typeof(BorderFromPolygon))]
    [CanEditMultipleObjects]
    public class BorderFromPolygonEditor : UnityEditor.Editor //-V3072
    {
        private SerializedProperty floorPolygon;
        private SerializedProperty material;
        private SerializedProperty height;
        private SerializedProperty outerExtension;
        private SerializedProperty innerExtension;
        private SerializedProperty sideUvUnwrapSettings;

        private void OnEnable()
        {
            floorPolygon = serializedObject.FindProperty("floorPolygon");
            material = serializedObject.FindProperty("material");
            outerExtension = serializedObject.FindProperty("outerExtension");
            innerExtension = serializedObject.FindProperty("innerExtension");
            sideUvUnwrapSettings = serializedObject.FindProperty("sideUvUnwrapSettings");
            height = serializedObject.FindProperty("height");
        }

        public override void OnInspectorGUI()
        {
            var controller = serializedObject.targetObject as BorderFromPolygon;

            serializedObject.Update();

            EditorGUILayout.PropertyField(height);
            EditorGUILayout.PropertyField(floorPolygon);
            EditorGUILayout.PropertyField(outerExtension);
            EditorGUILayout.PropertyField(innerExtension);
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(material);
            EditorGUILayout.PropertyField(sideUvUnwrapSettings);

            // EditorGUILayout.PropertyField(floor);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set Test Floor"))
            {
                CreateTestFloor();
            }
            if (GUILayout.Button("Draw"))
            {
                var builder = new BorderFromPolygonBuilder(controller);
                builder.Draw();
            }
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }

        private void CreateTestFloor()
        {
            var controller = target as BorderFromPolygon;

            controller.floorPolygon = new List<Vector3>() {
                new( 5f, 0f, 10f),
                new(-5f, 0f, 10f),
                new(-5f, 0f,-10f),
                new( 5f, 0f,-10f)
            };
        }
    }
}