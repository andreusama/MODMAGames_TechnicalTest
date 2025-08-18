using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    // typedef struct XblUserStatisticsResult
    // {
    //     uint64_t xboxUserId;
    //     XblServiceConfigurationStatistic* serviceConfigStatistics;
    //     uint32_t serviceConfigStatisticsCount;
    // } XblUserStatisticsResult;

    [StructLayout(LayoutKind.Sequential)]
    internal struct XblUserStatisticsResult
    {
        internal T[] GetServiceConfigStatistics<T>(Func<XblServiceConfigurationStatistic, T> ctor) =>
            Converters.PtrToClassArray(this.serviceConfigStatistics, this.serviceConfigStatisticsCount, ctor);

        internal readonly UInt64 xboxUserId;
        private readonly IntPtr serviceConfigStatistics;
        private readonly UInt32 serviceConfigStatisticsCount;
    }
}
