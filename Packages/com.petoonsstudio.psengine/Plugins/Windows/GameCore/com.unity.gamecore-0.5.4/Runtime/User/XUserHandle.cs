using System;
using System.Collections.Generic;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XUserHandle
    {
        internal XUserHandle(Interop.XUserHandle interopHandle)
        {
            this.InteropHandle = interopHandle;
        }

        internal static Int32 WrapAndReturnHResult(Int32 hresult, Interop.XUserHandle interopHandle, out XUserHandle handle)
        {
            if (HR.SUCCEEDED(hresult))
            {
                handle = new XUserHandle(interopHandle);
            }
            else
            {
                handle = default(XUserHandle);
            }
            return hresult;
        }

        internal void ClearInteropHandle()
        {
            this.InteropHandle = new Interop.XUserHandle();
        }

        public override bool Equals(object obj) => obj is XUserHandle userHandleObj && this.InteropHandle.Ptr == userHandleObj.InteropHandle.Ptr;
        public override int GetHashCode() => this.InteropHandle.Ptr.GetHashCode();
        public static bool operator ==(XUserHandle handle1, XUserHandle handle2) =>
            object.ReferenceEquals(handle1, null) ? object.ReferenceEquals(handle2, null) : handle1.Equals(handle2);
        public static bool operator !=(XUserHandle handle1, XUserHandle handle2) => !(handle1 == handle2);

        internal Interop.XUserHandle InteropHandle { get; private set; }
    }
}
