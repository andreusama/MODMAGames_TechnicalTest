using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public enum XStoreCanLicenseStatus : UInt32
    {
        NotLicensableToUser = 0,
        Licensable = 1
    }

    public class XStoreCanAcquireLicenseResult
    {
        internal XStoreCanAcquireLicenseResult(Interop.XStoreCanAcquireLicenseResult interopResult)
        {
            LicensableSku = Converters.ByteArrayToString(interopResult.licensableSku);
            Status = interopResult.status;
        }

        public string LicensableSku { get; }
        public XStoreCanLicenseStatus Status { get; }
    }
}
