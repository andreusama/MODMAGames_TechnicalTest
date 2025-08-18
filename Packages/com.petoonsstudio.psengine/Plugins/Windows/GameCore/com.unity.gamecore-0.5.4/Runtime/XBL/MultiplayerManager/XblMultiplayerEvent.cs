using System;
using System.Runtime.InteropServices;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblMultiplayerEvent
    {
        internal XblMultiplayerEvent(Interop.XblMultiplayerEvent interopStruct)
        {
            this.Result = interopStruct.Result;
            this.ErrorMessage = interopStruct.ErrorMessage.GetString();
            this.EventType = interopStruct.EventType;
            this.EventArgsHandle = new XblMultiplayerEventArgsHandle(interopStruct.EventArgsHandle);
            this.SessionType = interopStruct.SessionType;

            // Unwrap and unpin wrapped context pointer
            this.Context = null;
            if (interopStruct.Context != IntPtr.Zero)
            {
                var handle = GCHandle.FromIntPtr(interopStruct.Context);
                this.Context = handle.Target;
                handle.Free();
            }
        }

        public Int32 Result { get; }
        public string ErrorMessage { get; }
        public object Context { get; }
        public XblMultiplayerEventType EventType { get; }
        public XblMultiplayerEventArgsHandle EventArgsHandle { get; }
        public XblMultiplayerSessionType SessionType { get; }
    }
}