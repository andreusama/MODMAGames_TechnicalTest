using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

#if UNITY_GAMECORE
using Unity.GameCore;

namespace PetoonsStudio.PSEngine.Multiplatform.GameCore
{
    public class GameCoreDownloadableContentFinder : IDownloadableContentFinder
    {
        public static Dictionary<string, string> DLCMountPaths = new Dictionary<string, string>();

        public void EnumerateDLC(Action<List<string>> callback)
        {
            AdditionalContentManager.Instance.StartCoroutine(EnumerateDLCIntern(callback));
        }

        private IEnumerator EnumerateDLCIntern(Action<List<string>> callback)
        {
            List<string> returnKeys = new List<string>();
            int hResult = GameCoreOperationResults.Invalid;
            hResult = SDK.XPackageEnumeratePackages(XPackageKind.Content, XPackageEnumerationScope.ThisAndRelated, out XPackageDetails[] packageDetails);

            if (GameCoreOperationResults.IsInvalid(hResult))
            {
                Debug.Log($"[ADD_CON] Couldn't enumerate packages:{GameCoreOperationResults.GetName(hResult)}");
                callback?.Invoke(returnKeys);
                yield break;
            }

            if (GameCoreManager.Instance.XboxUser.XboxUserHandle == null)
            {
                Debug.Log($"[ADD_CON] No user logged, can't retrieve information because UserHandler is null.");
                callback?.Invoke(returnKeys);
                yield break;
            }

            GameCoreManager.Instance.CloseValidStoreLicenses();
            DLCMountPaths.Clear();

            Debug.Log($"[ADD_CON] Found {packageDetails.Length} packages for enumerate.");
            foreach (var package in packageDetails)
            {
                Debug.Log($"[ADD_CON] PackageStoreID: {package.StoreId} ");
                hResult = GameCoreOperationResults.Invalid;
                XStoreLicense myStoreLicene = null;

                SDK.XStoreAcquireLicenseForPackageAsync(GameCoreManager.Instance.XboxUser.StoreContext,
                                                            package.PackageIdentifier, (result, license) =>
                                                            {
                                                                hResult = result;
                                                                myStoreLicene = license;
                                                            });

                while (GameCoreOperationResults.IsInvalid(hResult))
                {
                    yield return null;
                }

                if (HR.FAILED(hResult))
                {
                    Debug.Log($"[ADD_CON] Couldn't adcquire License for the package:{GameCoreOperationResults.GetName(hResult)}");
                    continue;
                }

                if (SDK.XStoreIsLicenseValid(myStoreLicene))
                {
                    Debug.Log($"[ADD_CON] Has license for: {package.StoreId}, name: {package.DisplayName} ");
                    GameCoreManager.Instance.UserController.AddStoreLicenseToCurrentUser(package.StoreId, myStoreLicene);

                    if (MountDLC(package, "DLCContent"))
                    {
                        returnKeys.Add(package.StoreId);
                    }
                }
                else
                {
                    Debug.Log($"[ADD_CON] License not valid for package. ");
                    if (myStoreLicene != null)
                        SDK.XStoreCloseLicenseHandle(myStoreLicene);
                }
            }

            callback?.Invoke(returnKeys);
        }

        XPackageMountHandle mountHandle = null;

        private bool MountDLC(XPackageDetails packageDetails, string dlcPath)
        {
            var hr = SDK.XPackageMount(packageDetails.PackageIdentifier, out XPackageMountHandle newMountHandle);
            if (HR.SUCCEEDED(hr))
            {
                mountHandle = newMountHandle;

                string mountPath;
                hr = SDK.XPackageGetMountPath(mountHandle, out mountPath);
                if (HR.SUCCEEDED(hr))
                {
                    DLCMountPaths.Add(packageDetails.StoreId, mountPath);
                    return true;
                }
                else
                {
                    Debug.Log($"[ADD_CON] Error retrieving mount path: {packageDetails.PackageIdentifier}");
                }
            }
            else
            {
                Debug.Log($"[ADD_CON] Error mounting package {packageDetails.PackageIdentifier} / {GameCoreOperationResults.GetName(hr)}");
            }

            return false;
            //TO CHECK if it's really needed
            //SDK.XPackageCloseMountHandle(mountHandle);
        }
    }
}
#endif