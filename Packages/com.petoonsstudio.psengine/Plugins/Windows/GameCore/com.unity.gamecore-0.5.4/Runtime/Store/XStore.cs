using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public delegate void XStoreShowProductPageUICompleted(Int32 hresult);
    public delegate void XStoreShowAssociatedProductsUICompleted(Int32 hresult);
    public delegate void XStoreAcquireLicenseForDurablesCompleted(Int32 hresult, XStoreLicense license);

    public class XStoreContext
    {
        internal XStoreContextHandle handle { get; set; }
    }

    public partial class SDK
    {
        public static Int32 XStoreCreateContext(out XStoreContext storeContext)
        {
            return XStoreCreateContext(null, out storeContext);
        }

        public static Int32 XStoreCreateContext(XUserHandle user, out XStoreContext storeContext)
        {
            storeContext = null;
            XStoreContextHandle context;
            Int32 hr = XGRInterop.XStoreCreateContext(user == null ? new Interop.XUserHandle() : user.InteropHandle, out context);
            if (HR.SUCCEEDED(hr))
            {
                storeContext = new XStoreContext { handle = context };
            }

            return hr;
        }

        public static void XStoreCloseContextHandle(XStoreContext context)
        {
            if (context == null)
            {
                return;
            }

            XGRInterop.XStoreCloseContextHandle(context.handle);
        }

        public static Int32 XStoreShowProductPageUIAsync(
            XStoreContext storeContextHandle,
            string storeId,
            XStoreShowProductPageUICompleted completionRoutine
            )
        {
            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                // extract result
                Int32 hresult = XGRInterop.XStoreShowProductPageUIResult(block);
                completionRoutine(hresult);
            });

            Int32 hr = XGRInterop.XStoreShowProductPageUIAsync(
                storeContextHandle.handle,
                Converters.StringToNullTerminatedUTF8ByteArray(storeId),
                asyncBlock);

            if (HR.FAILED(hr))
            {
                AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                completionRoutine(hr);
            }

            return hr;
        }

        public static Int32 XStoreShowAssociatedProductsUIAsync(
            XStoreContext storeContextHandle,
            string storeId,
            XStoreProductKind productKinds,
            XStoreShowAssociatedProductsUICompleted completionRoutine
            )
        {
            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                // extract result
                Int32 hresult = XGRInterop.XStoreShowAssociatedProductsUIResult(block);
                completionRoutine(hresult);
            });

            Int32 hr = XGRInterop.XStoreShowAssociatedProductsUIAsync(
                storeContextHandle.handle,
                Converters.StringToNullTerminatedUTF8ByteArray(storeId),
                productKinds,
                asyncBlock);

            if (HR.FAILED(hr))
            {
                AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                completionRoutine(hr);
            }

            return hr;
        }

        public static bool XStoreIsAvailabilityPurchasable(
            XStoreAvailability availability
            )
        {
            using (DisposableCollection disposableCollection = new DisposableCollection())
            {
                NativeBool ret = XGRInterop.XStoreIsAvailabilityPurchasable(new Interop.XStoreAvailability(availability, disposableCollection));

                return ret.Value;
            }
        }

        public static void XStoreAcquireLicenseForDurablesAsync(
            XStoreContext storeContextHandle,
            string storeId,
            XStoreAcquireLicenseForDurablesCompleted completionRoutine
            )
        {
            if (storeContextHandle == null)
            {
                completionRoutine(HR.E_INVALIDARG, null);
                return;
            }

            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                Int32 hresult = XGRInterop.XStoreAcquireLicenseForDurablesResult(block, out XStoreLicenseHandle handle);
                completionRoutine(hresult, new XStoreLicense(handle));
            });

            Int32 hr = XGRInterop.XStoreAcquireLicenseForDurablesAsync(storeContextHandle.handle, Converters.StringToNullTerminatedUTF8ByteArray(storeId), asyncBlock);

            if (HR.FAILED(hr))
            {
                AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                completionRoutine(hr, null);
            }
        }
    }
}
