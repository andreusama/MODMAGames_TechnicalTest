using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XSystemAnalyticsInfo
    {
        internal XSystemAnalyticsInfo(Interop.XSystemAnalyticsInfo rawVersion)
        {
            OsVersion = new XVersion(rawVersion.osVersion);
            HostingOsVersion = new XVersion(rawVersion.hostingOsVersion);
            Family = Converters.ByteArrayToString(rawVersion.family);
            Form = Converters.ByteArrayToString(rawVersion.form);
        }

        public XVersion OsVersion { get; }
        public XVersion HostingOsVersion { get; }
        public string Family { get; }
        public string Form { get; }
    }
}