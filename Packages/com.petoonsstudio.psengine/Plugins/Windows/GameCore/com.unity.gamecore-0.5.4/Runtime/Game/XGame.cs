using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    partial class SDK
    {
        public static Int32 XGameGetXboxTitleId(out UInt32 titleId)
        {
            return XGRInterop.XGameGetXboxTitleId(out titleId);
        }

        public static void XLaunchNewGame(string exePath, string args, XUserHandle defaultUser)
        {
            if ( defaultUser != null )
                XGRInterop.XLaunchNewGame( Converters.StringToNullTerminatedUTF8ByteArray(exePath), Converters.StringToNullTerminatedUTF8ByteArray(args), defaultUser.InteropHandle);
            else
                XGRInterop.XLaunchNewGame(Converters.StringToNullTerminatedUTF8ByteArray(exePath), Converters.StringToNullTerminatedUTF8ByteArray(args), new Interop.XUserHandle ());
        }
    }
}
