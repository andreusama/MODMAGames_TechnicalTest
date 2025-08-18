using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Multiplatform
{
    [CustomEditor(typeof(DownloadableContentWatcherObjectEnabler))]
    public class AdditionalContentWatcherObjectEnablerEditor : Editor
    {

        private SerializedProperty Target;
        private SerializedProperty Keys;
        private SerializedProperty Activator;
        private string[] AddConNames;

        private void OnEnable()
        {
            Target = serializedObject.FindProperty("TargetGameobject");
            Keys = serializedObject.FindProperty("AdditionalContentKeys");
            Activator = serializedObject.FindProperty("Activator");
            GetKeysNames();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            using (var x = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.ObjectField(Target, GUILayout.MinWidth(200));
                Activator.boolValue = EditorGUILayout.Toggle(Activator.boolValue, GUILayout.MaxWidth(25));
            }
            if(AddConNames.Length > 0) Keys.intValue = EditorGUILayout.MaskField(Keys.intValue, AddConNames);
            
            if(GUILayout.Button("Get DLCs"))
            {
                GetKeysNames();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void GetKeysNames()
        {
            AddConNames = new string[DownloadableContentTable.Instance.Keys.Count];
            DownloadableContentTable.Instance.Keys.CopyTo(AddConNames, 0);
        }
    }
}
