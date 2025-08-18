using UnityEngine;
using System.Collections.Generic;
using System.IO;

#if UNITY_PS4
using UnityEngine.PS4;

namespace PetoonsStudio.PSEngine.Multiplatform.PS4
{
    public class PS4DownloadableContentFinder : IDownloadableContentFinder
    {
        public static Dictionary<string, string> DLCMountPaths = new Dictionary<string, string>();
        private const string ADDITIONALCONTENT_MOUNT_BASE = "/addcont";

        public void EnumerateDLC(System.Action<List<string>> callback)
        {
            Debug.Log("[ADD_CON]Starting search for additional content");

            List<string> dlcsKeys = new List<string>();
            dlcsKeys = EnumerateDRMContent(0); // service label 0 indicates the current title
            //EnumerateDRMContent(1); // use service label 1 to get installed content from a different product

            callback.Invoke(dlcsKeys);
        }

        /// <summary>
        /// Searches for additional content in the specific platform
        /// </summary>
        /// <param name="serviceLabel"></param>
        /// <returns></returns>
        private List<string> EnumerateDRMContent(int serviceLabel)
        {
            List<string> returnKeys = new List<string>();
            int currentAddCont = 0;
            DLCMountPaths.Clear();

            PS4DRM.DrmContentFinder finder = new PS4DRM.DrmContentFinder();
            finder.serviceLabel = serviceLabel;
            bool found = false;
            if (PS4DRM.ContentFinderOpen(ref finder))
            {
                Debug.Log("[ADD_CON]Finder opened");

                found = true;
                EnumerateDRMContentFiles(finder.entitlementLabel, finder.serviceLabel);
                returnKeys.Add(finder.entitlementLabel);
                DLCMountPaths.Add(finder.entitlementLabel, GetMountPathName(currentAddCont));
                Debug.Log("[ADD_CON]Entitlement label = " + finder.entitlementLabel);

                while (PS4DRM.ContentFinderNext(ref finder))
                {
                    currentAddCont += 1;
                    EnumerateDRMContentFiles(finder.entitlementLabel, finder.serviceLabel);
                    DLCMountPaths.Add(finder.entitlementLabel, GetMountPathName(currentAddCont));
                    returnKeys.Add(finder.entitlementLabel);
                    Debug.Log("[ADD_CON]Entitlement label = " + finder.entitlementLabel);
                };
                PS4DRM.ContentFinderClose(ref finder);
            }
            if (!found)
            {
                Debug.Log("[ADD_CON]No content found");
            }

            return returnKeys;
        }

        private string GetMountPathName(int currentAddCont)
        {
            return ADDITIONALCONTENT_MOUNT_BASE + currentAddCont;
        }

        /// <summary>
        /// Dowload the files. WARNING, could throw an error if the have not been marked the size of additional content in params
        /// </summary>
        /// <param name="entitlementLabel"></param>
        /// <param name="serviceLabel"></param>
        private void EnumerateDRMContentFiles(string entitlementLabel, int serviceLabel)
        {
            Debug.Log("[ADD_CON]Entitlement label = " + entitlementLabel);

            if (PS4DRM.ContentOpen(entitlementLabel, serviceLabel) == true)
            {
                string mountPoint;

                if (PS4DRM.ContentGetMountPoint(entitlementLabel, serviceLabel, out mountPoint) == true)
                {
                    string filePath = mountPoint;

                    Debug.Log("[ADD_CON]Found content folder: " + filePath);

                    string[] files = Directory.GetFiles(filePath);
                    Debug.Log("[ADD_CON]containing " + files.Length + " files");
                    foreach (string file in files)
                    {
                        Debug.Log("[ADD_CON]  " + file);
                        if (file.Contains(".unity3d"))
                        {
                            AssetBundle bundle = AssetBundle.LoadFromFile(file);

                            Object[] assets = bundle.LoadAllAssets();
                            Debug.Log("[ADD_CON]  Loaded " + assets.Length + " assets from asset bundle.");

                            bundle.Unload(false);
                        }
                    }
                }
                else
                {
                    Debug.Log("[ADD_CON]Can't mount entitlement");
                }

                PS4DRM.ContentClose(entitlementLabel);
            }
            else
            {
                Debug.Log("[ADD_CON]Can't open entitlement");
            }
        }
    }
}
#endif