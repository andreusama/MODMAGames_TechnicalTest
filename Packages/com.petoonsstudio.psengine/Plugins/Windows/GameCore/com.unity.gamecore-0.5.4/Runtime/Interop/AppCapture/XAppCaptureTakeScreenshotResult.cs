using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //struct XAppCaptureTakeScreenshotResult
    //{
    //    _Field_z_ char localId[APPCAPTURE_MAX_LOCALID_LENGTH];
    //    XAppCaptureScreenshotFormatFlag availableScreenshotFormats;
    //};

    [StructLayout(LayoutKind.Sequential)]
    internal struct XAppCaptureTakeScreenshotResult
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = XGRInterop.APPCAPTURE_MAX_LOCALID_LENGTH)]
        internal Byte[] localId;
        internal XAppCaptureScreenshotFormatFlag availableScreenshotFormats;
    }
}
