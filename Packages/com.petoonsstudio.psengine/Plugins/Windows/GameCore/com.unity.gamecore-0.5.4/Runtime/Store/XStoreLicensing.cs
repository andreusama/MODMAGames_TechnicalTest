using System;
using System.Runtime.InteropServices;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public delegate void XStoreAcquireLicenseForPackageCompleted(Int32 hresult, XStoreLicense license);
    public delegate void XStoreCanAcquireLicenseForPackageCompleted(Int32 hresult, XStoreCanAcquireLicenseResult result);
    public delegate void XStoreCanAcquireLicenseForStoreIdCompleted(Int32 hresult, XStoreCanAcquireLicenseResult result);
    public delegate void XStoreQueryAddOnLicensesCompleted(Int32 hresult, XStoreAddonLicense[] licenses);
    public delegate void XStoreQueryGameLicenseCompleted(Int32 hresult, XStoreGameLicense license);
    public delegate void XStoreQueryLicenseTokenCompleted(Int32 hresult, string token);

    public delegate void XStoreGameLicenseChangedCallback();
    public delegate void XStorePackageLicenseLostCallback();

    partial class SDK
    {
        #region Callbacks
        [MonoPInvokeCallback]
        private static void LicenseChangedCallback(IntPtr context)
        {
            GCHandle cbHandle = GCHandle.FromIntPtr(context);
            var callbacks = cbHandle.Target as UnmanagedCallback<Interop.XStoreGameLicenseChangedCallback, XStoreGameLicenseChangedCallback>;
            callbacks.userCallback?.Invoke();
        }

        [MonoPInvokeCallback]
        private static void LicenseLostCallback(IntPtr context)
        {
            GCHandle cbHandle = GCHandle.FromIntPtr(context);
            var callbacks = cbHandle.Target as UnmanagedCallback<Interop.XStorePackageLicenseLostCallback, XStorePackageLicenseLostCallback>;
            callbacks.userCallback?.Invoke();
        }

        #endregion

        public static void XStoreAcquireLicenseForPackageAsync(XStoreContext context, string packageIdentifier, XStoreAcquireLicenseForPackageCompleted completionRoutine)
        {
            if (context == null)
            {
                completionRoutine(HR.E_INVALIDARG, null);
                return;
            }

            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                Int32 hresult = XGRInterop.XStoreAcquireLicenseForPackageResult(block, out XStoreLicenseHandle handle);
                completionRoutine(hresult, new XStoreLicense(handle));
            });

            Int32 hr = XGRInterop.XStoreAcquireLicenseForPackageAsync(context.handle, Converters.StringToNullTerminatedUTF8ByteArray(packageIdentifier), asyncBlock);

            if (HR.FAILED(hr))
            {
                AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                completionRoutine(hr, null);
            }
        }

        public static void XStoreCanAcquireLicenseForPackageAsync(XStoreContext context, string packageIdentifier, XStoreCanAcquireLicenseForPackageCompleted completionRoutine)
        {
            if (context == null)
            {
                completionRoutine(HR.E_INVALIDARG, null);
                return;
            }

            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                Int32 hresult = XGRInterop.XStoreCanAcquireLicenseForPackageResult(block, out Interop.XStoreCanAcquireLicenseResult result);
                completionRoutine(hresult, new XStoreCanAcquireLicenseResult(result));
            });

            Int32 hr = XGRInterop.XStoreCanAcquireLicenseForPackageAsync(context.handle, Converters.StringToNullTerminatedUTF8ByteArray(packageIdentifier), asyncBlock);
            
            if (HR.FAILED(hr))
            {
                AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                completionRoutine(hr, null);
            }
        }

        public static void XStoreCanAcquireLicenseForStoreIdAsync(XStoreContext context, string storeProductId, XStoreCanAcquireLicenseForStoreIdCompleted completionRoutine)
        {
            if (context == null)
            {
                completionRoutine(HR.E_INVALIDARG, null);
                return;
            }

            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                Int32 hresult = XGRInterop.XStoreCanAcquireLicenseForStoreIdResult(block, out Interop.XStoreCanAcquireLicenseResult result);
                completionRoutine(hresult, new XStoreCanAcquireLicenseResult(result));
            });

            Int32 hr = XGRInterop.XStoreCanAcquireLicenseForStoreIdAsync(context.handle, Converters.StringToNullTerminatedUTF8ByteArray(storeProductId), asyncBlock);

            if (HR.FAILED(hr))
            {
                AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                completionRoutine(hr, null);
            }
        }

        public static void XStoreCloseLicenseHandle(XStoreLicense license)
        {
            if (license == null)
            {
                return;
            }

            XGRInterop.XStoreCloseLicenseHandle(license.Handle);
            license.Handle = new XStoreLicenseHandle();
        }

        public static bool XStoreIsLicenseValid(XStoreLicense license)
        {
            if (license == null)
            {
                return false;
            }

            return XGRInterop.XStoreIsLicenseValid(license.Handle);
        }

        public static void XStoreQueryAddOnLicensesAsync(XStoreContext context, XStoreQueryAddOnLicensesCompleted completionRoutine)
        {
            if (context == null)
            {
                completionRoutine(HR.E_INVALIDARG, null);
                return;
            }

            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                Int32 hresult = XGRInterop.XStoreQueryAddOnLicensesResultCount(block, out UInt32 count);

                if (HR.FAILED(hresult))
                {
                    completionRoutine(hresult, null);
                    return;
                }

                IntPtr licenses = Marshal.AllocHGlobal((Int32)count * Marshal.SizeOf(new Interop.XStoreAddonLicense()));
                hresult = XGRInterop.XStoreQueryAddOnLicensesResult(block, count, licenses);
                XStoreAddonLicense[] result = null;

                if (HR.SUCCEEDED(hresult))
                {
                    result = Converters.PtrToClassArray<XStoreAddonLicense, Interop.XStoreAddonLicense>(licenses, count, x => new XStoreAddonLicense(x));
                }

                Marshal.FreeHGlobal(licenses);
                completionRoutine(hresult, result);
            });

            Int32 hr = XGRInterop.XStoreQueryAddOnLicensesAsync(context.handle, asyncBlock);

            if (HR.FAILED(hr))
            {
                AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                completionRoutine(hr, null);
            }
        }

        public static void XStoreQueryGameLicenseAsync(XStoreContext context, XStoreQueryGameLicenseCompleted completionRoutine)
        {
            if (context == null)
            {
                completionRoutine(HR.E_INVALIDARG, null);
                return;
            }

            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                Int32 hresult = XGRInterop.XStoreQueryGameLicenseResult(block, out Interop.XStoreGameLicense license);
                completionRoutine(hresult, new XStoreGameLicense(license));
            });

            Int32 hr = XGRInterop.XStoreQueryGameLicenseAsync(context.handle, asyncBlock);

            if (HR.FAILED(hr))
            {
                AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                completionRoutine(hr, null);
            }
        }

        [Obsolete("This overload is going to be removed in a future release.")]
        public static void XStoreQueryLicenseTokenAsync(XStoreContext context, string[] productIds, UInt32 productIdsCount, string customDeveloperString, XStoreQueryLicenseTokenCompleted completionRoutine)
        {
            XStoreQueryLicenseTokenAsync(context, productIds, customDeveloperString, completionRoutine);
        }

        public static void XStoreQueryLicenseTokenAsync(XStoreContext context, string[] productIds, string customDeveloperString, XStoreQueryLicenseTokenCompleted completionRoutine)
        {
            if (context == null)
            {
                completionRoutine(HR.E_INVALIDARG, null);
                return;
            }

            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                Int32 hresult = XGRInterop.XStoreQueryLicenseTokenResultSize(block, out SizeT size);

                if (HR.FAILED(hresult))
                {
                    completionRoutine(hresult, null);
                }

                Byte[] tempResult = new Byte[size.ToUInt32()];
                hresult = XGRInterop.XStoreQueryLicenseTokenResult(block, size, tempResult);
                string token = Converters.ByteArrayToString(tempResult);
                completionRoutine(hresult, token);
            });

            using (DisposableBuffer stringBuffer = Converters.StringArrayToUTF8StringArray(productIds))
            {
                Int32 hr = XGRInterop.XStoreQueryLicenseTokenAsync(context.handle, stringBuffer.IntPtr, new SizeT(productIds?.Length ?? 0), Converters.StringToNullTerminatedUTF8ByteArray(customDeveloperString), asyncBlock);

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr, null);
                }
            }
        }

        public static Int32 XStoreRegisterGameLicenseChanged(XStoreContext context, XStoreGameLicenseChangedCallback callback, out XRegistrationToken token)
        {
            if (context == null)
            {
                token = null;
                return HR.E_INVALIDARG;
            }

            var callbacks = new UnmanagedCallback<Interop.XStoreGameLicenseChangedCallback, XStoreGameLicenseChangedCallback>
            {
                directCallback = LicenseChangedCallback,
                userCallback = callback
            };
            GCHandle cbHandle = GCHandle.Alloc(callbacks);

            Int32 hr = XGRInterop.XStoreRegisterGameLicenseChanged(
                context.handle,
                defaultQueue.handle,
                GCHandle.ToIntPtr(cbHandle),
                callbacks.directCallback,
                out XTaskQueueRegistrationToken taskQueueToken);

            if (HR.SUCCEEDED(hr))
            {
                token = new XRegistrationToken(cbHandle, taskQueueToken);
            }
            else
            {
                token = null;
                cbHandle.Free();
            }

            return hr;
        }

        public static Int32 XStoreRegisterPackageLicenseLost(XStoreLicense license, XStorePackageLicenseLostCallback callback, out XRegistrationToken token)
        {
            if (license == null)
            {
                token = null;
                return HR.E_INVALIDARG;
            }

            var callbacks = new UnmanagedCallback<Interop.XStorePackageLicenseLostCallback, XStorePackageLicenseLostCallback>
            {
                directCallback = LicenseLostCallback,
                userCallback = callback
            };
            GCHandle cbHandle = GCHandle.Alloc(callbacks);

            Int32 hr = XGRInterop.XStoreRegisterPackageLicenseLost(
                license.Handle,
                defaultQueue.handle,
                GCHandle.ToIntPtr(cbHandle),
                callbacks.directCallback,
                out XTaskQueueRegistrationToken taskQueueToken);

            if (HR.SUCCEEDED(hr))
            {
                token = new XRegistrationToken(cbHandle, taskQueueToken);
            }
            else
            {
                token = null;
                cbHandle.Free();
            }

            return hr;
        }

        public static void XStoreUnregisterGameLicenseChanged(XStoreContext context, XRegistrationToken token)
        {
            if (context == null || token == null)
            {
                return;
            }

            XGRInterop.XStoreUnregisterGameLicenseChanged(context.handle, token.Token, true);
            token.CallbackHandle.Free();
        }

        public static void XStoreUnregisterPackageLicenseLost(XStoreLicense license, XRegistrationToken token)
        {
            if (license == null || token == null)
            {
                return;
            }

            XGRInterop.XStoreUnregisterPackageLicenseLost(license.Handle, token.Token, true);
            token.CallbackHandle.Free();
        }
    }
}
