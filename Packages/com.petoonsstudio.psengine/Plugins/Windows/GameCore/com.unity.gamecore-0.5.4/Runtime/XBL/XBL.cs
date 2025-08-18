using System;
using System.Runtime.InteropServices;

using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public partial class SDK
    {
        public partial class XBL
        {
            public delegate void XblCleanupResult(Int32 hresult);

            public static Int32 XblInitialize(string scid)
            {
                IntPtr scidPtr = Marshal.StringToHGlobalAnsi(scid);

                XblInitArgs args = new XblInitArgs();
                args.queue = defaultQueue.handle;
                args.scid = scidPtr;

                Int32 blockSize = Marshal.SizeOf(args);
                IntPtr argsPnt = Marshal.AllocHGlobal(blockSize);
                Marshal.StructureToPtr(args, argsPnt, false);

                XblInitArgsPtr argsPtr = new XblInitArgsPtr(argsPnt);

                Int32 result = XblInterop.XblInitialize(argsPtr);
                Marshal.FreeHGlobal(scidPtr);
                Marshal.FreeHGlobal(argsPnt);

                return result;
            }

            public static void XblCleanup(XblCleanupResult completionRoutine)
            {
                XAsyncBlockPtr asyncBlock = AsyncHelpers.WrapAsyncBlock(defaultQueue.handle, (XAsyncBlockPtr block) =>
                {
                    Int32 hresult = XGRInterop.XAsyncGetStatus(block, wait: false);
                    if (completionRoutine != null)
                        completionRoutine(hresult);
                });

                Int32 hr = XblInterop.XblCleanupAsync(asyncBlock);

                if (HR.FAILED(hr))
                {
                    AsyncHelpers.CleanupAsyncBlock(asyncBlock);
                    if (completionRoutine != null)
                        completionRoutine(hr);
                }
            }

            public static Int32 XblContextCreateHandle(XUserHandle user, out XblContextHandle context)
            {
                if (user == null)
                {
                    context = default(XblContextHandle);
                    return HR.E_INVALIDARG;
                }

                Interop.XblContextHandle interopContext;
                Int32 hresult = XblInterop.XblContextCreateHandle(user.InteropHandle, out interopContext);
                if (HR.SUCCEEDED(hresult))
                {
                    context = new XblContextHandle(interopContext);
                    context.m_gCHandle = GCHandle.Alloc(context, GCHandleType.Normal);
                }
                else
                {
                    context = default(XblContextHandle);
                }
                return hresult;
            }

            public static void XblContextCloseHandle(XblContextHandle xboxLiveContextHandle)
            {
                if (xboxLiveContextHandle == null)
                {
                    return;
                }

                if (xboxLiveContextHandle.sessionChangedCallback != null)
                {
                    foreach(var i in xboxLiveContextHandle.sessionChangedCallback.GetInvocationList())
                    {
                        xboxLiveContextHandle.sessionChangedCallback -= (SDK.XBL.XblMultiplayerSessionChangedHandler)i;
                    }
                }

                if (xboxLiveContextHandle.sessionSubscriptionLostCallback != null)
                {
                    foreach (var i in xboxLiveContextHandle.sessionSubscriptionLostCallback.GetInvocationList())
                    {
                        xboxLiveContextHandle.sessionSubscriptionLostCallback -= (SDK.XBL.XblMultiplayerSessionSubscriptionLostHandler)i;
                    }
                }

                if (xboxLiveContextHandle.connectionIdChangedCallback != null)
                {
                    foreach (var i in xboxLiveContextHandle.connectionIdChangedCallback.GetInvocationList())
                    {
                        xboxLiveContextHandle.connectionIdChangedCallback -= (SDK.XBL.XblMultiplayerConnectionIdChangedHandler)i;
                    }
                }

                if (xboxLiveContextHandle.statisticChangedCallback != null)
                {
                    foreach (var i in xboxLiveContextHandle.statisticChangedCallback.GetInvocationList())
                    {
                        xboxLiveContextHandle.statisticChangedCallback -= (SDK.XBL.XblUserStatisticsStatisticChangedHandler)i;
                    }
                }

                if (xboxLiveContextHandle.m_gCHandle != null)
                    xboxLiveContextHandle.m_gCHandle.Free();

                XblInterop.XblContextCloseHandle(xboxLiveContextHandle.InteropHandle);
                xboxLiveContextHandle.InteropHandle = new Interop.XblContextHandle();
            }
        }
    }
}
