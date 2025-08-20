using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DotSpawner))]
public class DotSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DotSpawner spawner = (DotSpawner)target;
        GUILayout.Space(10);

        if (GUILayout.Button("Generar Dots (Bake Dots)"))
        {
            spawner.BakeDots();
        }
    }
}