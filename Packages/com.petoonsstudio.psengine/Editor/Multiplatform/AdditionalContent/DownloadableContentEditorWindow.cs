using PetoonsStudio.PSEngine.Utils;
using UnityEditor;
using UnityEditor.Callbacks;

namespace PetoonsStudio.PSEngine.Multiplatform
{
    public class DownloadableContentEditorWindow : AddressablesDatabaseEditorWindow<DownloadableContentEditorWindow, DownloadableContentTable, DownloadableContent>
    {
        [MenuItem("Window/Petoons Studio/Downloadable Content Table")]
        public static void OpenEditor() => ShowWindow("Downloadable Content DB", "CustomSorting");

        [OnOpenAsset]
        public static bool OpenDBAsset(int instanceID, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is DownloadableContentTable)
            {
                OpenEditor();
                return true;
            }

            return false;
        }
    }
}
