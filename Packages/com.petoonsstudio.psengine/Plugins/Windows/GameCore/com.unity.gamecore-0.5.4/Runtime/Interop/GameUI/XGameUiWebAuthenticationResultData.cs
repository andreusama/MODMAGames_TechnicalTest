namespace Unity.GameCore.Interop
{
    internal struct XGameUiWebAuthenticationResultData
    {
        internal readonly int responseStatus;
        internal readonly SizeT responseCompletionUriSize;
        internal readonly UTF8StringPtr responseCompletionUri;
    }
}
