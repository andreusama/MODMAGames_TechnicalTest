using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    partial class SDK
    {
        public static Int32 XLaunchUri(XUserHandle requestingUser, string uri)
        {
            if (requestingUser == null)
            {
                return HR.E_INVALIDARG;
            }

            return XGRInterop.XLaunchUri(requestingUser.InteropHandle, Converters.StringToNullTerminatedUTF8ByteArray(uri));
        }
    }
}
