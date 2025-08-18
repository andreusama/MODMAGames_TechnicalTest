using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef bool CALLBACK XPackageEnumerationCallback(_In_ void* context, _In_ const XPackageDetails* details);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate NativeBool XPackageEnumerationCallback(IntPtr context, ref XPackageDetails details);

    //typedef void CALLBACK XPackageInstalledCallback(_In_ void* context, _In_ const XPackageDetails* details);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate void XPackageInstalledCallback(IntPtr context, ref XPackageDetails details);

    //typedef bool CALLBACK XPackageChunkAvailabilityCallback(_In_ void* context, _In_ const XPackageChunkSelector* selector, _In_ XPackageChunkAvailability availability);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate NativeBool XPackageChunkAvailabilityCallback(IntPtr context, ref XPackageChunkSelector selector, XPackageChunkAvailability availability);

    //typedef void CALLBACK XPackageInstallationProgressCallback(_In_ void* context, _In_ XPackageInstallationMonitorHandle monitor);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate void XPackageInstallationProgressCallback(IntPtr context, XPackageInstallationMonitorHandle monitor);

    //typedef bool CALLBACK XPackageFeatureEnumerationCallback(_In_ void* context, _In_ const XPackageFeature* feature);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate NativeBool XPackageFeatureEnumerationCallback(IntPtr context, ref XPackageFeature feature);

    partial class XGRInterop
    {
        internal const Int32 LOCALE_NAME_MAX_LENGTH = 85;

        //STDAPI XPackageGetCurrentProcessPackageIdentifier(
        //    _In_ size_t bufferSize,
        //    _Out_writes_(bufferSize) char* buffer
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XPackageGetCurrentProcessPackageIdentifier(SizeT bufferSize, [Out] Byte[] buffer);

        //STDAPI_(bool) XPackageIsPackagedProcess() noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        internal static extern bool XPackageIsPackagedProcess();

        //STDAPI XPackageGetUserLocale(
        //    _In_ size_t localeSize,
        //    _Out_writes_(localeSize) char* locale
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XPackageGetUserLocale(SizeT localeSize, [Out] Byte[] locale);

        //STDAPI XPackageEnumeratePackages(
        //    _In_ XPackageKind kind,
        //    _In_ XPackageEnumerationScope scope,
        //    _In_opt_ void* context,
        //    _In_ XPackageEnumerationCallback* callback
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XPackageEnumeratePackages(XPackageKind kind, XPackageEnumerationScope scope, IntPtr context, XPackageEnumerationCallback callback);

        //STDAPI XPackageRegisterPackageInstalled(
        //    _In_ XTaskQueueHandle queue,
        //    _In_opt_ void* context,
        //    _In_ XPackageInstalledCallback* callback,
        //    _Out_ XTaskQueueRegistrationToken* token
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XPackageRegisterPackageInstalled(XTaskQueueHandle queue, IntPtr context, XPackageInstalledCallback callback, out XTaskQueueRegistrationToken token);

        //STDAPI_(bool) XPackageUnregisterPackageInstalled(
        //    _In_ XTaskQueueRegistrationToken token,
        //    _In_ bool wait
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        internal static extern bool XPackageUnregisterPackageInstalled(XTaskQueueRegistrationToken token, [MarshalAs(UnmanagedType.U1)] bool wait);

        //STDAPI XPackageEnumerateFeatures(
        //    _In_z_ const char* packageIdentifier,
        //    _In_opt_ void* context,
        //    _In_ XPackageFeatureEnumerationCallback* callback
        //) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int XPackageEnumerateFeatures(
          byte[] packageIdentifier,
          IntPtr context,
          XPackageFeatureEnumerationCallback callback);

        //STDAPI XPackageMount(
        //    _In_z_ const char* packageIdentifier,
        //    _Out_ XPackageMountHandle* mount
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XPackageMount(Byte[] packageIdentifier, out XPackageMountHandle mountHandle);

        //STDAPI XPackageGetMountPathSize(
        //    _In_ XPackageMountHandle mount,
        //    _Out_ size_t* pathSize
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XPackageGetMountPathSize(XPackageMountHandle mountHandle, out SizeT pathSize);

        //STDAPI XPackageGetMountPath(
        //    _In_ XPackageMountHandle mount,
        //    _In_ size_t pathSize,
        //    _Out_writes_(pathSize) char* path
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XPackageGetMountPath(XPackageMountHandle mountHandle, SizeT pathSize, [Out] Byte[] path);

        //STDAPI_(void) XPackageCloseMountHandle(
        //    _In_ XPackageMountHandle mount
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern void XPackageCloseMountHandle(XPackageMountHandle mountHandle);

        //STDAPI XPackageCreateInstallationMonitor(
        //    _In_z_ const char* packageIdentifier,
        //    _In_ uint32_t selectorCount,
        //    _In_reads_opt_(selectorCount) XPackageChunkSelector* selectors,
        //    _In_ uint32_t minimumUpdateIntervalMs, // 0 == no update
        //    _In_opt_ XTaskQueueHandle queue,
        //    _Out_ XPackageInstallationMonitorHandle* installationMonitor
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XPackageCreateInstallationMonitor(
            byte[] packageIdentifier,
            UInt32 selectorCount,
            [Optional] XPackageChunkSelector[] selectors,
            UInt32 minimumUpdateIntervalMs,
            [Optional] XTaskQueueHandle queue,
            out XPackageInstallationMonitorHandle installationMonitor);

        //STDAPI_(void) XPackageCloseInstallationMonitorHandle(
        //    _In_ XPackageInstallationMonitorHandle installationMonitor
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern void XPackageCloseInstallationMonitorHandle(XPackageInstallationMonitorHandle installationMonitor);

        //STDAPI_(void) XPackageGetInstallationProgress(
        //    _In_ XPackageInstallationMonitorHandle installationMonitor,
        //    _Out_ XPackageInstallationProgress* progress
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern void XPackageGetInstallationProgress(
            XPackageInstallationMonitorHandle installationMonitor,
            out XPackageInstallationProgress progress);

        //STDAPI_(bool) XPackageUpdateInstallationMonitor(
        //    _In_ XPackageInstallationMonitorHandle installationMonitor
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        internal static extern bool XPackageUpdateInstallationMonitor(XPackageInstallationMonitorHandle installationMonitor);

        //STDAPI XPackageRegisterInstallationProgressChanged(
        //    _In_ XPackageInstallationMonitorHandle installationMonitor,
        //    _In_opt_ void* context,
        //    _In_ XPackageInstallationProgressCallback* callback,
        //    _Out_ XTaskQueueRegistrationToken* token
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XPackageRegisterInstallationProgressChanged(
            XPackageInstallationMonitorHandle installationMonitor,
            IntPtr context,
            XPackageInstallationProgressCallback callback,
            out XTaskQueueRegistrationToken token);

        //STDAPI_(bool) XPackageUnregisterInstallationProgressChanged(
        //    _In_ XPackageInstallationMonitorHandle installationMonitor,
        //    _In_ XTaskQueueRegistrationToken token,
        //    _In_ bool wait
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        internal static extern bool XPackageUnregisterInstallationProgressChanged(
            XPackageInstallationMonitorHandle installationMonitor,
            XTaskQueueRegistrationToken token,
            [MarshalAs(UnmanagedType.U1)]
            bool wait);

        //STDAPI XPackageEstimateDownloadSize(
        //    _In_z_ const char* packageIdentifier,
        //    _In_ uint32_t selectorCount,
        //    _In_reads_(selectorCount) XPackageChunkSelector* selectors,
        //    _Out_ uint64_t* downloadSize,
        //    _Out_opt_ bool* shouldPresentUserConfirmation
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XPackageEstimateDownloadSize(
            byte[] packageIdentifier,
            UInt32 selectorCount,
            [Optional] XPackageChunkSelector[] selectors,
            out UInt64 downloadSize,
            [MarshalAs(UnmanagedType.U1)] [Optional] out bool shouldPresentUserConfirmation);

        //STDAPI XPackageFindChunkAvailability(
        //    _In_z_ const char* packageIdentifier,
        //    _In_ uint32_t selectorCount,
        //    _In_reads_opt_(selectorCount) XPackageChunkSelector* selectors,
        //    _Out_ XPackageChunkAvailability* availability
        //    ) noexcept;

        //STDAPI XPackageEnumerateChunkAvailability(
        //    _In_z_ const char* packageIdentifier,
        //    _In_ XPackageChunkSelectorType type,
        //    _In_ void* context,
        //    _In_ XPackageChunkAvailabilityCallback* callback
        //    ) noexcept;

        //STDAPI XPackageChangeChunkInstallOrder(
        //    _In_z_ const char* packageIdentifier,
        //    _In_ uint32_t selectorCount,
        //    _In_reads_(selectorCount) XPackageChunkSelector* selectors
        //    ) noexcept;

        //STDAPI XPackageInstallChunks(
        //    _In_z_ const char* packageIdentifier,
        //    _In_ uint32_t selectorCount,
        //    _In_reads_(selectorCount) XPackageChunkSelector* selectors,
        //    _In_ uint32_t minimumUpdateIntervalMs,
        //    _In_ bool suppressUserConfirmation,
        //    _In_opt_ XTaskQueueHandle queue,
        //    _Out_ XPackageInstallationMonitorHandle* installationMonitor
        //    ) noexcept;

        //STDAPI XPackageInstallChunksAsync(
        //    _In_z_ const char* packageIdentifier,
        //    _In_ uint32_t selectorCount,
        //    _In_reads_(selectorCount) XPackageChunkSelector* selectors,
        //    _In_ uint32_t minimumUpdateIntervalMs,
        //    _In_ bool suppressUserConfirmation,
        //    _Inout_ XAsyncBlock* asyncBlock
        //    ) noexcept;

        //STDAPI XPackageInstallChunksAsyncResult(
        //    _Inout_ XAsyncBlock* asyncBlock,
        //    _Out_ XPackageInstallationMonitorHandle* installationMonitor
        //    ) noexcept;

        //STDAPI XPackageUninstallChunks(
        //    _In_z_ const char* packageIdentifier,
        //    _In_ uint32_t selectorCount,
        //    _In_reads_(selectorCount) XPackageChunkSelector* selectors
        //    ) noexcept;

        //STDAPI XPackageGetWriteStats(
        //    _Out_ XPackageWriteStats* writeStats
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int XPackageGetWriteStats(out XPackageWriteStats writeStats);
    }
}
