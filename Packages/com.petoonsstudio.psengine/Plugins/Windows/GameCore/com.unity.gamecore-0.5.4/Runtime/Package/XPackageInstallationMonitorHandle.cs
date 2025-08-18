using System;

namespace Unity.GameCore
{
    public class XPackageInstallationMonitorHandle
    {
        internal XPackageInstallationMonitorHandle(Interop.XPackageInstallationMonitorHandle interopHandle)
        {
            this.InteropHandle = interopHandle;
        }

        internal static Int32 WrapInteropHandleAndReturnHResult(
            Int32 hresult,
            Interop.XPackageInstallationMonitorHandle interopHandle,
            out XPackageInstallationMonitorHandle handle)
        {
            if (HR.SUCCEEDED(hresult))
            {
                handle = new XPackageInstallationMonitorHandle(interopHandle);
            }
            else
            {
                handle = default(XPackageInstallationMonitorHandle);
            }

            return hresult;
        }

        internal Interop.XPackageInstallationMonitorHandle InteropHandle { get; set; }
    }
}
