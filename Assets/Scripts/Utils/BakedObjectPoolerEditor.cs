#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BakedSimpleObjectPooler))]
public class BakedObjectPoolerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var pooler = (BakedSimpleObjectPooler)target;

        EditorGUILayout.Space();
        if (GUILayout.Button("Auto-fill BakedObjects with children"))
        {
            pooler.BakedObjects.Clear();
            for (int i = 0; i < pooler.transform.childCount; i++)
            {
                pooler.BakedObjects.Add(pooler.transform.GetChild(i).gameObject);
            }
            EditorUtility.SetDirty(pooler);
        }

        if (GUILayout.Button("Refill Pool (Play Mode)"))
        {
            if (Application.isPlaying)
            {
                pooler.FillObjectPool();
            }
            else
            {
                Debug.LogWarning("Play Mode only.");
            }
        }
    }
}

#endif