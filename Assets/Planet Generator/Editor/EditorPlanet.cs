using System;
using UnityEngine;
using UnityEditor;

using Object = UnityEngine.Object;

[CustomEditor(typeof(Planet))]
public class EditorPlanet : Editor
{

    private Planet planet;

    private Editor shapeEditor;
    private Editor colorEditor;

    private void OnEnable()
    {
        planet = (Planet)target;
    }

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed)
            {
                planet.GeneratePlanet();
            }
        }

        if (GUILayout.Button("Generate Planet"))
        {
            planet.GeneratePlanet();
        }

        DrawSettingsEditor(planet.GetShapeSettings, planet.OnShapeSettingsUpdated, 
            serializedObject.FindProperty("shapeSettings"), ref shapeEditor);
        DrawSettingsEditor(planet.GetColorSettings, planet.OnColorSettingsUpdated, 
            serializedObject.FindProperty("colorSettings"), ref colorEditor);
        EditorGUILayout.Space();
    }

    private void DrawSettingsEditor(Object settings, Action onSettingsUpdated, SerializedProperty prop, ref Editor editor)
    {
        if (settings == null) return;

        prop.isExpanded = EditorGUILayout.InspectorTitlebar(prop.isExpanded, settings);
        if (!prop.isExpanded) return;

        EditorGUI.indentLevel++;
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            CreateCachedEditor(settings, null, ref editor);
            editor.OnInspectorGUI();

            if (check.changed)
            {
                onSettingsUpdated?.Invoke();
            }
        }
        EditorGUI.indentLevel--;
    }

}
