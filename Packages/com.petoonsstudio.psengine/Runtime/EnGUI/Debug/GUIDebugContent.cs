using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PetoonsStudio.PSEngine.EnGUI
{
    public class GUIDebugContent : EnGUIContent
    {
        [SerializeField] private GameObject m_OptionPrefab;
        [SerializeField] private SmartScrollRect m_Scroll;

        private List<DebugOption> m_OptionsData;
        private bool m_Initialized = false;

        protected override void Awake()
        {
            base.Awake();
            m_OnCancelAction.AddListener(() => EnGUIManager.Instance.RemoveLastContent());
        }

        public void SetOptions(List<DebugOption> options)
        {
            m_OptionsData = options;

            if (m_Initialized)
                UpdateOptions();
        }

        public override IEnumerator Initialize()
        {
            UpdateOptions();
            m_Initialized = true;
            yield break;
        }

        public override void DisableContent()
        {
            base.DisableContent();
            Destroy(gameObject);
        }

        private void UpdateOptions()
        {
            if (m_SelectableContent == null) m_SelectableContent = GetComponentsInChildren<Selectable>(includeInactive: true).ToList();
            m_Scroll.content.DestroyChildren();

            if (m_OptionsData == null || m_OptionsData.Count == 0)
                return;

            for (int i = 0; i < m_OptionsData.Count; i++)
            {
                AddOption(m_OptionsData[i]);
            }

            m_Scroll.Setup();
        }

        private void AddOption(DebugOption optionData)
        {
            var go = Instantiate(m_OptionPrefab, m_Scroll.content);
            var option = go.GetComponent<GUIDebugOption>();
            option.Setup(optionData);

            m_SelectableContent.Add(option.Selectable);

            go.SetActive(true);

            if (m_Selection == null)
            {
                m_Selection = go;
                if (m_CurrentEventSystem != null)
                    m_CurrentEventSystem.SetSelectedGameObject(m_Selection);
            }
        }
    }
}