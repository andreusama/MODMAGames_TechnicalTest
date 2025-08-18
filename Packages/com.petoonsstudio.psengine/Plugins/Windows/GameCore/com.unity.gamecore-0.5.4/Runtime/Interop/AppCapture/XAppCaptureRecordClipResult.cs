using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //struct XAppCaptureRecordClipResult
    //{
    //    _Field_z_ char path[MAX_PATH];
    //    size_t fileSize;
    //    time_t startTime;
    //    uint32_t durationInMs;
    //    uint32_t width;
    //    uint32_t height;
    //    XAppCaptureVideoEncoding encoding;
    //};

    [StructLayout(LayoutKind.Sequential)]
    internal struct XAppCaptureRecordClipResult
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = XGRInterop.APPCAPTURE_MAX_PATH)]
        internal Byte[] path;
        internal SizeT fileSize;
        internal TimeT startTime;
        internal UInt32 durationInMs;
        internal UInt32 width;
        internal UInt32 height;
        internal XAppCaptureVideoEncoding encoding;
    }
}
