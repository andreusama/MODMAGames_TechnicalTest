using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    internal partial class XblInterop
    {
        // STDAPI XblTitleManagedStatsWriteAsync(
        //     _In_ XblContextHandle xblContextHandle,
        //     _In_ uint64_t xboxUserId,
        //     _In_ const XblTitleManagedStatistic* statistics,
        //     _In_ size_t statisticsCount,
        //     _In_ XAsyncBlock* async
        // ) XBL_NOEXCEPT;

        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XblTitleManagedStatsWriteAsync(
            XblContextHandle xblContextHandle,
            UInt64 xboxUserId,
            [In] XblTitleManagedStatistic[] statistics,
            SizeT statisticsCount,
            XAsyncBlockPtr async);
    }
}
