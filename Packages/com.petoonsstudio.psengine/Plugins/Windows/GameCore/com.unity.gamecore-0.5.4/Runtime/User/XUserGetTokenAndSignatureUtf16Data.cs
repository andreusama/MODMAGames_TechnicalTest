namespace Unity.GameCore
{
    public class XUserGetTokenAndSignatureUtf16Data
    {
        internal XUserGetTokenAndSignatureUtf16Data(string token, string signature)
        {
            this.Token = token;
            this.Signature = signature;
        }

        public string Token { get; }
        public string Signature { get; }
    }
}
