using UnityEditor;
using UnityEngine;
using GameUtils;

public class SkillDatabaseEditorWindow : DatabaseEditorWindow<SkillDatabase, SkillEntry>
{
    [MenuItem("Game/Skill Database Editor")]
    public static void OpenWindow()
    {
        ShowWindow<SkillDatabaseEditorWindow>("Skill Database");
    }

    protected override void Setup()
    {
        base.Setup();
        // A�ade la columna "Skill" despu�s de la columna "Key"
        AddColumn("Skill", 200, 100, 400, true, TextAlignment.Left);
    }

    protected override void DrawEntryRowData(SerializedProperty entryProperty)
    {
        // Dibuja la columna "Skill" en el �ndice 1
        DrawEntryProperty(entryProperty, "skill", 1, new GUIContent("Skill"));
    }
}