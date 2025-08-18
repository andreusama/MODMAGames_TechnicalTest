﻿using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef struct XblAchievementTitleAssociation
    //{
    //    _Field_z_ const char* name;
    //    uint32_t titleId;
    //}
    //XblAchievementTitleAssociation;

    [StructLayout(LayoutKind.Sequential)]
    internal struct XblAchievementTitleAssociation
    {
        internal readonly UTF8StringPtr name;
        internal readonly UInt32 titleId;
    }
}
