using System;
using System.Runtime.InteropServices;
using System.Text;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    partial class SDK
    {
        public delegate void XStoreQueryGameAndDlcPackageUpdatesCompleted(Int32 hresult, XStorePackageUpdate[] packageUpdates);
        public delegate void XStoreDownloadAndInstallPackagesCompleted(Int32 hresult, string[] packageIdentifiers);
        public delegate void XStoreDownloadAndInstallPackageUpdatesCompleted(Int32 hresult);
        public delegate void XStoreDownloadPackageUpdatesCompleted(Int32 hresult);

        public static void XStoreQueryGameAndDlcPackageUpdatesAsync(XStoreContext context, XStoreQueryGameAndDlcPackageUpdatesCompleted completionRoutine)
        {
            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                // extract result
                XStorePackageUpdate[] packageUpdates = null;
                UInt32 count;
                Int32 hresult = XGRInterop.XStoreQueryGameAndDlcPackageUpdatesResultCount(block, out count);
                if (hresult == 0)
                {
                    if (count > 0)
                    {
                        Int32 size = Marshal.SizeOf(new Interop.XStorePackageUpdate());
                        IntPtr memPnt = Marshal.AllocHGlobal((Int32)(size * count));
                        hresult = XGRInterop.XStoreQueryGameAndDlcPackageUpdatesResult(block, count, memPnt);
                        if (hresult == 0)
                        {
                            packageUpdates = Converters.PtrToClassArray<XStorePackageUpdate, Interop.XStorePackageUpdate>(memPnt, count, x => new XStorePackageUpdate(x));
                        }
                        Marshal.FreeHGlobal(memPnt);
                    }
                }
                completionRoutine(hresult, packageUpdates);
            });

            Int32 hr = XGRInterop.XStoreQueryGameAndDlcPackageUpdatesAsync(context.handle, asyncBlock);

            if (HR.FAILED(hr))
            {
                AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                completionRoutine(hr, null);
            }
        }

        public static void XStoreDownloadAndInstallPackagesAsync(XStoreContext context, string[] storeIds, XStoreDownloadAndInstallPackagesCompleted completionRoutine)
        {
            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                // extract result
                string[] packageIdentifiers = null;
                UInt32 count;
                Int32 hresult = XGRInterop.XStoreDownloadAndInstallPackagesResultCount(block, out count);
                if (hresult == 0)
                {
                    packageIdentifiers = new string[count];

                    if (count > 0)
                    {
                        Byte[] temp = new Byte[count * XGRInterop.XPACKAGE_IDENTIFIER_MAX_LENGTH];
                        hresult = XGRInterop.XStoreDownloadAndInstallPackagesResult(block, count, temp);

                        for (Int32 index = 0; index < count; index++)
                        {
                            packageIdentifiers[index] = Converters.ByteArrayToString(temp, index * XGRInterop.XPACKAGE_IDENTIFIER_MAX_LENGTH, XGRInterop.XPACKAGE_IDENTIFIER_MAX_LENGTH);
                        }
                    }
                }
                completionRoutine(hresult, packageIdentifiers);
            });

            using (DisposableBuffer buffer = Converters.StringArrayToUTF8StringArray(storeIds))
            {
                Int32 hr = XGRInterop.XStoreDownloadAndInstallPackagesAsync(context.handle, buffer.IntPtr, new SizeT(storeIds?.Length ?? 0), asyncBlock);

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr, null);
                }
            }
        }

        public static void XStoreDownloadAndInstallPackageUpdatesAsync(XStoreContext context, string[] packageIdentifiers, XStoreDownloadAndInstallPackageUpdatesCompleted completionRoutine)
        {
            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                // extract result
                Int32 hresult = XGRInterop.XStoreDownloadAndInstallPackageUpdatesResult(block);
                completionRoutine(hresult);
            });

            using (DisposableBuffer buffer = Converters.StringArrayToUTF8StringArray(packageIdentifiers))
            {
                Int32 hr = XGRInterop.XStoreDownloadAndInstallPackageUpdatesAsync(context.handle, buffer.IntPtr, new SizeT(packageIdentifiers?.Length ?? 0), asyncBlock);

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr);
                }
            }
        }

        public static void XStoreDownloadPackageUpdatesAsync(XStoreContext context, string[] packageIdentifiers, XStoreDownloadPackageUpdatesCompleted completionRoutine)
        {
            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                // extract result
                Int32 hresult = XGRInterop.XStoreDownloadPackageUpdatesResult(block);
                completionRoutine(hresult);
            });

            using (DisposableBuffer buffer = Converters.StringArrayToUTF8StringArray(packageIdentifiers))
            {
                Int32 hr = XGRInterop.XStoreDownloadPackageUpdatesAsync(context.handle, buffer.IntPtr, new SizeT(packageIdentifiers?.Length ?? 0), asyncBlock);

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr);
                }
            }
        }

        public static Int32 XStoreQueryPackageIdentifier(string storeId, out string packageIdentifier)
        {
            packageIdentifier = null;

            Byte[] data = new Byte[XGRInterop.XPACKAGE_IDENTIFIER_MAX_LENGTH];
            Int32 hr = XGRInterop.XStoreQueryPackageIdentifier(Converters.StringToNullTerminatedUTF8ByteArray(storeId), new SizeT(data.Length), data);

            if (HR.SUCCEEDED(hr))
            {
                packageIdentifier = Converters.ByteArrayToString(data);
            }

            return hr;
        }
    }
}
