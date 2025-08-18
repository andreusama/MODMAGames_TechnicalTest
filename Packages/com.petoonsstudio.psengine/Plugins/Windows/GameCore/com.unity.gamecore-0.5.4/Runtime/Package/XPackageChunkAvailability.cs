using System;

namespace Unity.GameCore
{
    public enum XPackageChunkAvailability: UInt32
    {
        Ready,
        Pending,
        Installable,
        Unavailable
    }
}
