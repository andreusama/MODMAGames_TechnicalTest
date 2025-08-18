using System;

namespace Unity.GameCore
{
    public class XGameSaveUpdateHandle
    {
        internal XGameSaveUpdateHandle(Interop.XGameSaveUpdateHandle interopHandle)
        {
            this.InteropHandle = interopHandle;
        }

        internal static Int32 WrapInteropHandleAndReturnHResult(Int32 hresult, Interop.XGameSaveUpdateHandle interopHandle, out XGameSaveUpdateHandle userHandle)
        {
            if (HR.SUCCEEDED(hresult))
            {
                userHandle = new XGameSaveUpdateHandle(interopHandle);
            }
            else
            {
                userHandle = default(XGameSaveUpdateHandle);
            }
            return hresult;
        }

        internal Interop.XGameSaveUpdateHandle InteropHandle { get; }
    }
}
