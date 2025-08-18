using PetoonsStudio.PSEngine.EnGUI;
using PetoonsStudio.PSEngine.Timeline;
using PetoonsStudio.PSEngine.Utils;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace PetoonsStudio.PSEngine
{
    public class SubtitlesController : PersistentSingleton<SubtitlesController>
    {
        [SerializeField] private AssetReferenceGameObject m_GUI;

        private GUISubtitles m_GUISubtitles;
        private bool m_AsyncGUIRequest;

        private AsyncOperationHandle<GameObject> m_AsyncOperation;

        public void CreateGUI()
        {
            if (m_AsyncGUIRequest)
                return;

            if (m_GUI == null || !m_GUI.RuntimeKeyIsValid())
                return;

            StartCoroutine(CreateGUI_Internal());
        }

        private IEnumerator CreateGUI_Internal()
        {
            m_AsyncGUIRequest = true;

            m_AsyncOperation = Addressables.InstantiateAsync(m_GUI, EnGUIManager.Instance.GUICutscenes);

            yield return m_AsyncOperation;

            m_GUISubtitles = m_AsyncOperation.Result.GetComponent<GUISubtitles>();

            m_AsyncGUIRequest = false;
        }

        public void DestroyGUI()
        {
            if (m_AsyncGUIRequest)
            {
                StopCoroutine(nameof(CreateGUI_Internal));
                return;
            }

            if (m_GUISubtitles != null)
                Addressables.ReleaseInstance(m_AsyncOperation);
        }

        public void ShowText(string text)
        {
            if (m_GUISubtitles == null)
                return;

            m_GUISubtitles.ShowText(text);
        }

        public void HideText()
        {
            if (m_GUISubtitles == null)
                return;

            m_GUISubtitles.HideText();
        }
    }
}
