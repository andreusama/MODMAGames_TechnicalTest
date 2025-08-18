using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XVersion
    {
        internal XVersion(Interop.XVersion rawVersion)
        {
            this.Major = rawVersion.major;
            this.Minor = rawVersion.minor;
            this.Build = rawVersion.build;
            this.Revision = rawVersion.revision;
        }

        public UInt16 Major { get; }
        public UInt16 Minor { get; }
        public UInt16 Build { get; }
        public UInt16 Revision { get; }
    }
}
