using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    partial class SDK
    {
        public static bool XThreadIsTimeSensitive()
        {
            return XGRInterop.XThreadIsTimeSensitive().Value;
        }

        public static Int32 XThreadSetTimeSensitive(bool isTimeSensitiveThread)
        {
            return XGRInterop.XThreadSetTimeSensitive(new NativeBool(isTimeSensitiveThread));
        }

        public static void XThreadAssertNotTimeSensitive()
        {
            XGRInterop.XThreadAssertNotTimeSensitive();
        }
    }
}
