using System;

namespace Unity.GameCore
{
    public class XAppCaptureTakeScreenshotResult
    {
        public XAppScreenshotLocalId Id { get; }
        public XAppCaptureScreenshotFormatFlag Formats { get; }

        internal XAppCaptureTakeScreenshotResult(Interop.XAppCaptureTakeScreenshotResult interopResult)
        {
            Formats = interopResult.availableScreenshotFormats;
            Id = new XAppScreenshotLocalId (interopResult.localId);
        }
    }
}