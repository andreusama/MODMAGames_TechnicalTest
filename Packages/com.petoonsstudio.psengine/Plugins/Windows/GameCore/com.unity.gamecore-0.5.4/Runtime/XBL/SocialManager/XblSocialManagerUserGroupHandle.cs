using System;

namespace Unity.GameCore
{
    public class XblSocialManagerUserGroupHandle
    {
        internal XblSocialManagerUserGroupHandle(Interop.XblSocialManagerUserGroupHandle interopHandle)
        {
            this.InteropHandle = interopHandle;
        }

        internal static Int32 WrapAndReturnHResult(Int32 hresult, Interop.XblSocialManagerUserGroupHandle interopHandle, out XblSocialManagerUserGroupHandle handle)
        {
            if (HR.SUCCEEDED(hresult))
            {
                handle = new XblSocialManagerUserGroupHandle(interopHandle);
            }
            else
            {
                handle = default(XblSocialManagerUserGroupHandle);
            }
            return hresult;
        }

        internal void ClearInteropHandle()
        {
            this.InteropHandle = new Interop.XblSocialManagerUserGroupHandle();
        }

        internal Interop.XblSocialManagerUserGroupHandle InteropHandle { get; set; }
    }
}
