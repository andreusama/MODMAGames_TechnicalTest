using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public abstract class AddressablesDatabaseEditorWindow<TEditorWindow, TDatabase, TAddressable> : DatabaseEditorWindow<TDatabase, AddressableEntry<TAddressable>>
        where TEditorWindow : AddressablesDatabaseEditorWindow<TEditorWindow, TDatabase, TAddressable>
        where TDatabase : AddressablesDatabase<TDatabase, TAddressable>
        where TAddressable : UnityEngine.Object
    {
        public static void ShowWindow(string windowTitle = "Database", string icon = "")
        {
            GUIContent title;

            if (icon != "")
                title = new GUIContent(EditorGUIUtility.IconContent(icon)) { text = windowTitle };
            else
                title = new GUIContent(windowTitle);

            var window = ShowWindow<TEditorWindow>(title);
            window.AddColumn(typeof(TAddressable).Name, 300, 80, 800);
        }

        protected override void DrawEntryRowData(SerializedProperty entryProperty)
        {
            DrawEntryProperty(entryProperty, "Value", 1, null);
        }
    }
}
