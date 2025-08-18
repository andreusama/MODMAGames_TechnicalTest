using PetoonsStudio.PSEngine.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.EnGUI
{
    public class GUIDebugPanel : EnGUIPanel
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject m_AdditionalContentPrefab;

        [Header("Debug Content")]
        [SerializeField] private GameObject m_ProfilePrefab;

        private List<GUIDebugContent> m_NestedLists = new();
        public GUIDebugContent BaseContent => m_Content as GUIDebugContent;
        public GUIDebugContent CurrentNestedList => m_NestedLists[m_NestedLists.Count - 1];

        public EnGUIContent NewNestedList(List<DebugOption> options)
        {
            var list = Instantiate(m_AdditionalContentPrefab, transform).GetComponent<GUIDebugContent>();
            list.SetOptions(options);
            list.OnCloseAnimationComplete += () => RemoveNestedList(list);
            m_NestedLists.Add(list);
            return list;
        }

        private void RemoveNestedList(GUIDebugContent list)
        {
            m_NestedLists.Remove(list);
        }

        public void OpenProfileMenu()
        {
            var panel = Instantiate(m_ProfilePrefab, transform).GetComponent<GUIDebugProfile>();
            EnGUIManager.Instance.PushContent(panel);
        }
    }
}
