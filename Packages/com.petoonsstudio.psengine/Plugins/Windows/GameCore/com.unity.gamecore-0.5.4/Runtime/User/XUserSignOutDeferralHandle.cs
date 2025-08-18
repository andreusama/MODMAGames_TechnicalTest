namespace Unity.GameCore
{
    public class XUserSignOutDeferralHandle
    {
        internal XUserSignOutDeferralHandle(Interop.XUserSignOutDeferralHandle interopHandle)
        {
            this.InteropHandle = interopHandle;
        }

        internal Interop.XUserSignOutDeferralHandle InteropHandle { get; }
    }
}
