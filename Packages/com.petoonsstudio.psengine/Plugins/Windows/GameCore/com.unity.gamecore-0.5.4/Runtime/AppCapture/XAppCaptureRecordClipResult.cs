using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XAppCaptureRecordClipResult
    {
        public string path { get; }
        public ulong fileSize { get; }
        public DateTime startTime { get; }
        public UInt32 durationInMs { get; }
        public UInt32 width { get; }
        public UInt32 height { get; }
        public XAppCaptureVideoEncoding encoding { get; }

        internal XAppCaptureRecordClipResult(Interop.XAppCaptureRecordClipResult interop)
        {
            path = Converters.ByteArrayToString(interop.path);
            fileSize = interop.fileSize.ToUInt64();
            startTime = interop.startTime.DateTime;
            durationInMs = interop.durationInMs;
            width = interop.width;
            height = interop.height;
            encoding = interop.encoding;
        }
    }
}