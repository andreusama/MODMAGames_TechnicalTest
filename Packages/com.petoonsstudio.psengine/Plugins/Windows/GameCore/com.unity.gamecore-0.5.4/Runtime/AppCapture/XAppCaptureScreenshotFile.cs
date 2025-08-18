using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XAppCaptureScreenshotFile
    {
        public string path {get;}
        public ulong fileSize{get;}
        public uint width{get;}
        public uint height{get;}

        internal XAppCaptureScreenshotFile ( Interop.XAppCaptureScreenshotFile interop )
        {
            path = Converters.ByteArrayToString(interop.path);
            var size = interop.fileSize.ToInt32();
            fileSize = interop.fileSize.ToUInt64();
            width = interop.width;
            height = interop.height;
        }
    }
}