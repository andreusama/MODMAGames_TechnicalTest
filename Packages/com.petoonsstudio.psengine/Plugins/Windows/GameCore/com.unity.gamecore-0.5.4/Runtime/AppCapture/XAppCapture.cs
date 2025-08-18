using System;
using System.Runtime.InteropServices;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public delegate void XAppBroadcastMonitorCallback();

    public delegate void XAppCaptureMetadataPurgedCallback();

    partial class SDK
    {
        #region Callbacks
        [MonoPInvokeCallback]
        private static void XAppBroadcastMonitorCallback(IntPtr context)
        {
            GCHandle callbackHandle = GCHandle.FromIntPtr(context);
            var callbacks = callbackHandle.Target as UnmanagedCallback<Interop.XAppBroadcastMonitorCallback, XAppBroadcastMonitorCallback>;
            callbacks.userCallback?.Invoke();
        }

        [MonoPInvokeCallback]
        private static void XAppCaptureMetadataPurgedCallback(IntPtr context)
        {
            GCHandle callbackHandle = GCHandle.FromIntPtr(context);
            var callbacks = callbackHandle.Target as UnmanagedCallback<Interop.XAppCaptureMetadataPurgedCallback, XAppCaptureMetadataPurgedCallback>;
            callbacks.userCallback?.Invoke();
        }
        #endregion

        #region Broadcast
        public static Int32 XAppBroadcastShowUI(XUserHandle requestingUser)
        {
            if (requestingUser == null)
            {
                return HR.E_INVALIDARG;
            }

            return XGRInterop.XAppBroadcastShowUI(requestingUser.InteropHandle);
        }

        public static Int32 XAppBroadcastGetStatus(XUserHandle requestingUser, out XAppBroadcastStatus appBroadcastStatus)
        {
            if (requestingUser == null)
            {
                appBroadcastStatus = default;
                return HR.E_INVALIDARG;
            }

            var hr = XGRInterop.XAppBroadcastGetStatus(requestingUser.InteropHandle, out var interopResult);

            if (HR.SUCCEEDED(hr))
            {
                appBroadcastStatus = new XAppBroadcastStatus(interopResult);
            }
            else
            {
                appBroadcastStatus = default;
            }

            return hr;
        }

        public static bool XAppBroadcastIsAppBroadcasting()
        {
            return XGRInterop.XAppBroadcastIsAppBroadcasting().Value;
        }
        
        public static Int32 XAppBroadcastRegisterIsAppBroadcastingChanged(
            XAppBroadcastMonitorCallback callback,
            out XRegistrationToken token)
        {
            var callbacks = new UnmanagedCallback<Interop.XAppBroadcastMonitorCallback, XAppBroadcastMonitorCallback>
            {
                directCallback = XAppBroadcastMonitorCallback,
                userCallback = callback
            };

            GCHandle callbackHandle = GCHandle.Alloc(callbacks);

            int hr = XGRInterop.XAppBroadcastRegisterIsAppBroadcastingChanged(defaultQueue.handle, GCHandle.ToIntPtr(callbackHandle), callbacks.directCallback, out var taskQueueToken);

            if (HR.SUCCEEDED(hr))
            {
                token = new XRegistrationToken(callbackHandle, taskQueueToken);
            }
            else
            {
                token = default;
                callbackHandle.Free();
            }

            return hr;
        }

        public static bool XAppBroadcastUnregisterIsAppBroadcastingChanged(XRegistrationToken token)
        {
            if (token == null)
            {
                return false;
            }

            //TODO: investigate if it is possible to not not wait, given the resources we release here
            var result = XGRInterop.XAppBroadcastUnregisterIsAppBroadcastingChanged(token.Token, new NativeBool(true));
            token.CallbackHandle.Free();

            return result.Value;
        }
        #endregion

        #region metadata

        public static Int32 XAppCaptureMetadataAddStringEvent(
            string name,
            string value,
            XAppCaptureMetadataPriority priority)
        {
            return XGRInterop.XAppCaptureMetadataAddStringEvent(Converters.StringToNullTerminatedUTF8ByteArray(name), Converters.StringToNullTerminatedUTF8ByteArray(value), priority);
        }

        public static Int32 XAppCaptureMetadataAddInt32Event(
            string name,
            Int32 value,
            XAppCaptureMetadataPriority priority)
        {
            return XGRInterop.XAppCaptureMetadataAddInt32Event(Converters.StringToNullTerminatedUTF8ByteArray(name), value, priority);
        }

        public static Int32 XAppCaptureMetadataAddDoubleEvent(
            string name,
            double value,
            XAppCaptureMetadataPriority priority)
        {
            return XGRInterop.XAppCaptureMetadataAddDoubleEvent(Converters.StringToNullTerminatedUTF8ByteArray(name), value, priority);
        }

        public static Int32 XAppCaptureMetadataStartStringState(
            string name,
            string value,
            XAppCaptureMetadataPriority priority)
        {
            return XGRInterop.XAppCaptureMetadataStartStringState(Converters.StringToNullTerminatedUTF8ByteArray(name), Converters.StringToNullTerminatedUTF8ByteArray(value), priority);
        }
        public static Int32 XAppCaptureMetadataStartInt32State(
            string name,
            Int32 value,
            XAppCaptureMetadataPriority priority)
        {
            return XGRInterop.XAppCaptureMetadataStartInt32State(Converters.StringToNullTerminatedUTF8ByteArray(name), value, priority);
        }

        public static Int32 XAppCaptureMetadataStartDoubleState(
            string name,
            double value,
            XAppCaptureMetadataPriority priority)
        {
            return XGRInterop.XAppCaptureMetadataStartDoubleState(Converters.StringToNullTerminatedUTF8ByteArray(name), value, priority);
        }

        public static Int32 XAppCaptureMetadataStopState(string name)
        {
            return XGRInterop.XAppCaptureMetadataStopState(Converters.StringToNullTerminatedUTF8ByteArray(name));
        }

        public static Int32 XAppCaptureMetadataStopAllStates()
        {
            return XGRInterop.XAppCaptureMetadataStopAllStates();
        }

        public static Int32 XAppCaptureMetadataRemainingStorageBytesAvailable(out UInt64 value)
        {
            return XGRInterop.XAppCaptureMetadataRemainingStorageBytesAvailable(out value);
        }

        public static Int32 XAppCaptureRegisterMetadataPurged(
            XAppCaptureMetadataPurgedCallback callback,
            out XRegistrationToken token)
        {
            var callbacks = new UnmanagedCallback<Interop.XAppCaptureMetadataPurgedCallback, XAppCaptureMetadataPurgedCallback>
            {
                directCallback = XAppCaptureMetadataPurgedCallback,
                userCallback = callback
            };

            GCHandle callbackHandle = GCHandle.Alloc(callbacks);

            int hr = XGRInterop.XAppCaptureRegisterMetadataPurged(defaultQueue.handle, GCHandle.ToIntPtr(callbackHandle), callbacks.directCallback, out var taskQueueToken);

            if (HR.SUCCEEDED(hr))
            {
                token = new XRegistrationToken(callbackHandle, taskQueueToken);
            }
            else
            {
                token = default;
                callbackHandle.Free();
            }

            return hr;
        }

        public static bool XAppCaptureUnRegisterMetadataPurged(XRegistrationToken token)
        {
            if (token == null)
            {
                return false;
            }

            //TODO: investigate if it is possible to not not wait, given the resources we release here
            var result = XGRInterop.XAppBroadcastUnregisterIsAppBroadcastingChanged(token.Token, new NativeBool(true));
            token.CallbackHandle.Free();

            return result.Value;
        }

        #endregion

        #region Diagnostics

        public static Int32 XAppCaptureTakeDiagnosticScreenshot(
            bool gamescreenOnly,
            XAppCaptureScreenshotFormatFlag captureFlags,
            string filenamePrefix,
            out XAppCaptureDiagnosticScreenshotResult result
            )
        {
            var hr = XGRInterop.XAppCaptureTakeDiagnosticScreenshot(gamescreenOnly, captureFlags, filenamePrefix != null ? Converters.StringToNullTerminatedUTF8ByteArray(filenamePrefix) : null, out var interopResult);

            if (HR.SUCCEEDED(hr))
            {
                result = new XAppCaptureDiagnosticScreenshotResult(interopResult);
            }
            else
            {
                result = default;
            }

            return hr;
        }

        public static Int32 XAppCaptureRecordDiagnosticClip(
            System.DateTime startTime,
            uint durationInMS,
            string fileNamePrefix,
            out XAppCaptureRecordClipResult result
        )
        {
            // This needs to be a UTC date
            if (startTime.Kind != DateTimeKind.Utc)
            {
                startTime = startTime.ToUniversalTime();
            }

            var hr = XGRInterop.XAppCaptureRecordDiagnosticClip(new TimeT(startTime), durationInMS, fileNamePrefix != null ? Converters.StringToNullTerminatedUTF8ByteArray(fileNamePrefix) : null, out var interopResult);

            if (HR.SUCCEEDED(hr))
            {
                result = new XAppCaptureRecordClipResult(interopResult);
            }
            else
            {
                result = default;
            }
            
            return hr;
        }

        #endregion

        #region Screenshot
        public static Int32 XAppCaptureTakeScreenshot(XUserHandle userHandle, out XAppCaptureTakeScreenshotResult result)
        {
            if (userHandle == null)
            {
                result = default;
                return HR.E_INVALIDARG;
            }

            var hr = XGRInterop.XAppCaptureTakeScreenshot(userHandle.InteropHandle, out Interop.XAppCaptureTakeScreenshotResult interopResult);

            if (HR.SUCCEEDED(hr))
            {
                result = new XAppCaptureTakeScreenshotResult(interopResult);
            }
            else
            {
                result = default;
            }

            return hr;
        }

        public static Int32 XAppCaptureOpenScreenshotStream(XAppScreenshotLocalId id, XAppCaptureScreenshotFormatFlag screenshotFormat, out IntPtr handle, out ulong totalBytes)
        {
            return XGRInterop.XAppCaptureOpenScreenshotStream(id.Value, screenshotFormat, out handle, out totalBytes);
        }

        public static Int32 XAppCaptureReadScreenshotStream(IntPtr handle, UInt64 startPosition, UInt32 totalBytes, byte[] data, out Int32 bytesWritten)
        {
            return XGRInterop.XAppCaptureReadScreenshotStream(handle, startPosition, totalBytes, data, out bytesWritten);
        }

        public static Int32 XAppCaptureCloseScreenshotStream(IntPtr handle)
        {
            return XGRInterop.XAppCaptureCloseScreenshotStream(handle);
        }

        public static Int32 XAppCaptureEnableRecord()
        {
            return XGRInterop.XAppCaptureEnableRecord();
        }

        public static Int32 XAppCaptureDisableRecord()
        {
            return XGRInterop.XAppCaptureDisableRecord();
        }
        #endregion
    }
}