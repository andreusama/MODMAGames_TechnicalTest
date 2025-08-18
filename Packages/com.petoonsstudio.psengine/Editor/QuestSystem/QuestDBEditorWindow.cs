using PetoonsStudio.PSEngine.Tools;
using PetoonsStudio.PSEngine.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    public class QuestDBEditorWindow : AddressablesDatabaseEditorWindow<QuestDBEditorWindow, QuestDB, QuestData>
    {
        [MenuItem("Window/Petoons Studio/PSEngine/Quest System/QuestDB", priority = ToolsUtils.QUEST_CATEGORY)]
        public static void OpenEditor() => ShowWindow("Quest DB", "CustomSorting");

        [OnOpenAsset]
        public static bool OpenDBAsset(int instanceID, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is QuestDB)
            {
                OpenEditor();
                return true;
            }

            return false;
        }
    }
}