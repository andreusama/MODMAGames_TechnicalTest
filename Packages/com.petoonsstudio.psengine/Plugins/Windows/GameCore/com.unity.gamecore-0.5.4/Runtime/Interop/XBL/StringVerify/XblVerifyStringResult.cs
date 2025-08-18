﻿using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef struct XblVerifyStringResult
    //{
    //    XblVerifyStringResultCode resultCode;
    //    char* firstOffendingSubstring;
    //}
    //XblVerifyStringResult;

    [StructLayout(LayoutKind.Sequential)]
    internal struct XblVerifyStringResult
    {
        internal readonly XblVerifyStringResultCode resultCode;
        internal readonly UTF8StringPtr firstOffendingSubstring;
    }
}
