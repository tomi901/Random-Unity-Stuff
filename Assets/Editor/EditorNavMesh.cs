using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NavigationMesh2D))]
public class EditorNavMesh : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NavigationMesh2D navMesh = target as NavigationMesh2D;
        if (!navMesh) return;

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        if (GUILayout.Button("Generate NavMesh"))
        {
            Undo.RecordObject(navMesh, "Generate 2D Navigation Map.");
            navMesh.GenerateNavMesh();
            serializedObject.ApplyModifiedProperties();
        }

        if (GUILayout.Button("Clear"))
        {
            Undo.RecordObject(navMesh, "Clear 2D Navigation Map.");
            navMesh.Clear();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
