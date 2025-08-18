﻿using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    internal partial class XblInterop
    {
        //STDAPI XblSocialGetSocialRelationshipsAsync(
        //    _In_ XblContextHandle xboxLiveContext,
        //    _In_ uint64_t xboxUserId,
        //    _In_ XblSocialRelationshipFilter socialRelationshipFilter,
        //    _In_ size_t startIndex,
        //    _In_ size_t maxItems,
        //    _In_ XAsyncBlock* async
        //) XBL_NOEXCEPT;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XblSocialGetSocialRelationshipsAsync(
            XblContextHandle xboxLiveContext,
            UInt64 xboxUserId,
            XblSocialRelationshipFilter socialRelationshipFilter,
            SizeT startIndex,
            SizeT maxItems,
            XAsyncBlockPtr async
            );

        //STDAPI XblSocialGetSocialRelationshipsResult(
        //    _In_ XAsyncBlock* async,
        //    _Out_ XblSocialRelationshipResultHandle* handle
        //) XBL_NOEXCEPT;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XblSocialGetSocialRelationshipsResult(
            XAsyncBlockPtr async,
            out XblSocialRelationshipResultHandle handle
            );

        //STDAPI XblSocialRelationshipResultGetRelationships(
        //    _In_ XblSocialRelationshipResultHandle resultHandle,
        //    _Out_ const XblSocialRelationship** relationships,
        //    _Out_ size_t* relationshipsCount
        //) XBL_NOEXCEPT;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XblSocialRelationshipResultGetRelationships(
            XblSocialRelationshipResultHandle resultHandle,
            out IntPtr relationships,
            out SizeT relationshipsCount
            );

        //STDAPI XblSocialRelationshipResultHasNext(
        //    _In_ XblSocialRelationshipResultHandle resultHandle,
        //    _Out_ bool* hasNext
        //) XBL_NOEXCEPT;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XblSocialRelationshipResultHasNext(
            XblSocialRelationshipResultHandle resultHandle,
            [MarshalAs(UnmanagedType.U1)] out bool hasNext
            );

        //STDAPI XblSocialRelationshipResultGetTotalCount(
        //    _In_ XblSocialRelationshipResultHandle resultHandle,
        //    _Out_ size_t* totalCount
        //) XBL_NOEXCEPT;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XblSocialRelationshipResultGetTotalCount(
            XblSocialRelationshipResultHandle resultHandle,
            out SizeT totalCount
            );

        //STDAPI XblSocialRelationshipResultGetNextAsync(
        //    _In_ XblContextHandle xboxLiveContext,
        //    _In_ XblSocialRelationshipResultHandle resultHandle,
        //    _In_ size_t maxItems,
        //    _In_ XAsyncBlock* async
        //) XBL_NOEXCEPT;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XblSocialRelationshipResultGetNextAsync(
            XblContextHandle xboxLiveContext,
            XblSocialRelationshipResultHandle resultHandle,
            SizeT maxItems,
            XAsyncBlockPtr async
            );

        //STDAPI XblSocialRelationshipResultGetNextResult(
        //    _In_ XAsyncBlock* async,
        //    _Out_ XblSocialRelationshipResultHandle* handle
        //) XBL_NOEXCEPT;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XblSocialRelationshipResultGetNextResult(
            XAsyncBlockPtr async,
            out XblSocialRelationshipResultHandle handle
            );

        //STDAPI XblSocialRelationshipResultDuplicateHandle(
        //    _In_ XblSocialRelationshipResultHandle handle,
        //    _Out_ XblSocialRelationshipResultHandle* duplicatedHandle
        //) XBL_NOEXCEPT;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XblSocialRelationshipResultDuplicateHandle(
            XblSocialRelationshipResultHandle handle,
            out XblSocialRelationshipResultHandle duplicatedHandle
            );

        //STDAPI_(void) XblSocialRelationshipResultCloseHandle(
        //    _In_ XblSocialRelationshipResultHandle handle
        //) XBL_NOEXCEPT;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern void XblSocialRelationshipResultCloseHandle(
            XblSocialRelationshipResultHandle handle
            );
    }
}
