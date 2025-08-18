using PetoonsStudio.PSEngine.Utils;
using UnityEditor;
using UnityEditor.Callbacks;

namespace PetoonsStudio.PSEngine.Achievements
{
    public class AchievementTableEditorWindow : AddressablesDatabaseEditorWindow<AchievementTableEditorWindow, AchievementTable, Achievement>
    {
        [MenuItem("Window/Petoons Studio/Achievement Table")]

        public static void OpenEditor() => ShowWindow("Achievement DB", "CustomSorting");

        [OnOpenAsset]
        public static bool OpenDBAsset(int instanceID, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is AchievementTable)
            {
                OpenEditor();
                return true;
            }

            return false;
        }
    }
}