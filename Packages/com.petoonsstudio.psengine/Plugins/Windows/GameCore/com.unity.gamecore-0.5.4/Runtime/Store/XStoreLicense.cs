using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XStoreLicense
    {
        internal XStoreLicense(XStoreLicenseHandle interopHandle)
        {
            Handle = interopHandle;
        }

        internal XStoreLicenseHandle Handle { get; set; }
    }
}
