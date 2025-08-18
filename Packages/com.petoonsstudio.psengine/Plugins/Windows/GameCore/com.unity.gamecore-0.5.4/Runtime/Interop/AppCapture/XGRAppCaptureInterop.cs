using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef void CALLBACK XAppBroadcastMonitorCallback(_In_ void* context);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate void XAppBroadcastMonitorCallback(IntPtr context);

    //typedef void CALLBACK XAppCaptureMetadataPurgedCallback(_In_ void* context);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate void XAppCaptureMetadataPurgedCallback(IntPtr context);

    partial class XGRInterop
    {
        #region Size constants from XAppCapture.h
        // #define APPCAPTURE_MAX_PATH 260
        internal const Int32 APPCAPTURE_MAX_PATH = 260;
        // const uint8_t APPCAPTURE_MAX_CAPTURE_FILES = 10;
        internal const byte APPCAPTURE_MAX_CAPTURE_FILES = 10;
        // const int APPCAPTURE_MAX_LOCALID_LENGTH = 250;
        internal const Int32 APPCAPTURE_MAX_LOCALID_LENGTH = 250;
        #endregion

        //STDAPI XAppBroadcastShowUI(_In_ XUserHandle requestingUser);
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XAppBroadcastShowUI(XUserHandle requestingUser);

        //STDAPI XAppBroadcastGetStatus(_In_ XUserHandle requestingUser, _Out_ XAppBroadcastStatus* appBroadcastStatus);
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XAppBroadcastGetStatus(XUserHandle requestingUser, out XAppBroadcastStatus appBroadcastStatus);

        //STDAPI_(bool) XAppBroadcastIsAppBroadcasting();
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern NativeBool XAppBroadcastIsAppBroadcasting();

        //STDAPI XAppBroadcastRegisterIsAppBroadcastingChanged(
        //    _In_opt_ XTaskQueueHandle queue,
        //    _In_opt_ void* context,
        //    _In_ XAppBroadcastMonitorCallback* appBroadcastMonitorCallback,
        //    _Out_ XTaskQueueRegistrationToken* token);
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XAppBroadcastRegisterIsAppBroadcastingChanged(
            XTaskQueueHandle queue,
            IntPtr context,
            XAppBroadcastMonitorCallback callback,
            out XTaskQueueRegistrationToken token);

        //STDAPI_(bool) XAppBroadcastUnregisterIsAppBroadcastingChanged(
        //    _In_ XTaskQueueRegistrationToken token,
        //    _In_ bool wait);
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern NativeBool XAppBroadcastUnregisterIsAppBroadcastingChanged(XTaskQueueRegistrationToken token, NativeBool wait);

        //STDAPI XAppCaptureMetadataAddStringEvent(_In_z_ const char* name,
        //    _In_z_ const char* value,
        //    _In_ XAppCaptureMetadataPriority priority);
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XAppCaptureMetadataAddStringEvent(
            Byte[] name,
            Byte[] value,
            XAppCaptureMetadataPriority priority);

        //STDAPI XAppCaptureMetadataAddInt32Event(_In_z_ const char* name,
        //    _In_ int32_t value,
        //    _In_ XAppCaptureMetadataPriority priority);
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XAppCaptureMetadataAddInt32Event(
            Byte[] name,
            Int32 value,
            XAppCaptureMetadataPriority priority);

        //STDAPI XAppCaptureMetadataAddDoubleEvent(_In_z_ const char* name,
        //    _In_ double value,
        //    _In_ XAppCaptureMetadataPriority priority);
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XAppCaptureMetadataAddDoubleEvent(
            Byte[] name,
            double value,
            XAppCaptureMetadataPriority priority);

        //STDAPI XAppCaptureMetadataStartStringState(_In_z_ const char* name,
        //    _In_z_ const char* value,
        //    _In_ XAppCaptureMetadataPriority priority);
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XAppCaptureMetadataStartStringState(
            Byte[] name,
            Byte[] value,
            XAppCaptureMetadataPriority priority);

        //STDAPI XAppCaptureMetadataStartInt32State(_In_z_ const char* name,
        //    _In_ int32_t value,
        //    _In_ XAppCaptureMetadataPriority priority);
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XAppCaptureMetadataStartInt32State(
            Byte[] name,
            Int32 value,
            XAppCaptureMetadataPriority priority);

        //STDAPI XAppCaptureMetadataStartDoubleState(_In_z_ const char* name,
        //    _In_ double value,
        //    _In_ XAppCaptureMetadataPriority priority);
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XAppCaptureMetadataStartDoubleState(
            Byte[] name,
            double value,
            XAppCaptureMetadataPriority priority);

        //STDAPI XAppCaptureMetadataStopState(_In_z_ const char* name);
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XAppCaptureMetadataStopState(
            Byte[] name);

        //STDAPI XAppCaptureMetadataStopAllStates();
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XAppCaptureMetadataStopAllStates();

        //STDAPI XAppCaptureMetadataRemainingStorageBytesAvailable(_Out_ uint64_t* value);
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XAppCaptureMetadataRemainingStorageBytesAvailable(out UInt64 value);

        //STDAPI XAppCaptureRegisterMetadataPurged(
        //    _In_opt_ XTaskQueueHandle queue,
        //    _In_ void* context,
        //    _In_ XAppCaptureMetadataPurgedCallback* callback,
        //    _Out_ XTaskQueueRegistrationToken* token);
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XAppCaptureRegisterMetadataPurged(
            XTaskQueueHandle queue,
            IntPtr context,
            XAppCaptureMetadataPurgedCallback callback,
            out XTaskQueueRegistrationToken token);

        //STDAPI_(bool) XAppCaptureUnRegisterMetadataPurged(_In_ XTaskQueueRegistrationToken token, _In_ bool wait);
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern NativeBool XAppCaptureUnRegisterMetadataPurged(XTaskQueueRegistrationToken token, NativeBool wait);

        //STDAPI XAppCaptureTakeDiagnosticScreenshot(
        //_In_ bool gamescreenOnly,
        //_In_ XAppCaptureScreenshotFormatFlag captureFlags,
        //_In_opt_ const char* filenamePrefix,
        //_Out_ XAppCaptureDiagnosticScreenshotResult* result
        //);
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XAppCaptureTakeDiagnosticScreenshot(
            bool gamescreenOnly,
            XAppCaptureScreenshotFormatFlag captureFlags,
            Byte[] filenamePrefix,
            out XAppCaptureDiagnosticScreenshotResult result);

        //STDAPI XAppCaptureRecordDiagnosticClip(
        //    _In_ time_t startTime,
        //    _In_ uint32_t durationInMs,
        //    _In_opt_ const char* filenamePrefix,
        //    _Out_ XAppCaptureRecordClipResult* result
        //);
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XAppCaptureRecordDiagnosticClip(
            TimeT startTime,
            UInt32 durationInMs,
            Byte[] filenamePrefix,
            out XAppCaptureRecordClipResult result);

        //STDAPI XAppCaptureTakeScreenshot(
        //    _In_ XUserHandle requestingUser,
        //    _Out_ XAppCaptureTakeScreenshotResult* result
        //);
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XAppCaptureTakeScreenshot(XUserHandle userHandle, out XAppCaptureTakeScreenshotResult result);

        //STDAPI XAppCaptureOpenScreenshotStream(
        //    _In_ const char* localId,
        //    _In_ XAppCaptureScreenshotFormatFlag screenshotFormat,
        //    _Out_ XAppCaptureScreenshotStreamHandle* handle,
        //    _Out_opt_ uint64_t* totalBytes);
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XAppCaptureOpenScreenshotStream(
            Byte[] localId,
            XAppCaptureScreenshotFormatFlag screenshotFormat,
            out IntPtr handle,
            out UInt64 totalBytes);

        //STDAPI XAppCaptureReadScreenshotStream(
        //    _In_ XAppCaptureScreenshotStreamHandle handle,
        //    _In_ uint64_t startPosition,
        //    _In_ uint32_t bytesToRead,
        //    _Out_writes_to_(bytesToRead, * bytesWritten) uint8_t* buffer,
        //    _Out_ uint32_t* bytesWritten);
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XAppCaptureReadScreenshotStream(
            IntPtr handle,
            UInt64 startPosition,
            UInt32 bytesToRead,
            [Out] Byte[] buffer,
            out Int32 bytesWritten);

        //STDAPI XAppCaptureCloseScreenshotStream(_In_ XAppCaptureScreenshotStreamHandle handle);
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XAppCaptureCloseScreenshotStream(IntPtr handle);

        //STDAPI XAppCaptureEnableRecord(); // enable recording and screenshots for user
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XAppCaptureEnableRecord();

        //STDAPI XAppCaptureDisableRecord(); // disable recording and screenshots for user
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XAppCaptureDisableRecord();
    }
}
