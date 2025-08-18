using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    //enum class HCTraceLevel : uint32_t
    //{
    //    Off = HC_PRIVATE_TRACE_LEVEL_OFF,
    //    Error = HC_PRIVATE_TRACE_LEVEL_ERROR,
    //    Warning = HC_PRIVATE_TRACE_LEVEL_WARNING,
    //    Important = HC_PRIVATE_TRACE_LEVEL_IMPORTANT,
    //    Information = HC_PRIVATE_TRACE_LEVEL_INFORMATION,
    //    Verbose = HC_PRIVATE_TRACE_LEVEL_VERBOSE,
    //};
    public enum HCTraceLevel
    {
        Off = 0,
        Error = 1,
        Warning = 2,
        Important = 3,
        Information = 4,
        Verbose = 5,
    }
}
