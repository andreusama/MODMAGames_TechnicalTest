using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Unity.GameCore.Interop
{
    partial class XGRInterop
    {
        //STDAPI XStoreShowRedeemTokenUIAsync(
        //    _In_ const XStoreContextHandle storeContextHandle,
        //    _In_z_ const char* token,
        //    _In_z_count_(allowedStoreIdsCount) const char** allowedStoreIds,
        //    _In_ size_t allowedStoreIdsCount,
        //    _In_ bool disallowCsvRedemption,
        //    _Inout_ XAsyncBlock* async
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreShowRedeemTokenUIAsync(
            XStoreContextHandle storeContextHandle,
            Byte[] token,
            IntPtr allowedStoreIds,
            SizeT allowedStoreIdsCount,
            bool disallowCsvRedeption,
            XAsyncBlockPtr asyncBlock);

        //STDAPI XStoreShowRedeemTokenUIResult(
        //    _Inout_ XAsyncBlock* async
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreShowRedeemTokenUIResult(XAsyncBlockPtr asyncBlock);

        //STDAPI XStoreShowRateAndReviewUIAsync(
        //    _In_ const XStoreContextHandle storeContextHandle,
        //    _Inout_ XAsyncBlock* async
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreShowRateAndReviewUIAsync(
            XStoreContextHandle storeContextHandle,
            XAsyncBlockPtr asyncBlock);

        //STDAPI XStoreShowRateAndReviewUIResult(
        //    _Inout_ XAsyncBlock* async,
        //    _Out_ XStoreRateAndReviewResult* result
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreShowRateAndReviewUIResult(XAsyncBlockPtr asyncBlock, out XStoreRateAndReview result);

        //STDAPI XStoreShowPurchaseUIAsync(
        //    _In_ const XStoreContextHandle storeContextHandle,
        //    _In_z_ const char* storeId,
        //    _In_opt_z_ const char* name,
        //    _In_opt_z_ const char* extendedJsonData,
        //    _Inout_ XAsyncBlock* async
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreShowPurchaseUIAsync(
            XStoreContextHandle storeContextHandle,
            Byte[] storeId,
            [Optional] Byte[] name,
            [Optional] Byte[] extendedJsonData,
            XAsyncBlockPtr asyncBlock);

        //STDAPI XStoreShowPurchaseUIResult(
        //    _Inout_ XAsyncBlock* async
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreShowPurchaseUIResult(XAsyncBlockPtr asyncBlock);

        //STDAPI XStoreQueryConsumableBalanceRemainingAsync(
        //    _In_ const XStoreContextHandle storeContextHandle,
        //    _In_z_ const char* storeProductId,
        //    _Inout_ XAsyncBlock* async
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreQueryConsumableBalanceRemainingAsync(
            XStoreContextHandle storeContextHandle,
            Byte[] storeProductId,
            XAsyncBlockPtr asyncBlock);

        //STDAPI XStoreQueryConsumableBalanceRemainingResult(
        //    _Inout_ XAsyncBlock* async,
        //    _Out_ XStoreConsumableResult* consumableResult
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreQueryConsumableBalanceRemainingResult(XAsyncBlockPtr asyncBlock, out XStoreConsumable result);

        //STDAPI XStoreReportConsumableFulfillmentAsync(
        //    _In_ const XStoreContextHandle storeContextHandle,
        //    _In_z_ const char* storeProductId,
        //    _In_ uint32_t quantity,
        //    _In_ GUID trackingId,
        //    _Inout_ XAsyncBlock* async
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreReportConsumableFulfillmentAsync(
            XStoreContextHandle storeContextHandle,
            Byte[] storeProductId,
            UInt32 quantity,
            Guid trackingId,
            XAsyncBlockPtr asyncBlock);

        //STDAPI XStoreReportConsumableFulfillmentResult(
        //    _Inout_ XAsyncBlock* async,
        //    _Out_ XStoreConsumableResult* consumableResult
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreReportConsumableFulfillmentResult(XAsyncBlockPtr asyncBlock, out XStoreConsumable result);

        //STDAPI XStoreGetUserCollectionsIdAsync(
        //    _In_ const XStoreContextHandle storeContextHandle,
        //    _In_z_ const char* serviceTicket,
        //    _In_z_ const char* publisherUserId,
        //    _Inout_ XAsyncBlock* async
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreGetUserCollectionsIdAsync(
            XStoreContextHandle storeContextHandle,
            Byte[] serviceTicket,
            Byte[] publisherUserId,
            XAsyncBlockPtr asyncBlock);

        //STDAPI XStoreGetUserCollectionsIdResultSize(
        //    _In_ XAsyncBlock* async,
        //    _Out_ size_t* size
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreGetUserCollectionsIdResultSize(XAsyncBlockPtr asyncBlock, out SizeT size);

        //STDAPI XStoreGetUserCollectionsIdResult(
        //    _Inout_ XAsyncBlock* async,
        //    _In_ size_t size,
        //    _Out_writes_z_(size) char* result
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreGetUserCollectionsIdResult(XAsyncBlockPtr asyncBlock, SizeT size, [Out] Byte[] token);

        //STDAPI XStoreGetUserPurchaseIdAsync(
        //    _In_ const XStoreContextHandle storeContextHandle,
        //    _In_z_ const char* serviceTicket,
        //    _In_z_ const char* publisherUserId,
        //    _Inout_ XAsyncBlock* async
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreGetUserPurchaseIdAsync(
            XStoreContextHandle storeContextHandle,
            Byte[] serviceTicket,
            Byte[] publisherUserId,
            XAsyncBlockPtr asyncBlock);

        //STDAPI XStoreGetUserPurchaseIdResultSize(
        //    _In_ XAsyncBlock* async,
        //    _Out_ size_t* size
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreGetUserPurchaseIdResultSize(XAsyncBlockPtr asyncBlock, out SizeT size);

        //STDAPI XStoreGetUserPurchaseIdResult(
        //    _Inout_ XAsyncBlock* async,
        //    _In_ size_t size,
        //    _Out_writes_z_(size) char* result
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XStoreGetUserPurchaseIdResult(XAsyncBlockPtr asyncBlock, SizeT size, [Out] Byte[] token);
    }
}
