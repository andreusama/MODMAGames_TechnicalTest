using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.GameCore
{
    public class XPackageInstallationProgress
    {
        internal XPackageInstallationProgress(Interop.XPackageInstallationProgress rawProgress)
        {
            this.TotalBytes = rawProgress.totalBytes;
            this.InstalledBytes = rawProgress.installedBytes;
            this.LaunchBytes = rawProgress.launchBytes;
            this.Launchable = rawProgress.launchable;
            this.Completed = rawProgress.completed;
        }

        public UInt64 TotalBytes { get; }
        public UInt64 InstalledBytes { get; }
        public UInt64 LaunchBytes { get; }
        public bool Launchable { get; }
        public bool Completed { get; }
    }
}
