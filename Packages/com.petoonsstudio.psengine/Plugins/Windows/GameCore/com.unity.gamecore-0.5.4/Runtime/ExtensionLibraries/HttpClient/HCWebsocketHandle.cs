using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore
{
    public class HCWebsocketHandle
    {
        internal HCWebsocketHandle(Interop.HCWebsocketHandle interopHandle)
        {
            this.InteropHandle = interopHandle;
        }

        internal static Int32 WrapAndReturnHResult(Int32 hresult, Interop.HCWebsocketHandle interopHandle, out HCWebsocketHandle handle, GCHandle callbackHandle)
        {
            if (HR.SUCCEEDED(hresult))
            {
                handle = new HCWebsocketHandle(interopHandle);
                handle.cbHandle = callbackHandle;
            }
            else
            {
                if ( callbackHandle != null && callbackHandle.IsAllocated )
                    callbackHandle.Free();

                handle = default(HCWebsocketHandle);
            }
            return hresult;
        }

        internal void ClearInteropHandle()
        {
            this.InteropHandle = new Interop.HCWebsocketHandle();
        }

        public override bool Equals(object obj) => obj is HCWebsocketHandle userHandleObj && this.InteropHandle.Ptr == userHandleObj.InteropHandle.Ptr;
        public override int GetHashCode() => this.InteropHandle.Ptr.GetHashCode();
        public static bool operator ==(HCWebsocketHandle handle1, HCWebsocketHandle handle2) =>
            object.ReferenceEquals(handle1, null) ? object.ReferenceEquals(handle2, null) : handle1.Equals(handle2);
        public static bool operator !=(HCWebsocketHandle handle1, HCWebsocketHandle handle2) => !(handle1 == handle2);

        public HCWebSocketMessageFunction MessageFunction { get { return messageCallback; } }
        public HCWebSocketBinaryMessageFunction BinaryMessageFunction { get { return binaryMessageCallback; } }
        public HCWebSocketCloseEventFunction CloseFunction { get { return closeCallback; } }

        internal Interop.HCWebsocketHandle InteropHandle { get; set; }

        internal Interop.HCWebSocketMessageFunction messageFunc;
        internal Interop.HCWebSocketBinaryMessageFunction binaryMessageFunc;
        internal Interop.HCWebSocketCloseEventFunction closeFunc;

        internal HCWebSocketMessageFunction messageCallback;
        internal HCWebSocketBinaryMessageFunction binaryMessageCallback;
        internal HCWebSocketCloseEventFunction closeCallback;

        internal GCHandle cbHandle;
    }
}
