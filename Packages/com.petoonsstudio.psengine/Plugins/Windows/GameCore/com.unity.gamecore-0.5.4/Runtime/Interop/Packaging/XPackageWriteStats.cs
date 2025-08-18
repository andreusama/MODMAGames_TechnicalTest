using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //struct XPackageWriteStats
    //{
    //    uint64_t interval;
    //    uint64_t budget;
    //    uint64_t elapsed;
    //    uint64_t bytesWritten;
    //};
    internal struct XPackageWriteStats
    {
        internal readonly ulong interval;
        internal readonly ulong budget;
        internal readonly ulong elapsed;
        internal readonly ulong bytesWritten;

        internal XPackageWriteStats(Unity.GameCore.XPackageWriteStats publicObject)
        {
            this.interval = publicObject.Interval;
            this.budget = publicObject.Budget;
            this.elapsed = publicObject.Elapsed;
            this.bytesWritten = publicObject.BytesWritten;
        }
    }
}
