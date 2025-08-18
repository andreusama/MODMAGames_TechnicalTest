using System;

namespace Unity.GameCore
{
    public class XblPresenceRecordHandle
    {
        internal XblPresenceRecordHandle(Interop.XblPresenceRecordHandle interopHandle)
        {
            this.InteropHandle = interopHandle;
        }

        internal static Int32 WrapInteropHandleAndReturnHResult(Int32 hresult, Interop.XblPresenceRecordHandle interopHandle, out XblPresenceRecordHandle handle)
        {
            if (HR.SUCCEEDED(hresult))
            {
                handle = new XblPresenceRecordHandle(interopHandle);
            }
            else
            {
                handle = default(XblPresenceRecordHandle);
            }
            return hresult;
        }

        internal Interop.XblPresenceRecordHandle InteropHandle { get; }
    }
}
