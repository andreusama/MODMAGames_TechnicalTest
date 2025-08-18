using PetoonsStudio.PSEngine.EnGUI;
using PetoonsStudio.PSEngine.Utils;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Timeline
{
    [CustomEditor(typeof(ScreenFadeClip))]
    public class ScreenFadeClipEditor : Editor
    {
        private List<string> m_FadeTypeNames;

        public override void OnInspectorGUI()
        {
            if (m_FadeTypeNames == null) FetchFadeTypes();

            //base.OnInspectorGUI();

            var faderProperty = serializedObject.FindProperty("FaderType");
            int currentFaderIndex = Mathf.Max(m_FadeTypeNames.IndexOf(faderProperty.stringValue), 0);

            int newFaderIndex = EditorGUILayout.Popup("FaderType", currentFaderIndex, m_FadeTypeNames.ToArray());

            faderProperty.stringValue = m_FadeTypeNames[newFaderIndex];

            var colorProperty = serializedObject.FindProperty("Color");
            EditorGUILayout.PropertyField(colorProperty);

            serializedObject.ApplyModifiedProperties();
        }

        private void FetchFadeTypes()
        {
            var types = ReflectionUtils.GetClassChildren(typeof(IFader));
            m_FadeTypeNames = new List<string>(types.Count);

            foreach (var type in types)
            {
                m_FadeTypeNames.Add(type.Name);
            }
        }
    }
}
