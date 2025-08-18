using UnityEditor;

namespace PetoonsStudio.PSEngine.Framework
{
    public abstract class DataPersisterEditor : Editor
    {
        protected IDataPersister m_DataPersister;

        private SerializedProperty m_Settings;
        private SerializedProperty m_Type;
        private SerializedProperty m_Tag;
        private SerializedProperty m_Release;

        void OnEnable()
        {
            m_DataPersister = (IDataPersister)target;

            m_Settings = serializedObject.FindProperty("DataSettings");
            m_Type = m_Settings.FindPropertyRelative("Type");
            m_Tag = m_Settings.FindPropertyRelative("DataTag");
            m_Release = m_Settings.FindPropertyRelative("ReleaseOnSave");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Persistence", EditorStyles.boldLabel);

            DataPersisterGUI();
        }

        public void DataPersisterGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_Type);
            if (m_DataPersister.GetDataSettings().Type != DataSettings.PersistenceType.DoNotPersist)
            {
                EditorGUILayout.PropertyField(m_Tag);
            }
            EditorGUILayout.PropertyField(m_Release);

            serializedObject.ApplyModifiedProperties();
        }
    }
}