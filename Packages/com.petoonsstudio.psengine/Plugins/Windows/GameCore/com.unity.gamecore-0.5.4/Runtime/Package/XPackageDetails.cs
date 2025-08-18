using System;

namespace Unity.GameCore
{
    public class XPackageDetails
    {
        internal XPackageDetails(Interop.XPackageDetails rawDetails)
        {
            this.PackageIdentifier = rawDetails.packageIdentifier.GetString();
            this.Version = new XVersion(rawDetails.version);
            this.Kind = rawDetails.kind;
            this.DisplayName = rawDetails.displayName.GetString();
            this.Description = rawDetails.description.GetString();
            this.Publisher = rawDetails.publisher.GetString();
            this.StoreId = rawDetails.storeId.GetString();
            this.Installing = rawDetails.installing;
        }

        public string PackageIdentifier { get; }
        public XVersion Version { get; }
        public XPackageKind Kind { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public string Publisher { get; }
        public string StoreId { get; }
        public bool Installing { get; }
    }
}
