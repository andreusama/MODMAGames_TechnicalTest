using System.Collections.Generic;
using UnityEngine;
using System;

#if STANDALONE_STEAM
using Steamworks;

namespace PetoonsStudio.PSEngine.Multiplatform
{
    public class SteamDownloadableContentFinder : IDownloadableContentFinder
    {
        public void EnumerateDLC(Action<List<string>> callback)
        {
            Debug.Log("[ADD_CON] Start enumarating keys");
            List<string> returnKeys = new List<string>();

            int dlcCount = SteamApps.GetDLCCount();

            Debug.Log($"[ADD_CON] Specific keys count is {dlcCount}");
            EnumerateInstalledDLCs(returnKeys, dlcCount);

            callback?.Invoke(returnKeys);
        }

        private void EnumerateInstalledDLCs(List<string> returnKeys, int dlcCount)
        {
            for (int i = 0; i < dlcCount; i++)
            {
                var success = SteamApps.BGetDLCDataByIndex(i, out AppId_t pAppId, out bool pbAvailable, out string pchName, 512);
                if (success)
                {
                    if (SteamApps.BIsDlcInstalled(pAppId))
                    {
                        Debug.Log($"[ADD_CON] Found specific key: {pAppId}");
                        returnKeys.Add(pAppId.m_AppId.ToString());
                    }
                    else
                    {
                        Debug.Log($"[ADD_CON] DLC:{pAppId.m_AppId.ToString()} is not installed.");
                    }
                }
                else
                {
                    Debug.Log($"[ADD_CON] Couldn't retrieve information from DLC.");
                }
            }
        }
    }
}
#endif
