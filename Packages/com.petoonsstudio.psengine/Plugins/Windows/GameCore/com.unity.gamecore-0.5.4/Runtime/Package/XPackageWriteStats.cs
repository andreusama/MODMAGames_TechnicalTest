using System;

namespace Unity.GameCore
{
    public class XPackageWriteStats
    {
        internal XPackageWriteStats(Interop.XPackageWriteStats interopStruct)
        {
            this.Interval = interopStruct.interval;
            this.Budget = interopStruct.budget;
            this.Elapsed = interopStruct.elapsed;
            this.BytesWritten = interopStruct.bytesWritten;
        }

        public ulong Interval { get; }

        public ulong Budget { get; }

        public ulong Elapsed { get; }

        public ulong BytesWritten { get; }
    }
}
