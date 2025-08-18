using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PetoonsStudio.PSEngine.Multiplatform
{
    public class DownloadableContentWatcherObjectEnabler : MonoBehaviour
    {
        [SerializeField]
        public GameObject TargetGameobject = null;
        [Tooltip("Determines if this componentn is an activator or deactivator")]
        public bool Activator = true;
        public UnityEvent<bool> OnAdditionalContentUpdated;
        [Tooltip("The key that will activate or deactivate the object. WARNING, althought you can use more than one, the system have been thought for one only.")]
        public int AdditionalContentKeys = 0;

        DownloadableContentTable m_AdditionalContentTable;

        private void Awake()
        {
#if !UNITY_EDITOR || (UNITY_STANDALONE && !(STANDALONE_STEAM || STANDALONE_EPIC || STANDALONE_GOG || MICROSOFT_GAME_CORE))
            FakeAdditionalContentActivation();
#else
            if (!AdditionalContentManager.InstanceExists)
            {
                Debug.LogWarning($"[ADD_CON] No instance of {typeof(AdditionalContentManager).Name} found by {typeof(DownloadableContentWatcherObjectEnabler).Name} named {this.gameObject.name}");
            }
            else
            {
                AdditionalContentManager.Instance.OnDownloadableContentActivated += AdditionalContentActivated;
                AdditionalContentManager.Instance.OnDownloadableContentDeactivated += AdditionalContentDeactivated;

                m_AdditionalContentTable = AdditionalContentManager.Instance.DownloadableContentTable;

                CheckKeys();
            }
#endif
        }

        private void OnDestroy()
        {
            if (!AdditionalContentManager.InstanceExists)
            {
                Debug.LogWarning($"[ADD_CON] No instance of {typeof(AdditionalContentManager).Name} found by {typeof(DownloadableContentWatcherObjectEnabler).Name} named {this.gameObject.name}");
            }
            else
            {
                AdditionalContentManager.Instance.OnDownloadableContentActivated -= AdditionalContentActivated;
                AdditionalContentManager.Instance.OnDownloadableContentDeactivated -= AdditionalContentDeactivated;
            }
        }

        /// <summary>
        /// Called when one of the additional content is activated
        /// </summary>
        /// <param name="additionalContentKey"></param>
        private void AdditionalContentActivated(string additionalContentKey)
        {

            if (m_AdditionalContentTable.GetSelectedAddCon(AdditionalContentKeys).Contains(additionalContentKey))
            {
                OnAdditionalContentUpdated.Invoke(Activator);
                TargetGameobject.SetActive(Activator);
            }
        }

        /// <summary>
        /// Called when one of the additional content is deactivated
        /// </summary>
        /// <param name="additionalContentKey"></param>
        private void AdditionalContentDeactivated(string additionalContentKey)
        {
            if (m_AdditionalContentTable.GetSelectedAddCon(AdditionalContentKeys).Contains(additionalContentKey))
            {
                OnAdditionalContentUpdated.Invoke(!Activator);
                TargetGameobject.SetActive(!Activator);
            }
        }

        private void CheckKeysIntegrity()
        {
            List<string> tempList = m_AdditionalContentTable.GetSelectedAddCon(AdditionalContentKeys);

            for (int i = 0; i < tempList.Count; i++)
            {
                if (tempList[i].Equals(string.Empty) || tempList[i] == null)
                {
                    Debug.LogWarning($"[ADD_CON] KEY_NULL_OR_EMPTY. The key: {i} of the {typeof(DownloadableContentWatcherObjectEnabler).Name} named {this.gameObject.name} is null or empty.");
                }

                if (!m_AdditionalContentTable.ContainsKey(tempList[i]))
                {
                    Debug.LogWarning($"[ADD_CON] NOT_FOUND_KEY. The key: {i} of the {typeof(DownloadableContentWatcherObjectEnabler).Name} named {this.gameObject.name} have not been found on the table.");
                }
            }
        }

        private void CheckKeys()
        {
            List<string> tempList = m_AdditionalContentTable.GetSelectedAddCon(AdditionalContentKeys);

            foreach (string key in tempList)
            {
                if (AdditionalContentManager.Instance.IsDownloadableContentActive(key))
                {
                    this.TargetGameobject.SetActive(Activator);
                    return;
                }
            }
            this.TargetGameobject.SetActive(!Activator);
        }

        /// <summary>
        /// Fakes an all additionalContent activation
        /// </summary>
        private void FakeAdditionalContentActivation()
        {
            TargetGameobject.SetActive(Activator);
        }

        /// <summary>
        /// Fakes an all additionalContent Deactivation
        /// </summary>
        private void FakeAdditionalContentDeactivation()
        {
            TargetGameobject.SetActive(!Activator);
        }

        private void OnValidate()
        {
            if (TargetGameobject == null) TargetGameobject = this.gameObject;
        }

        private void Reset()
        {
            OnValidate();
        }
    }
}
