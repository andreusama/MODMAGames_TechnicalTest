using System;

namespace Unity.GameCore
{
    public class XGameSaveContainerHandle
    {
        internal XGameSaveContainerHandle(Interop.XGameSaveContainerHandle interopHandle)
        {
            this.InteropHandle = interopHandle;
        }        

        internal static Int32 WrapInteropHandleAndReturnHResult(Int32 hresult, Interop.XGameSaveContainerHandle interopHandle, out XGameSaveContainerHandle userHandle)
        {
            if (HR.SUCCEEDED(hresult))
            {
                userHandle = new XGameSaveContainerHandle(interopHandle);
            }
            else
            {
                userHandle = default(XGameSaveContainerHandle);
            }
            return hresult;
        }

        internal Interop.XGameSaveContainerHandle InteropHandle { get; }
    }
}
