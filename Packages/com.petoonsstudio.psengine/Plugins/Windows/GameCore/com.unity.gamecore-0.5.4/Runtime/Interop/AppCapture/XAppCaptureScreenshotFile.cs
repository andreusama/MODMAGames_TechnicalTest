using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //struct XAppCaptureScreenshotFile
    //{
    //    _Field_z_ char path[MAX_PATH];
    //    size_t fileSize;
    //    uint32_t width;
    //    uint32_t height;
    //};
    [StructLayout(LayoutKind.Sequential)]
    internal struct XAppCaptureScreenshotFile
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = XGRInterop.APPCAPTURE_MAX_PATH)]
        internal Byte[] path;
        internal SizeT fileSize; // at present (up to 210600) the API returns 0 for this value
        internal UInt32 width;
        internal UInt32 height;
    };
}
