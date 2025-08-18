using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef bool CALLBACK XStoreProductQueryCallback(_In_ const XStoreProduct* product, _In_opt_ void* context);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate NativeBool XStoreProductQueryCallback(IntPtr product, IntPtr context);

    partial class XGRInterop
    {
        //STDAPI XStoreQueryAssociatedProductsAsync(
        //    _In_ const XStoreContextHandle storeContextHandle,
        //    _In_ XStoreProductKind productKinds,
        //    _In_ uint32_t maxItemsToRetrievePerPage,
        //    _Inout_ XAsyncBlock* async
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreQueryAssociatedProductsAsync(XStoreContextHandle storeContextHandle, XStoreProductKind productKinds, UInt32 maxItemsToRetrievePerPage, XAsyncBlockPtr asyncBlock);

        //STDAPI XStoreQueryAssociatedProductsResult(
        //    _Inout_ XAsyncBlock* async,
        //    _Out_ XStoreProductQueryHandle* productQueryHandle
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreQueryAssociatedProductsResult(XAsyncBlockPtr asyncBlock, [Out] out XStoreProductQueryHandle productQueryHandle);

        //STDAPI_(void) XStoreCloseProductsQueryHandle(
        //    _In_ XStoreProductQueryHandle productQueryHandle
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern void XStoreCloseProductsQueryHandle(XStoreProductQueryHandle productQueryHandle);

        //STDAPI_(bool) XStoreProductsQueryHasMorePages(
        //    _In_ const XStoreProductQueryHandle productQueryHandle
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool XStoreProductsQueryHasMorePages(XStoreProductQueryHandle productQueryHandle);

        //STDAPI XStoreEnumerateProductsQuery(
        //    _In_ const XStoreProductQueryHandle productQueryHandle,
        //    _In_opt_ void* context,
        //    _In_ XStoreProductQueryCallback* callback
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreEnumerateProductsQuery(XStoreProductQueryHandle productQueryHandle, IntPtr context, XStoreProductQueryCallback callback);

        //STDAPI XStoreProductsQueryNextPageAsync(
        //    _In_ const XStoreProductQueryHandle productQueryHandle,
        //    _Inout_ XAsyncBlock* async
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreProductsQueryNextPageAsync(XStoreProductQueryHandle productQueryHandle, XAsyncBlockPtr asyncBlock);

        //STDAPI XStoreProductsQueryNextPageResult(
        //    _Inout_ XAsyncBlock* async,
        //    _Out_ XStoreProductQueryHandle* productQueryHandle
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreProductsQueryNextPageResult(XAsyncBlockPtr asyncBlock, out XStoreProductQueryHandle productQueryHandle);

        //STDAPI XStoreQueryEntitledProductsAsync(
        //    _In_ const XStoreContextHandle storeContextHandle,
        //    _In_ XStoreProductKind productKinds,
        //    _In_ uint32_t maxItemsToRetrievePerPage,
        //    _Inout_ XAsyncBlock* async
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreQueryEntitledProductsAsync(XStoreContextHandle storeContextHandle, XStoreProductKind productKinds, UInt32 maxItemsToRetrievePerPage, XAsyncBlockPtr asyncBlock);

        //STDAPI XStoreQueryEntitledProductsResult(
        //    _Inout_ XAsyncBlock* async,
        //    _Out_ XStoreProductQueryHandle* productQueryHandle
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreQueryEntitledProductsResult(XAsyncBlockPtr asyncBlock, out XStoreProductQueryHandle productQueryHandle);

        //STDAPI XStoreQueryProductForCurrentGameAsync(
        //    _In_ const XStoreContextHandle storeContextHandle,
        //    _Inout_ XAsyncBlock* async
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreQueryProductForCurrentGameAsync(XStoreContextHandle storeContextHandle, XAsyncBlockPtr asyncBlock);

        //STDAPI XStoreQueryProductForCurrentGameResult(
        //    _Inout_ XAsyncBlock* async,
        //    _Out_ XStoreProductQueryHandle* productQueryHandle
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreQueryProductForCurrentGameResult(XAsyncBlockPtr asyncBlock, out XStoreProductQueryHandle productQueryHandle);

        //STDAPI XStoreQueryProductForPackageAsync(
        //    _In_ const XStoreContextHandle storeContextHandle,
        //    _In_ XStoreProductKind productKinds,
        //    _In_z_ const char* packageIdentifier,
        //    _Inout_ XAsyncBlock* async
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreQueryProductForPackageAsync(XStoreContextHandle storeContextHandle, XStoreProductKind productKinds, Byte[] packageIdentifier, XAsyncBlockPtr asyncBlock);

        //STDAPI XStoreQueryProductForPackageResult(
        //    _Inout_ XAsyncBlock* async,
        //    _Out_ XStoreProductQueryHandle* productQueryHandle
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreQueryProductForPackageResult(XAsyncBlockPtr asyncBlock, out XStoreProductQueryHandle productQueryHandle);

        //STDAPI XStoreQueryProductsAsync(
        //    _In_ const XStoreContextHandle storeContextHandle,
        //    _In_ XStoreProductKind productKinds,
        //    _In_z_count_(storeIdsCount) const char** storeIds,
        //    _In_ size_t storeIdsCount,
        //    _In_opt_z_count_(actionFiltersCount) const char** actionFilters,
        //    _In_ size_t actionFiltersCount,
        //    _Inout_ XAsyncBlock* async
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreQueryProductsAsync(
            XStoreContextHandle storeContextHandle,
            XStoreProductKind productKinds,
            IntPtr storeIds,
            SizeT storeIdsCount,
            IntPtr actionFilters,
            SizeT actionFiltersCount,
            XAsyncBlockPtr asyncBlock);

        //STDAPI XStoreQueryProductsResult(
        //    _Inout_ XAsyncBlock* async,
        //    _Out_ XStoreProductQueryHandle* productQueryHandle
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreQueryProductsResult(XAsyncBlockPtr asyncBlock, out XStoreProductQueryHandle productQueryHandle);
    }
}
