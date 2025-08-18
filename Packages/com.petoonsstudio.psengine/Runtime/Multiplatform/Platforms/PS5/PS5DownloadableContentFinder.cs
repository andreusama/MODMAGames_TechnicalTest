using System;
using System.Collections.Generic;

#if UNITY_PS5
using Unity.PSN.PS5.Aysnc;
using Unity.PSN.PS5.Entitlement;
using UnityEngine;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform.PS5
{
    public class PS5DownloadableContentFinder
#if UNITY_PS5
        : IDownloadableContentFinder
#endif
    {
#if UNITY_PS5
        private Action<List<string>> m_EnumerateDLCCallback;
        public static Dictionary<string, string> DLCMountPaths = new Dictionary<string, string>();

        public void EnumerateDLC(Action<List<string>> callback)
        {
            m_EnumerateDLCCallback = callback;
            GetOwnedAdditionalContentList(true);
        }

        /// <summary>
        /// Generates a list of active additional content Entitlement keys and returns it in callback.
        /// </summary>
        /// <param name="getMountedOnly">true: The list will only include successfully mounted dlc. false: The list will include all owned Entitlements, mounted or not.</param>
        public void GetOwnedAdditionalContentList(bool getMountedOnly)
        {
            Entitlements.GetAdditionalContentEntitlementListRequest request = new Entitlements.GetAdditionalContentEntitlementListRequest()
            {
                ServiceLabel = 0
            };

            var requestOp = new AsyncRequest<Entitlements.GetAdditionalContentEntitlementListRequest>(request).ContinueWith((antecedent) =>
            {
                if (PS5Manager.CheckAysncRequestOK(antecedent))
                {
                    LogAndHandleList(antecedent.Request.Entitlements, getMountedOnly);
                }
            });

            Debug.Log("[ADD_CON] Getting additional content entitlements...");

            Entitlements.Schedule(requestOp);
        }

        private void LogAndHandleList(Entitlements.AdditionalContentEntitlementInfo[] entitlements, bool getMountedOnly)
        {
            Debug.Log($"[ADD_CON] Aditional Content Entitlements (Only includes mounted: {getMountedOnly}): ");

            if (entitlements == null)
            {
                Debug.Log("[ADD_CON] No entitlements where found.");
            }
            else
            {
                List<string> returnKeys = new List<string>();
                DLCMountPaths.Clear();

                for (int i = 0; i < entitlements.Length; i++)
                {
                    Debug.Log("[ADD_CON] EntitlementLabel: " + entitlements[i].EntitlementLabel + " / PackageType: " + entitlements[i].PackageType + "DownloadStatus: " + entitlements[i].DownloadStatus);

                    if (entitlements[i].DownloadStatus == Entitlements.EntitlementAccessDownloadStatus.Installed
                        && entitlements[i].PackageType == Entitlements.EntitlementAccessPackageType.PSAC)
                    {
                        var mountPointPath = MountDLC(entitlements[i].EntitlementLabel);

                        Debug.Log($"[ADD_CON] PSAC content was mounted to addcont path: {mountPointPath}");

                        if (getMountedOnly)
                        {
                            returnKeys.Add(entitlements[i].EntitlementLabel);
                            DLCMountPaths.Add(entitlements[i].EntitlementLabel, mountPointPath);
                        }
                    }

                    if (!getMountedOnly) returnKeys.Add(entitlements[i].EntitlementLabel);
                }

                m_EnumerateDLCCallback.Invoke(returnKeys);
            }
        }

        public static string MountDLC(string entitlementLabel, int serviceLabel = 0)
        {
            if (UnityEngine.PS5.PS5DRM.ContentOpen(entitlementLabel, serviceLabel) == true)
            {
                string mountPointPath = "";
                if (UnityEngine.PS5.PS5DRM.ContentGetMountPoint(entitlementLabel, serviceLabel, out mountPointPath) == true)
                {
                    return mountPointPath;
                }
            }
            return null;
        }
#endif
    }
}