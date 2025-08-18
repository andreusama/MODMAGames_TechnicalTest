using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    // typedef void (CALLBACK HCTraceCallback) (_In_z_ const char* areaName, _In_ HCTraceLevel level, _In_ uint64_t threadId, _In_ uint64_t timestamp, _In_z_ const char* message);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate void HCTraceCallback(
        Byte[] areaName,
        HCTraceLevel level,
        UInt64 threadId,
        UInt64 timestamp,
        Byte[] message
        );

    internal partial class XblInterop
    {
        //STDAPI HCSettingsSetTraceLevel(
        //    _In_ HCTraceLevel traceLevel
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 HCSettingsSetTraceLevel(
            HCTraceLevel traceLevel
            );

        //STDAPI HCSettingsGetTraceLevel(
        //    _Out_ HCTraceLevel* traceLevel
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 HCSettingsGetTraceLevel(
            out HCTraceLevel traceLevel
            );

        // STDAPI_(void) HCTraceSetClientCallback(_In_opt_ HCTraceCallback* callback) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern void HCTraceSetClientCallback(
            HCTraceCallback callback
            );

        // STDAPI_(void) HCTraceSetTraceToDebugger(_In_ bool traceToDebugger) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern void HCTraceSetTraceToDebugger(
            [MarshalAs(UnmanagedType.U1)] bool traceToDebugger
            );
    }
}
