//C# Example (LookAtPointEditor.cs)
using UnityEngine;
using UnityEditor;


namespace GeoJsonCityBuilder.Editor
{

    [CustomEditor(typeof(PrefabOnGrid))]
    [CanEditMultipleObjects]
    public class PrefabOnGridEditor : UnityEditor.Editor
    {
        SerializedProperty maxRowHeight;
        SerializedProperty maxColumnWidth;
        SerializedProperty prefab;
        SerializedProperty rotation;
        SerializedProperty zPosition;
        SerializedProperty width;
        SerializedProperty height;

        void OnEnable()
        {
            maxRowHeight = serializedObject.FindProperty("maxRowHeight");
            maxColumnWidth = serializedObject.FindProperty("maxColumnWidth");
            prefab = serializedObject.FindProperty("prefab");
            rotation = serializedObject.FindProperty("rotation");
            zPosition = serializedObject.FindProperty("zPosition");
            width = serializedObject.FindProperty("width");
            height = serializedObject.FindProperty("height");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(maxRowHeight);
            EditorGUILayout.PropertyField(maxColumnWidth);
            EditorGUILayout.PropertyField(prefab);
            EditorGUILayout.PropertyField(rotation);
            EditorGUILayout.PropertyField(zPosition);
            EditorGUILayout.PropertyField(width);
            EditorGUILayout.PropertyField(height);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Recalculate"))
            {
                var controller = this.serializedObject.targetObject as PrefabOnGrid;
                controller.RecalculateGrid();
            }
            if (GUILayout.Button("Draw"))
            {
                var controller = this.serializedObject.targetObject as PrefabOnGrid;
                controller.DrawGrid();
            }
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }
    }
}