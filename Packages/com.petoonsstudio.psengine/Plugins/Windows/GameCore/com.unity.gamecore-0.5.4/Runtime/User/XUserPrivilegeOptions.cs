using System;

namespace Unity.GameCore
{
    [Flags]
    public enum XUserPrivilegeOptions : UInt32
    {
        None = 0x00,
        AllUsers = 0x01,
    }
}
