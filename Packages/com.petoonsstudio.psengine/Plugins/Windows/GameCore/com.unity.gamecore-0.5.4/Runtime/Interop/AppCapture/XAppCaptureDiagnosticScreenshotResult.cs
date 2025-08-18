using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //struct XAppCaptureDiagnosticScreenshotResult
    //{
    //    size_t fileCount;
    //    XAppCaptureScreenshotFile files[APPCAPTURE_MAX_CAPTURE_FILES];
    //};
    [StructLayout(LayoutKind.Sequential)]
    internal struct XAppCaptureDiagnosticScreenshotResult
    {
        internal SizeT fileCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = XGRInterop.APPCAPTURE_MAX_CAPTURE_FILES)]
        internal XAppCaptureScreenshotFile[] files;
    };
}
