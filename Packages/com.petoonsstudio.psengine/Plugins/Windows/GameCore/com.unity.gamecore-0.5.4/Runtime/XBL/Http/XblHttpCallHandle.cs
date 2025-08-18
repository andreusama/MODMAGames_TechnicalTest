using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.GameCore
{
    public class XblHttpCallHandle
    {
        internal XblHttpCallHandle(Interop.XblHttpCallHandle interopHandle)
        {
            this.InteropHandle = interopHandle;
        }

        internal static Int32 WrapInteropHandleAndReturnHResult(Int32 hresult, Interop.XblHttpCallHandle interopHandle, out XblHttpCallHandle handle)
        {
            if (HR.SUCCEEDED(hresult))
            {
                handle = new XblHttpCallHandle(interopHandle);
            }
            else
            {
                handle = default(XblHttpCallHandle);
            }
            return hresult;
        }

        internal Interop.XblHttpCallHandle InteropHandle { get; set; }
    }
}

