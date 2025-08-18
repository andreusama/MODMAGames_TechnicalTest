using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public delegate void XStoreQueryComplete(Int32 hresult, XStoreQueryResult result);
    internal delegate Int32 QueryExtractionFunction(XAsyncBlockPtr asyncBlock, out XStoreProductQueryHandle queryHandle);

    partial class SDK
    {
        #region Callbacks
        [MonoPInvokeCallback]
        private static NativeBool ProductQueryCallback(IntPtr product, IntPtr context)
        {
            Interop.XStoreProduct p = (Interop.XStoreProduct)Marshal.PtrToStructure(product, typeof(Interop.XStoreProduct));
            GCHandle productsHandle = GCHandle.FromIntPtr(context);
            var products = productsHandle.Target as List<XStoreProduct>;
            products.Add(new XStoreProduct(p));
            return new NativeBool(true);
        }
        #endregion

        static Int32 RetrieveQueryProducts(XStoreProductQueryHandle queryPage, out XStoreProduct[] result)
        {
            List<XStoreProduct> ret = new List<XStoreProduct>();
            GCHandle retHandle = GCHandle.Alloc(ret);

            Int32 hr = XGRInterop.XStoreEnumerateProductsQuery(queryPage, GCHandle.ToIntPtr(retHandle), ProductQueryCallback);
            result = ret.ToArray();
            retHandle.Free();
            return hr;
        }

        private static void ExtractQueryResultAndComplete(XStoreQueryComplete completionRoutine, XAsyncBlockPtr block, QueryExtractionFunction extractionFunction)
        {
            XStoreQueryResult result = null;
            XStoreProductQueryHandle queryHandle;
            Int32 hresult = extractionFunction(block, out queryHandle);
            if (HR.SUCCEEDED(hresult))
            {
                bool hasMorePages = XGRInterop.XStoreProductsQueryHasMorePages(queryHandle);

                XStoreProduct[] pageItems = null;
                hresult = RetrieveQueryProducts(queryHandle, out pageItems);
                result = new XStoreQueryResult(queryHandle, pageItems, hasMorePages);
            }
            completionRoutine(hresult, result);
        }

        public static void XStoreQueryAssociatedProductsAsync(XStoreContext context, XStoreProductKind productKinds, UInt32 maxItemsToRetrievePerPage, XStoreQueryComplete completionRoutine)
        {
            if (context == null)
            {
                completionRoutine(HR.E_INVALIDARG, null);
                return;
            }

            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                ExtractQueryResultAndComplete(completionRoutine, block, XGRInterop.XStoreQueryAssociatedProductsResult);
            });

            Int32 hr = XGRInterop.XStoreQueryAssociatedProductsAsync(context.handle, productKinds, maxItemsToRetrievePerPage, asyncBlock);

            if (HR.FAILED(hr))
            {
                AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                completionRoutine(hr, null);
            }
        }

        public static void XStoreQueryEntitledProductsAsync(XStoreContext context, XStoreProductKind productKinds, UInt32 maxItemsToRetrievePerPage, XStoreQueryComplete completionRoutine)
        {
            if (context == null)
            {
                completionRoutine(HR.E_INVALIDARG, null);
                return;
            }

            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                ExtractQueryResultAndComplete(completionRoutine, block, XGRInterop.XStoreQueryEntitledProductsResult);
            });

            Int32 hr = XGRInterop.XStoreQueryEntitledProductsAsync(context.handle, productKinds, maxItemsToRetrievePerPage, asyncBlock);

            if (HR.FAILED(hr))
            {
                AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                completionRoutine(hr, null);
            }
        }

        public static void XStoreQueryProductForCurrentGameAsync(XStoreContext context, XStoreQueryComplete completionRoutine)
        {
            if (context == null)
            {
                completionRoutine(HR.E_INVALIDARG, null);
                return;
            }

            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                ExtractQueryResultAndComplete(completionRoutine, block, XGRInterop.XStoreQueryProductForCurrentGameResult);
            });

            Int32 hr = XGRInterop.XStoreQueryProductForCurrentGameAsync(context.handle, asyncBlock);

            if (HR.FAILED(hr))
            {
                AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                completionRoutine(hr, null);
            }
        }

        public static void XStoreQueryProductForPackageAsync(XStoreContext context, XStoreProductKind productKinds, string packageIdentifier, XStoreQueryComplete completionRoutine)
        {
            if (context == null)
            {
                completionRoutine(HR.E_INVALIDARG, null);
                return;
            }

            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                ExtractQueryResultAndComplete(completionRoutine, block, XGRInterop.XStoreQueryProductForPackageResult);
            });

            Int32 hr = XGRInterop.XStoreQueryProductForPackageAsync(context.handle, productKinds, Converters.StringToNullTerminatedUTF8ByteArray(packageIdentifier), asyncBlock);

            if (HR.FAILED(hr))
            {
                AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                completionRoutine(hr, null);
            }
        }

        public static void XStoreQueryProductsAsync(
            XStoreContext context,
            XStoreProductKind productKinds,
            string[] storeIds,
            string[] actionFilters,
            XStoreQueryComplete completionRoutine)
        {
            if (context == null)
            {
                completionRoutine(HR.E_INVALIDARG, null);
                return;
            }

            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                ExtractQueryResultAndComplete(completionRoutine, block, XGRInterop.XStoreQueryProductsResult);
            });

            using (DisposableBuffer storeIdsBuffer = Converters.StringArrayToUTF8StringArray(storeIds))
            using (DisposableBuffer actionFiltersBuffer = Converters.StringArrayToUTF8StringArray(actionFilters))
            {
                Int32 hr = XGRInterop.XStoreQueryProductsAsync(context.handle, productKinds, storeIdsBuffer.IntPtr, new SizeT(storeIds?.Length ?? 0), actionFiltersBuffer.IntPtr, new SizeT(actionFilters?.Length ?? 0), asyncBlock);

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr, null);
                }
            }
        }

        public static void XStoreProductsQueryNextPageAsync(XStoreQueryResult currentPage, XStoreQueryComplete completionRoutine)
        {
            if (currentPage == null)
            {
                completionRoutine(HR.E_INVALIDARG, null);
                return;
            }

            XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
            {
                ExtractQueryResultAndComplete(completionRoutine, block, XGRInterop.XStoreProductsQueryNextPageResult);
            });

            Int32 hr = XGRInterop.XStoreProductsQueryNextPageAsync(currentPage.QueryHandle, asyncBlock);

            if (HR.FAILED(hr))
            {
                AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                completionRoutine(hr, null);
            }
        }

        public static void XStoreCloseProductsQueryHandle(XStoreQueryResult result)
        {
            XGRInterop.XStoreCloseProductsQueryHandle(result.QueryHandle);
        }
    }
}
