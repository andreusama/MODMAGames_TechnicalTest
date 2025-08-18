using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    //enum class XblMultiplayerInitializationStage : uint32_t
    //{
    //    Unknown,
    //    None,
    //    Joining,
    //    Measuring,
    //    Evaluating,
    //    Failed
    //};
    public enum XblMultiplayerInitializationStage : UInt32
    {
        Unknown,
        None,
        Joining,
        Measuring,
        Evaluating,
        Failed
    }
}
