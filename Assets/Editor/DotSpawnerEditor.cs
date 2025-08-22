using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DirtyDotSpawner))]
public class DotSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DirtyDotSpawner spawner = (DirtyDotSpawner)target;
        GUILayout.Space(10);

        if (GUILayout.Button("Generar Dots (Bake Dots)"))
        {
            spawner.BakeDots();
        }
    }
}