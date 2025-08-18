using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XStorePackageUpdate
    {
        internal XStorePackageUpdate(Interop.XStorePackageUpdate interopPackageUpdate)
        {
            PackageIdentifier = Converters.ByteArrayToString(interopPackageUpdate.packageIdentifier);
            IsMandatory = interopPackageUpdate.isMandatory;
        }

        public string PackageIdentifier { get; }
        public bool IsMandatory { get; }
    }
}
