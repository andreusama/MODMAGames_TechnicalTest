using System.Collections;
using PetoonsStudio.PSEngine.Utils;
using UnityEngine;
using RTLTMPro;
using UnityEngine.Localization;
using PetoonsStudio.PSEngine.Input;

namespace PetoonsStudio.PSEngine.EnGUI
{
    public class TextIconLoader : MonoBehaviour, PSEventListener<NewProviderEvent>
    {
        [SerializeField] private RTLTextMeshPro m_RTLTMPro;
        [SerializeField] private LocalizedString m_LocalizedString;

        const string INPUT_ACTIONS_SEPARATOR = "$";

        private void OnEnable()
        {
            this.PSEventStartListening();
            StartCoroutine(LoadText());
        }

        void OnDisable()
        {
            this.PSEventStopListening();
        }

        private IEnumerator LoadText()
        {
            if (m_LocalizedString.IsEmpty)
                yield break;

            var asyncOperation = m_LocalizedString.GetLocalizedStringAsync();
            yield return asyncOperation;
            string text = asyncOperation.Result;

            if (text.Contains(INPUT_ACTIONS_SEPARATOR))
            {
                m_RTLTMPro.spriteAsset = IconServiceProvider.Instance.GetTextSpriteAsset();
                var parsedActions = InputParser.ParseText(InputManager.Instance.InputAsset, text, INPUT_ACTIONS_SEPARATOR);
                m_RTLTMPro.text = TMPTagProvider.ReplaceActions(parsedActions, text, m_RTLTMPro.spriteAsset);
            }
        }

        public void SetLocalizedString(LocalizedString text)
        {
            m_LocalizedString = text;
        }

        public void OnPSEvent(NewProviderEvent eventType)
        {
            StartCoroutine(LoadText());
        }
    }
}