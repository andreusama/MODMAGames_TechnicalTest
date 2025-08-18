using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public partial class SDK
    {
        public partial class XBL
        {
            public delegate void XblSocialGetSocialRelationshipsResult(Int32 hresult, XblSocialRelationshipResult handle);
            public delegate void XblSocialRelationshipResultGetNextResult(Int32 hresult, XblSocialRelationshipResult handle);

            public static void XblSocialGetSocialRelationshipsAsync(
                XblContextHandle xboxLiveContext,
                UInt64 xboxUserId,
                XblSocialRelationshipFilter socialRelationshipFilter,
                uint startIndex,
                uint maxItems,
                XblSocialGetSocialRelationshipsResult completionRoutine
                )
            {
                if (xboxLiveContext == null)
                {
                    completionRoutine(HR.E_INVALIDARG, null);
                    return;
                }

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
                {
                    Interop.XblSocialRelationshipResultHandle result;
                    Int32 hresult = XblInterop.XblSocialGetSocialRelationshipsResult(block, out result);
                    if (HR.FAILED(hresult))
                    {
                        completionRoutine(hresult, null);
                        return;
                    }

                    completionRoutine(hresult, new XblSocialRelationshipResult(result));
                });

                Int32 hr = XblInterop.XblSocialGetSocialRelationshipsAsync(
                    xboxLiveContext.InteropHandle,
                    xboxUserId,
                    socialRelationshipFilter,
                    new SizeT(startIndex),
                    new SizeT(maxItems),
                    asyncBlock);

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr, null);
                }
            }

            public static Int32 XblSocialRelationshipResultGetRelationships(
                XblSocialRelationshipResult result,
                out XblSocialRelationship[] relationships
                )
            {
                if (result == null)
                {
                    relationships = null;
                    return HR.E_INVALIDARG;
                }

                IntPtr interopRelationships;
                SizeT interopRelationshipsSize;

                Int32 hr = XblInterop.XblSocialRelationshipResultGetRelationships(result.InteropHandle, out interopRelationships, out interopRelationshipsSize);

                if (HR.FAILED(hr))
                {
                    relationships = null;
                    return hr;
                }

                relationships = Converters.PtrToClassArray<XblSocialRelationship, Interop.XblSocialRelationship>(interopRelationships, interopRelationshipsSize, c => new XblSocialRelationship(c));

                return hr;
            }

            public static Int32 XblSocialRelationshipResultHasNext(
                XblSocialRelationshipResult result,
                out bool hasNext
                )
            {
                hasNext = false;
                if (result == null)
                {
                    return HR.E_INVALIDARG;
                }

                Int32 hr = XblInterop.XblSocialRelationshipResultHasNext(result.InteropHandle, out hasNext);

                return hr;
            }

            public static Int32 XblSocialRelationshipResultGetTotalCount(
                XblSocialRelationshipResult result,
                out UInt32 totalCount
                )
            {
                totalCount = 0;
                if (result == null)
                {
                    return HR.E_INVALIDARG;
                }

                SizeT totalCountInterop;

                Int32 hr = XblInterop.XblSocialRelationshipResultGetTotalCount(result.InteropHandle, out totalCountInterop);
                if (HR.FAILED(hr))
                {
                    return hr;
                }

                totalCount = totalCountInterop.ToUInt32();

                return hr;
            }

            public static void XblSocialRelationshipResultGetNextAsync(
                XblContextHandle xboxLiveContext,
                XblSocialRelationshipResult result,
                uint maxItems,
                XblSocialRelationshipResultGetNextResult completionRoutine
                )
            {
                if (xboxLiveContext == null || result == null)
                {
                    completionRoutine(HR.E_INVALIDARG, null);
                    return;
                }

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
                {
                    Interop.XblSocialRelationshipResultHandle getNextResult;
                    Int32 hresult = XblInterop.XblSocialRelationshipResultGetNextResult(block, out getNextResult);
                    if (HR.FAILED(hresult))
                    {
                        completionRoutine(hresult, null);
                        return;
                    }

                    completionRoutine(hresult, new XblSocialRelationshipResult(getNextResult));
                });

                Int32 hr = XblInterop.XblSocialRelationshipResultGetNextAsync(
                    xboxLiveContext.InteropHandle,
                    result.InteropHandle,
                    new SizeT(maxItems),
                    asyncBlock);

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr, null);
                }
            }

            public static Int32 XblSocialRelationshipResultDuplicateHandle(
                XblSocialRelationshipResult handle,
                out XblSocialRelationshipResult duplicatedHandle
                )
            {
                duplicatedHandle = null;
                if (handle == null)
                {
                    return HR.E_INVALIDARG;
                }

                XblSocialRelationshipResultHandle duplicatedHandleInterop;

                Int32 hr = XblInterop.XblSocialRelationshipResultDuplicateHandle(handle.InteropHandle, out duplicatedHandleInterop);
                if (HR.FAILED(hr))
                {
                    return hr;
                }

                duplicatedHandle = new XblSocialRelationshipResult(duplicatedHandleInterop);
                return hr;
            }

            public static void XblSocialRelationshipResultCloseHandle(XblSocialRelationshipResult handle)
            {
                handle.Dispose();
            }
        }
    }
}
