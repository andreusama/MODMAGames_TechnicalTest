using PetoonsStudio.PSEngine.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEditor;
using UnityEngine;
using PetoonsStudio.PSEngine.Framework;

namespace PetoonsStudio.PSEngine.Database
{
    public class SceneGroupDBEditorWindow : AddressablesDatabaseEditorWindow<SceneGroupDBEditorWindow, SceneGroupDB, SceneGroup>
    {
        [MenuItem("Window/Petoons Studio/PS Engine/Database/SceneGroupDB")]
        public static void OpenEditor() => ShowWindow("Scene Group Database", "BuildSettings.Metro.Small");

        [OnOpenAsset]
        public static bool OpenDBAsset(int instanceID, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is SceneGroupDB)
            {
                OpenEditor();
                return true;
            }

            return false;
        }
    }
}
