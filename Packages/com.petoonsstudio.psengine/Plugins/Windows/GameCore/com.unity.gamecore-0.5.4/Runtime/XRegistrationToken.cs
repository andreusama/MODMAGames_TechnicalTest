using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore
{
    public class XRegistrationToken
    {
        internal XRegistrationToken(GCHandle callbackHandle, Interop.XTaskQueueRegistrationToken token)
        {
            CallbackHandle = callbackHandle;
            Token = token;
        }

        internal GCHandle CallbackHandle { get; }
        internal Interop.XTaskQueueRegistrationToken Token { get; }
    }
}
