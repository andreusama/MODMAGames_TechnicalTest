using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public abstract class DatabaseEditorWindow<TDatabase, TEntry> : EditorWindow
        where TDatabase : Database<TDatabase, TEntry>
        where TEntry : ITableEntry
    {
        protected SerializedObject m_SerializedDatabase;
        protected TDatabase m_Database;
        protected SerializedProperty m_DatabaseValues;
        protected MultiColumnHeader m_ColumnHeader;
        protected MultiColumnHeaderState.Column[] m_HeaderColumns;
        protected string m_AddEntryName;
        protected Vector2 m_Scroll;
        protected SearchField m_SearchField;
        protected string m_SearchQuery;
        protected bool m_CaseSensitiveSearch = false;

        protected Queue<SerializedProperty> m_PropertiesToSave;
        protected Queue<TEntry> m_EntriesToAdd;
        protected Queue<string> m_EntriesToRemove;
        protected Dictionary<string, string> m_EntryKeysToReplace;

        protected List<string> m_CachedKeyList;
        protected int m_ListLength;
        protected bool m_DatabaseChanged;

        protected const int COLUMN_HEADER_HEIGHT = 25;
        protected const int VALUE_FIELD_WIDTH_OFFSET = -5;

        public static TEditorWindow ShowWindow<TEditorWindow>(string title = "Database") where TEditorWindow : DatabaseEditorWindow<TDatabase, TEntry>
        {
            return ShowWindow<TEditorWindow>(new GUIContent(title));
        }

        public static TEditorWindow ShowWindow<TEditorWindow>(GUIContent title) where TEditorWindow : DatabaseEditorWindow<TDatabase, TEntry>
        {
            // Get existing open window or if none, make a new one:
            TEditorWindow window = (TEditorWindow)EditorWindow.GetWindow(typeof(TEditorWindow));
            window.titleContent = title;
            window.minSize = new Vector2(485f, window.minSize.y);
            window.Show();
            window.Setup();
            return window;
        }

        private TDatabase FindDatabase()
        {
            string[] guid = AssetDatabase.FindAssets("t:" + typeof(TDatabase).ToString());
            if (guid.Length != 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid[0]);
                return AssetDatabase.LoadAssetAtPath(path, typeof(TDatabase)) as TDatabase;
            }
            return null;
        }

        private void Setup()
        {
            m_PropertiesToSave = new Queue<SerializedProperty>();
            m_EntriesToAdd = new Queue<TEntry>();
            m_EntriesToRemove = new Queue<string>();
            m_EntryKeysToReplace = new Dictionary<string, string>();

            m_SearchField = new SearchField();
            m_AddEntryName = "";

            DeleteColumns();
            AddColumn("Key");
        }

        protected void AddColumn(string title, int width = 80, int minWidth = 50, int maxWidth = 500, bool autoResize = true, TextAlignment textAlignment = TextAlignment.Center)
        {
            if (m_HeaderColumns == null) DeleteColumns();

            m_HeaderColumns = m_HeaderColumns.Append(new MultiColumnHeaderState.Column()
            {
                headerContent = new GUIContent(title),
                width = width,
                minWidth = minWidth,
                maxWidth = maxWidth,
                autoResize = autoResize,
                headerTextAlignment = textAlignment
            }).ToArray();

            SetupColumnsHeader();
        }

        protected void DeleteColumns()
        {
            m_HeaderColumns = new MultiColumnHeaderState.Column[] { };
        }

        private void SetupColumnsHeader()
        {
            m_ColumnHeader = new MultiColumnHeader(new MultiColumnHeaderState(m_HeaderColumns)) { height = COLUMN_HEADER_HEIGHT };
            m_ColumnHeader.ResizeToFit();
        }

        private void SetCurrentDatabase(TDatabase database)
        {
            if (!database) return;
            m_Database = database;
            m_SerializedDatabase = new SerializedObject(m_Database);
            m_DatabaseValues = m_SerializedDatabase.FindProperty("m_Entries").FindPropertyRelative("m_Values");

            var keyList = m_Database.Keys.ToList();

            if (m_ColumnHeader.sortedColumnIndex == 0)
            {
                if (m_ColumnHeader.IsSortedAscending(0))
                {
                    keyList = keyList.OrderBy((x) => x).ToList();
                }
                else
                {
                    keyList = keyList.OrderByDescending((x) => x).ToList();
                }
            }

            m_CachedKeyList = keyList;
        }

        private new void SaveChanges()
        {
            m_SerializedDatabase.ApplyModifiedProperties();
            m_DatabaseValues.serializedObject.ApplyModifiedProperties();

            while (m_PropertiesToSave.Count > 0)
            {
                var property = m_PropertiesToSave.Dequeue();
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorUtility.SetDirty(m_Database);

            SetCurrentDatabase(m_Database);
        }

        private void OnGUI()
        {
            if (m_Database == null || m_SerializedDatabase == null)
            {
                var db = FindDatabase();
                if (db) SetCurrentDatabase(db);
                else DrawNoDatabase();
            }

            if(m_Database != null)
            {
                DrawToolbar(position);

                EditorGUI.BeginChangeCheck();

                DrawColumnHeader();
                GUILayout.Space(COLUMN_HEADER_HEIGHT);
                m_Scroll = EditorGUILayout.BeginScrollView(m_Scroll);

                DrawTable();
                EditorGUILayout.EndScrollView();

                if (EditorGUI.EndChangeCheck() || m_DatabaseChanged)
                {
                    UpdateDatabase();
                    SaveChanges();
                    m_DatabaseChanged = false;
                }
            }
        }

        private void UpdateDatabase()
        {
            while (m_EntriesToAdd.Count > 0)
            {
                var entry = m_EntriesToAdd.Dequeue();
                m_Database.Add(entry.ID, entry);
            }

            while (m_EntriesToRemove.Count > 0)
            {
                var entryKey = m_EntriesToRemove.Dequeue();
                m_Database.Remove(entryKey);
            }

            foreach (var entry in m_EntryKeysToReplace)
            {
                m_Database.ChangeEntryKey(entry.Key, entry.Value, "_ALT");
            }
            m_EntryKeysToReplace.Clear();

            SetCurrentDatabase(m_Database);
        }

        private void DrawNoDatabase()
        {
            EditorGUILayout.HelpBox("No database is selected for editing. Please select the database you wish to edit.", MessageType.Warning);
            var database = EditorGUILayout.ObjectField(m_Database, typeof(TDatabase), false) as TDatabase;
            SetCurrentDatabase(database);
        }

        private void DrawColumnHeader()
        {
            GUILayout.FlexibleSpace();
            var windowVisibleRect = GUILayoutUtility.GetLastRect();
            windowVisibleRect.width = position.width;
            windowVisibleRect.height = position.height;

            var headerRect = windowVisibleRect;
            headerRect.height = m_ColumnHeader.height;
            m_ColumnHeader.OnGUI(headerRect, m_Scroll.x);
        }

        protected virtual void DrawToolbar(Rect windowRect)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            m_AddEntryName = EditorGUILayout.TextField(m_AddEntryName);

            GUIContent addEntryBtn = new GUIContent(EditorGUIUtility.IconContent("Toolbar Plus")) { text = "Add Entry" };

            GUI.enabled = m_AddEntryName != "";
            if (GUILayout.Button(addEntryBtn, EditorStyles.toolbarButton, GUILayout.MaxWidth(100)))
            {
                if (m_Database.ContainsKey(m_AddEntryName))
                {
                    Debug.LogWarning("You can't add an entry to the table with the same key.");
                }
                else
                {
                    var entry = (TEntry)Activator.CreateInstance(typeof(TEntry));
                    entry.ID = m_AddEntryName;

                    m_EntriesToAdd.Enqueue(entry);
                    m_DatabaseChanged = true;
                    m_Scroll.y = position.height;
                    GUI.FocusControl(null);
                    m_AddEntryName = "";
                }
            }
            GUI.enabled = true;

            GUIStyle entryCountIndicatorStyle = new GUIStyle(EditorStyles.label) { fontSize = 12 };
            entryCountIndicatorStyle.normal.textColor = Color.white * 0.8f;

            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbarButton);
            GUILayout.Label(" Search  ");
            m_SearchQuery = m_SearchField.OnGUI(m_SearchQuery, GUILayout.MaxWidth(300));
            m_CaseSensitiveSearch = GUILayout.Toggle(m_CaseSensitiveSearch, " Case Sensitive  ");
            EditorGUILayout.EndHorizontal();

            DrawAdditionalToolbarButtons(windowRect);

            GUILayout.FlexibleSpace();
            GUILayout.Label($"{m_ListLength} entries  ", entryCountIndicatorStyle);

            EditorGUILayout.EndHorizontal();
        }

        protected virtual void DrawAdditionalToolbarButtons(Rect windowRect) { }

        private void DrawTable()
        {
            //GUILayout.Space(COLUMN_HEADER_HEIGHT);
            GUILayout.BeginVertical(GUILayout.ExpandHeight(false));

            var keys = m_CachedKeyList;

            if (m_SearchQuery != null && m_SearchQuery != "")
            {
                if (m_CaseSensitiveSearch)
                    keys = keys.Where((x) => x.Contains(m_SearchQuery)).ToList();
                else
                    keys = keys.Where((x) => x.IndexOf(m_SearchQuery, StringComparison.OrdinalIgnoreCase) != -1).ToList();
            }

            m_ListLength = keys.Count;

            for (int i = 0; i < keys.Count; i++)
            {
                GUI.enabled = i % 2 == 0;
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUI.enabled = true;
                DrawEntryKey(keys[i], GetColumnWidth(0) + VALUE_FIELD_WIDTH_OFFSET);
                int index = m_Database.Keys.ToList().IndexOf(keys[i]);

                if (m_DatabaseValues == null)
                {
                    Debug.LogError("Make sure your TableEntry-inheriting class is System.Serializable");
                    return;
                }

                var entryProperty = m_DatabaseValues.GetArrayElementAtIndex(index);
                if (entryProperty != null) DrawEntryRowData(entryProperty);

                GUILayout.EndHorizontal();
            }

            // I know this is ugly. Let me know if you find a cleaner solution...
            GUILayout.FlexibleSpace();
            GUILayout.FlexibleSpace();
            GUILayout.FlexibleSpace();
            GUILayout.FlexibleSpace();
            GUILayout.FlexibleSpace();
            GUILayout.FlexibleSpace();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        private void DrawEntryKey(string key, float width)
        {
            GUILayout.BeginVertical(GUILayout.Width(width));
            GUILayout.Space(8); //GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            string updatedKey = EditorGUILayout.TextField(key);
            if (updatedKey != key)
            {
                m_EntryKeysToReplace[key] = updatedKey;
                m_DatabaseChanged = true;
            }
            if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("d_winbtn_win_close")), GUILayout.MaxWidth(25)))
            {
                m_EntriesToRemove.Enqueue(key);
                m_DatabaseChanged = true;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(8); //GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        protected abstract void DrawEntryRowData(SerializedProperty entryProperty);

        protected virtual void DrawEntryProperty(SerializedProperty entryProperty, string propertyName, int column, GUIContent label)
        {
            GUILayout.BeginVertical(GUILayout.Width(GetColumnWidth(column) + VALUE_FIELD_WIDTH_OFFSET));
            GUILayout.Space(8); //GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            var childProperty = entryProperty.FindPropertyRelative(propertyName);
            if (childProperty != null)
            {
                EditorGUILayout.PropertyField(childProperty, label, true);
                m_PropertiesToSave.Enqueue(childProperty);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(8); //GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        protected virtual void DrawEntryProperty(string propertyName, SerializedProperty entryProperty, int column, GUIContent label, string iconName)
        {
            GUILayout.BeginVertical(GUILayout.Width(GetColumnWidth(column) + VALUE_FIELD_WIDTH_OFFSET));
            GUILayout.Space(8); //GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            GUILayout.Label(EditorGUIUtility.IconContent(iconName), GUILayout.Width(20), GUILayout.Height(20));
            var childProperty = entryProperty.FindPropertyRelative(propertyName);
            if (childProperty != null)
            {
                EditorGUILayout.PropertyField(childProperty, label, true);
                m_PropertiesToSave.Enqueue(childProperty);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(8); //GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        protected void DrawCustomRowUI(SerializedProperty entryProperty, int column)
        {
            GUILayout.BeginVertical(GUILayout.Width(GetColumnWidth(column) + VALUE_FIELD_WIDTH_OFFSET));
            GUILayout.Space(8); //GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.Space(8);

            CustomRowUI(entryProperty, column);

            GUILayout.EndHorizontal();
            GUILayout.Space(8); //GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        protected virtual void CustomRowUI(SerializedProperty entryProperty, int column) { }

        protected float GetColumnWidth(int index)
        {
            return m_ColumnHeader.GetColumnRect(index).width;
        }
    }
}