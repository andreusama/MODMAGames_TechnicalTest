using System;
using System.Runtime.InteropServices;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public partial class SDK
    {
        public partial class XBL
        {
            public delegate void XblMultiplayerActivityAsyncOperationCompleted(Int32 hresult);

            public delegate void XblMultiplayerGetActivityAsyncOperationCompleted(Int32 hresult, XblMultiplayerActivityInfo[] results);

            public static Int32 XblMultiplayerActivityUpdateRecentPlayers(
                XblContextHandle xboxLiveContext,
                XblMultiplayerActivityRecentPlayerUpdate[] updates
                )
            {
                if (xboxLiveContext == null)
                {
                    return HR.E_INVALIDARG;
                }

                var interopUpdates = Converters.ConvertArrayToFixedLength(updates, updates.Length, r => new Interop.XblMultiplayerActivityRecentPlayerUpdate(r));

                Int32 hresult = XblInterop.XblMultiplayerActivityUpdateRecentPlayers(
                    xboxLiveContext.InteropHandle,
                    interopUpdates,
                    new SizeT(updates == null ? 0 : updates.Length));

                return hresult;
            }

            public static void XblMultiplayerActivityFlushRecentPlayersAsync(
                XblContextHandle xboxLiveContext,
                XblMultiplayerActivityAsyncOperationCompleted completionRoutine)
            {
                if (xboxLiveContext == null)
                {
                    completionRoutine(HR.E_INVALIDARG);
                    return;
                }

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
                {
                    completionRoutine(XGRInterop.XAsyncGetStatus(block, false));
                });

                int hr = XblInterop.XblMultiplayerActivityFlushRecentPlayersAsync(
                    xboxLiveContext.InteropHandle,
                    asyncBlock);

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr);
                }
            }

            public static void XblMultiplayerActivitySetActivityAsync(
                XblContextHandle xboxLiveContext,
                XblMultiplayerActivityInfo activityInfo,
                bool allowCrossPlatformJoin,
                XblMultiplayerActivityAsyncOperationCompleted completionRoutine)
            {
                if (xboxLiveContext == null)
                {
                    completionRoutine(HR.E_INVALIDARG);
                    return;
                }

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
                {
                    completionRoutine(XGRInterop.XAsyncGetStatus(block, false));
                });

                using (DisposableCollection disposableCollection = new DisposableCollection())
                {
                    Interop.XblMultiplayerActivityInfo interopActivityInfo = new Interop.XblMultiplayerActivityInfo(activityInfo, disposableCollection);
                    int hr = XblInterop.XblMultiplayerActivitySetActivityAsync(
                        xboxLiveContext.InteropHandle,
                        ref interopActivityInfo,
                        allowCrossPlatformJoin,
                        asyncBlock);

                    if (HR.FAILED(hr))
                    {
                        AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                        completionRoutine(hr);
                    }
                }
            }

            public static void XblMultiplayerActivityGetActivityAsync(
                XblContextHandle xboxLiveContext,
                UInt64[] xboxUserIdList,
                XblMultiplayerGetActivityAsyncOperationCompleted completionRoutine
            )
            {
                if (xboxLiveContext == null)
                {
                    completionRoutine(HR.E_INVALIDARG, null);
                    return;
                }

                int hr;

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
                {
                    hr = XblInterop.XblMultiplayerActivityGetActivityResultSize(
                        block,
                        out SizeT resultSizeInBytes
                        );

                    if (HR.FAILED(hr))
                    {
                        completionRoutine(hr, null);
                        return;
                    }

                    using (DisposableBuffer buffer = new DisposableBuffer(resultSizeInBytes.ToInt32()))
                    {
                        hr = XblInterop.XblMultiplayerActivityGetActivityResult(
                            block,
                            resultSizeInBytes,
                            buffer.IntPtr,
                            out IntPtr ptrToResults,
                            out SizeT resultCount,
                            out SizeT bufferUsed
                        );

                        if (HR.FAILED(hr))
                        {
                            completionRoutine(hr, null);
                            return;
                        }

                        completionRoutine(hr, Converters.PtrToClassArray<XblMultiplayerActivityInfo, Interop.XblMultiplayerActivityInfo>(ptrToResults, resultCount, r => new XblMultiplayerActivityInfo(r)));
                    }
                });

                hr = XblInterop.XblMultiplayerActivityGetActivityAsync(
                    xboxLiveContext.InteropHandle,
                    xboxUserIdList,
                    new SizeT(xboxUserIdList?.Length ?? 0),
                    asyncBlock);

                if(HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr, null);
                }
            }

            public static void XblMultiplayerActivityDeleteActivityAsync(
                XblContextHandle xboxLiveContext,
                XblMultiplayerActivityAsyncOperationCompleted completionRoutine)
            {
                if (xboxLiveContext == null)
                {
                    completionRoutine(HR.E_INVALIDARG);
                    return;
                }

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
                {
                    completionRoutine(XGRInterop.XAsyncGetStatus(block, false));
                });

                int hr = XblInterop.XblMultiplayerActivityDeleteActivityAsync(
                    xboxLiveContext.InteropHandle,
                    asyncBlock);

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr);
                }
            }

            public static void XblMultiplayerActivitySendInvitesAsync(
                XblContextHandle xboxLiveContext,
                UInt64[] xboxUserIdList,
                bool allowCrossPlatformJoin,
                string connectionString,
                XblMultiplayerActivityAsyncOperationCompleted completionRoutine
            )
            {
                if (xboxLiveContext == null)
                {
                    completionRoutine(HR.E_INVALIDARG);
                    return;
                }

                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
                {
                    completionRoutine(XGRInterop.XAsyncGetStatus(block, false));
                });

                int hr = XblInterop.XblMultiplayerActivitySendInvitesAsync(
                    xboxLiveContext.InteropHandle,
                    xboxUserIdList,
                    new SizeT( xboxUserIdList?.Length??0),
                    allowCrossPlatformJoin,
                    Converters.StringToNullTerminatedUTF8ByteArray (connectionString),
                    asyncBlock
                );

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    completionRoutine(hr);
                }
            }
        }
    }
}
