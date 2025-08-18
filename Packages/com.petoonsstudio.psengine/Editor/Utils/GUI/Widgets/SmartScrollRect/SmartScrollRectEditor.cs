using UnityEditor;
using UnityEditor.UI;

namespace PetoonsStudio.PSEngine.Utils
{
    [CustomEditor(typeof(SmartScrollRect))]
    public class SmartScrollRectEditor : ScrollRectEditor
    {
        private SerializedProperty m_SetupOnStart;
        private SerializedProperty m_ScrollSpeed;
        private SerializedProperty m_ScrollEase;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_SetupOnStart = serializedObject.FindProperty("SetupOnStart");
            m_ScrollSpeed = serializedObject.FindProperty("ScrollSpeed");
            m_ScrollEase = serializedObject.FindProperty("ScrollEase");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(m_SetupOnStart);
            EditorGUILayout.PropertyField(m_ScrollSpeed);
            EditorGUILayout.PropertyField(m_ScrollEase);

            serializedObject.ApplyModifiedProperties();
        }
    } 
}
