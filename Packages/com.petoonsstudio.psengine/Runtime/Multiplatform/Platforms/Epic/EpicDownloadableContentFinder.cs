using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PetoonsStudio.PSEngine.Framework;

#if STANDALONE_EPIC
using Epic.OnlineServices.Ecom;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform
{
#if STANDALONE_EPIC
    public class EpicDownloadableContentFinder : IDownloadableContentFinder
    {
        public void EnumerateDLC(Action<List<string>> callback)
        {
            Debug.Log("[ADD_CON] Start enumarating keys");
            EnumerateOwnedDLCs(callback);
        }

        private void EnumerateOwnedDLCs(Action<List<string>> callback)
        {
            List<string> returnKeys = new List<string>();
            if (EpicManager.Instance != null)
            {
                Epic.OnlineServices.Utf8String[] arrayKeys = new Epic.OnlineServices.Utf8String[AdditionalContentManager.Instance.DownloadableContentTable.GetEntryCount()];
                int contKeys = 0;
                foreach (var keys in AdditionalContentManager.Instance.DownloadableContentTable.Keys)
                {
                    var dlc = AdditionalContentManager.Instance.GetDownloadableContent(keys);
                    arrayKeys[contKeys] = dlc[Platform.Epic];
                    contKeys++;
                }
                var epicInterface = EpicManager.Instance.GetEOSEcomInterface();
                var param = new QueryOwnershipOptions()
                {
                    LocalUserId = EpicManager.Instance.GetLocalUserId(),
                    CatalogItemIds = arrayKeys
                };

                epicInterface.QueryOwnership(ref param, null, (ref QueryOwnershipCallbackInfo data) =>
                {
                    if (data.ResultCode == Epic.OnlineServices.Result.Success)
                    {
                        foreach (var item in data.ItemOwnership)
                        {
                            if (item.OwnershipStatus == OwnershipStatus.Owned)
                            {
                                Debug.Log($"[ADD_CON] Epic DLC: {item.Id} is owned.");
                                returnKeys.Add(item.Id.ToString());
                            }
                            else
                            {
                                Debug.Log($"[ADD_CON] Epic DLC: {item.Id} is not owned.");
                            }
                        }
                    }
                    else
                    {
                        Debug.Log($"[ADD_CON] Error not ownership of Epic DLC with result: {data.ResultCode}");
                    }
                    callback?.Invoke(returnKeys);
                });
            }
        }
    }
#endif
}