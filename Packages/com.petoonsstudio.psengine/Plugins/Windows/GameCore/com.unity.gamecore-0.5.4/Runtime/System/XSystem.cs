using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public partial class SDK
    {
        public static XSystemAnalyticsInfo XSystemGetAnalyticsInfo()
        {
            var raw = XGRInterop.XSystemGetAnalyticsInfo();

            return new XSystemAnalyticsInfo(raw);
        }

        public static Int32 XSystemGetConsoleId(out string consoleId)
        {
            using (var buffer = new DisposableBuffer(XGRInterop.XSystemConsoleIdBytes))
            {
                int ret = XGRInterop.XSystemGetConsoleId(new SizeT(XGRInterop.XSystemConsoleIdBytes), buffer.IntPtr, out SizeT usedBytes);

                if (HR.SUCCEEDED(ret))
                {
                    consoleId = Converters.PtrToStringUTF8(buffer.IntPtr);
                }
                else
                {
                    consoleId = String.Empty;
                }

                return ret;
            }
        }

        public static Int32 XSystemGetXboxLiveSandboxId(out string sandboxId)
        {
            using (var buffer = new DisposableBuffer(XGRInterop.XSystemXboxLiveSandboxIdMaxBytes))
            {
                int ret = XGRInterop.XSystemGetXboxLiveSandboxId(new SizeT(XGRInterop.XSystemXboxLiveSandboxIdMaxBytes), buffer.IntPtr, out SizeT usedBytes);

                if (HR.SUCCEEDED(ret))
                {
                    sandboxId = Converters.PtrToStringUTF8(buffer.IntPtr);
                }
                else
                {
                    sandboxId = String.Empty;
                }

                return ret;
            }
        }

        public static Int32 XSystemGetAppSpecificDeviceId(out string appSpecificDeviceId )
        {
            using (var buffer = new DisposableBuffer(XGRInterop.XSystemAppSpecificDeviceIdBytes))
            {
                int ret = XGRInterop.XSystemGetAppSpecificDeviceId(new SizeT(XGRInterop.XSystemAppSpecificDeviceIdBytes), buffer.IntPtr, out SizeT usedBytes);

                if (HR.SUCCEEDED(ret))
                {
                    appSpecificDeviceId = Converters.PtrToStringUTF8(buffer.IntPtr);
                }
                else
                {
                    appSpecificDeviceId = String.Empty;
                }

                return ret;
            }
        }
    }
}