using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.QuestSystem;
using PetoonsStudio.PSEngine.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Multiplatform
{
    public class AdditionalContentManager : PersistentSingleton<AdditionalContentManager>
    {
        public DownloadableContentTable DownloadableContentTable { get => m_DownloadableContentTable; }
        public bool ActivateAddConOnInitialize = true;

        public Action<string> OnDownloadableContentActivated;
        public Action<string> OnDownloadableContentDeactivated;

        [SerializeField]
        private DownloadableContentTable m_DownloadableContentTable = null;

        private List<string> m_ActiveDownloadableContents = new List<string>();

        public void Initialize()
        {
            if (DownloadableContentTable == null)
                return;

            if (ActivateAddConOnInitialize)
            {
                SearchAndActiveDownloadableContent();
            }
        }

        #region DLC's

        /// <summary>
        /// Check if an additional content is active right now
        /// </summary>
        /// <param name="additionalContentKey"></param>
        /// <returns></returns>
        public bool IsDownloadableContentActive(string additionalContentKey)
        {
            bool returnValue = m_ActiveDownloadableContents.Contains(additionalContentKey);
            return returnValue;
        }

        /// <summary>
        /// Check if the project has downloadable content
        /// </summary>
        /// <returns>Returns True if has additional content</returns>
        public bool HasDownloadableContent()
        {
            return DownloadableContentTable.Keys.Count > 0;
        }

        /// <summary>
        /// Search all additional content and only activates the one in the table.
        /// </summary>
        public void SearchAndActiveDownloadableContent()
        {
            if (!HasDownloadableContent())
            {
                return;
            }

#if UNITY_EDITOR || (UNITY_STANDALONE && !(STANDALONE_STEAM || STANDALONE_EPIC || STANDALONE_GOG || MICROSOFT_GAME_CORE))
            ForceAllDLCsUnlock();
            return;
#else
            PlatformManager.Instance.DownloadableContetnFinder.EnumerateDLC(SearchAndActiveDownloadableContent);
#endif
        }

        private void SearchAndActiveDownloadableContent(List<string> foundSpecificKeys)
        {
            List<string> newGeneralKeys = DiscriminateDownloadableKeys(foundSpecificKeys);
            ActiveDownloadableContent(newGeneralKeys);
        }

        private List<string> DiscriminateDownloadableKeys(List<string> foundSpecificKeys)
        {
            List<string> foundGeneralKeys = new List<string>();

            Debug.Log($"[ADD_CON] Found {foundSpecificKeys.Count} specific keys");

            foreach (string specificDownloadableContentKey in foundSpecificKeys)
            {
                Debug.Log($"[ADD_CON] Found specific key: {specificDownloadableContentKey} for {PlatformManager.Instance.CurrentPlatform}");

                if (m_DownloadableContentTable.AdditionalContentExists(specificDownloadableContentKey, out string generalAdditionalContentKey))
                {
                    Debug.Log($"[ADD_CON] Found that specific key: {specificDownloadableContentKey} for {Application.platform.ToString()}" +
                            $" is the key for general key: {generalAdditionalContentKey}");
                    foundGeneralKeys.Add(generalAdditionalContentKey);
                }
            }

            return foundGeneralKeys;
        }

        /// <summary>
        /// Activates new found keys, deactivates the unfounded old keys.
        /// </summary>
        /// <param name="newDownloadableContentKeys"></param>
        private void ActiveDownloadableContent(List<string> newDownloadableContentKeys)
        {
            Log($"[ADD_CON] START_KEY_ACT_DEACT, newAdditionalContentKeys count is : {newDownloadableContentKeys.Count}");

            if (newDownloadableContentKeys.Count <= 0) return;

            foreach (string oldKey in m_ActiveDownloadableContents)
            {
                if(!newDownloadableContentKeys.Contains(oldKey))
                {
                    OnDownloadableContentDeactivated?.Invoke(oldKey);
                    Debug.LogWarning($"[ADD_CON] DEACTIVATING_KEY key: {oldKey}");
                }
            }

            foreach(string newKey in newDownloadableContentKeys)
            {
                if(!m_ActiveDownloadableContents.Contains(newKey))
                {
                    OnDownloadableContentActivated?.Invoke(newKey);
                    Debug.LogWarning($"[ADD_CON] ACTIVATING_KEY key: {newKey}");
                }
            }

            m_ActiveDownloadableContents.Clear();
            m_ActiveDownloadableContents.AddRange(newDownloadableContentKeys);
            Debug.LogWarning($"[ADD_CON] END_KEY_ACT_DEACT");
            foreach (string key in m_ActiveDownloadableContents)
            {
                Debug.LogWarning($"[ADD_CON] key: {key}");
            }
        }

        /// <summary>
        /// Populate internal DLC list with all DLC's keys. USE IT WITH CAUTION!
        /// </summary>
        private void ForceAllDLCsUnlock()
        {
            m_ActiveDownloadableContents.Clear();
            foreach (var dlcKey in DownloadableContentTable.Keys)
            {
                m_ActiveDownloadableContents.Add(dlcKey);
            }
        }

        /// <summary>
        /// Populate internal DLC list with a DLC keys. USE IT WITH CAUTION!
        /// </summary>
        /// <param name="dlcID"></param>
        public void ForceDLCUnlock(string dlcID)
        {
            if (m_ActiveDownloadableContents.Contains(dlcID))
            {
                Debug.Log("DLC Already active.");
                return;
            }

            foreach (var dlcKey in DownloadableContentTable.Keys)
            {
                if(dlcKey == dlcID)
                {
                    m_ActiveDownloadableContents.Add(dlcKey);
                    return;
                }
            }
            return;
        }

#endregion

#region UTILS
        private void Log(string msg)
        {
            Debug.Log($"[AdditionalContentManager]{msg}");
        }
#endregion
    }
}
