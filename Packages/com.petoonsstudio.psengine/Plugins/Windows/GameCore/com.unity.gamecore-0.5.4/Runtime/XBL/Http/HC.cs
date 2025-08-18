using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public delegate void HCTraceCallback(Byte[] areaName, HCTraceLevel level, UInt64 threadId, UInt64 timestamp, Byte[] message);

    public partial class SDK
    {
        public partial class XBL
        {
            public static Int32 HCSettingsSetTraceLevel(
                HCTraceLevel traceLevel
                )
            {
                return XblInterop.HCSettingsSetTraceLevel(traceLevel);
            }

            public static Int32 HCSettingsGetTraceLevel(
                out HCTraceLevel traceLevel
                )
            {
                return XblInterop.HCSettingsGetTraceLevel(out traceLevel);
            }

            public static void HCTraceSetTraceToDebugger(
                bool traceToDebugger
                )
            {
                XblInterop.HCTraceSetTraceToDebugger(traceToDebugger);
            }
        }
    }
}
