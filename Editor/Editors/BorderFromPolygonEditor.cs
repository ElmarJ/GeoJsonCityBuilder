//C# Example (LookAtPointEditor.cs)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GeoJsonCityBuilder.Editor
{

    [CustomEditor(typeof(BorderFromPolygon))]
    [CanEditMultipleObjects]
    public class BorderFromPolygonEditor : UnityEditor.Editor
    {
        SerializedProperty floorPolygon;
        SerializedProperty material;
        SerializedProperty height;
        SerializedProperty width;
        SerializedProperty sideUvUnwrapSettings;

        void OnEnable()
        {
            floorPolygon = serializedObject.FindProperty("floorPolygon");
            material = serializedObject.FindProperty("material");
            width = serializedObject.FindProperty("width");
            sideUvUnwrapSettings = serializedObject.FindProperty("sideUvUnwrapSettings");
            height = serializedObject.FindProperty("height");
        }

        public override void OnInspectorGUI()
        {
            var controller = this.serializedObject.targetObject as BorderFromPolygon;

            serializedObject.Update();

            EditorGUILayout.PropertyField(height);
            EditorGUILayout.PropertyField(floorPolygon);
            EditorGUILayout.PropertyField(width);
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(material);
            EditorGUILayout.PropertyField(sideUvUnwrapSettings);

            // EditorGUILayout.PropertyField(floor);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set Test Floor"))
            {
                this.CreateTestFloor();
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
            var controller = this.target as BlockFromPolygon;

            controller.floorPolygon = new List<Vector3>() {
                new Vector3( 5f, 0f, 10f),
                new Vector3(-5f, 0f, 10f),
                new Vector3(-5f, 0f,-10f),
                new Vector3( 5f, 0f,-10f)
            };
        }
    }
}