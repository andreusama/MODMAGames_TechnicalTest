using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    partial class XGRInterop
    {
        //STDAPI_(XSystemAnalyticsInfo) XSystemGetAnalyticsInfo() noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern XSystemAnalyticsInfo XSystemGetAnalyticsInfo();

        //const size_t XSystemConsoleIdBytes = 39;
        public const int XSystemConsoleIdBytes = 39;

        // STDAPI XSystemGetConsoleId(
        // _In_ size_t consoleIdSize,
        // _Out_writes_bytes_to_(consoleIdSize, * consoleIdUsed) char* consoleId,
        // _Out_opt_ size_t* consoleIdUsed
        // ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XSystemGetConsoleId(SizeT consoleIdSize, IntPtr consoleId, out SizeT consoleIdUsed);

        //const size_t XSystemXboxLiveSandboxIdMaxBytes = 16;
        public const int XSystemXboxLiveSandboxIdMaxBytes = 16;

        // STDAPI XSystemGetXboxLiveSandboxId(
        // _In_ size_t sandboxIdSize,
        // _Out_writes_bytes_to_(sandboxIdSize, *sandboxIdUsed) char* sandboxId,
        // _Out_opt_ size_t* sandboxIdUsed
        // ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XSystemGetXboxLiveSandboxId(SizeT sandboxIdSize, IntPtr sandboxId, out SizeT sandboxIdUsed);

        //const size_t XSystemAppSpecificDeviceIdBytes = 45;
        public const int XSystemAppSpecificDeviceIdBytes = 45;

        //STDAPI XSystemGetAppSpecificDeviceId(
        //_In_ size_t appSpecificDeviceIdSize,
        //_Out_writes_bytes_to_(appSpecificDeviceIdSize, * appSpecificDeviceIdUsed) char* appSpecificDeviceId,
        //_Out_opt_ size_t* appSpecificDeviceIdUsed
        //) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 XSystemGetAppSpecificDeviceId(SizeT appSpecificDeviceIdSize, IntPtr appSpecificDeviceId, out SizeT appSpecificDeviceIdUsed);
    }
}
