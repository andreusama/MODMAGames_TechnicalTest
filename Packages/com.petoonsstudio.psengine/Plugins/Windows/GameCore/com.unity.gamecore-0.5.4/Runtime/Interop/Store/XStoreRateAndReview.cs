using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //struct XStoreRateAndReviewResult
    //{
    //    bool wasUpdated;
    //};

    [StructLayout(LayoutKind.Sequential)]
    internal struct XStoreRateAndReview
    {
        [MarshalAs(UnmanagedType.U1)]
        internal bool wasUpdated;
    }
}
