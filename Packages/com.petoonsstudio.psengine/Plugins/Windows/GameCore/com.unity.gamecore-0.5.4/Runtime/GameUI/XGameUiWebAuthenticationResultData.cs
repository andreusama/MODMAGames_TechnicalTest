namespace Unity.GameCore
{
    public class XGameUiWebAuthenticationResultData
    {
        public int responseStatus;
        public ulong responseCompletionUriSize;
        public string responseCompletionUri;

        internal XGameUiWebAuthenticationResultData(Interop.XGameUiWebAuthenticationResultData interopResult)
        {
            this.responseStatus = interopResult.responseStatus;
            this.responseCompletionUriSize = (ulong)interopResult.responseCompletionUriSize.ToUInt32();
            this.responseCompletionUri = interopResult.responseCompletionUri.GetString();
        }
    }
}
