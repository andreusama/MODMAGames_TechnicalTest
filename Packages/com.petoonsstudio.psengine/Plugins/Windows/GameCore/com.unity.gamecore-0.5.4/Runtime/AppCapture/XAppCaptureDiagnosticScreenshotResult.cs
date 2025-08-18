using System;

namespace Unity.GameCore
{
    public class XAppCaptureDiagnosticScreenshotResult
    {
        public XAppCaptureScreenshotFile[] files {get;}
        
        internal XAppCaptureDiagnosticScreenshotResult ( Interop.XAppCaptureDiagnosticScreenshotResult interop )
        {
            var count = interop.fileCount.ToUInt32();

            files = new XAppCaptureScreenshotFile[count];
             
            for (var i = 0; i < count; ++i)
            {
                files[i] = new XAppCaptureScreenshotFile(interop.files[i]);
            }
        }
    }
}