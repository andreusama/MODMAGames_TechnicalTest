namespace Unity.GameCore
{
    public class XUserGetTokenAndSignatureUtf16HttpHeader
    {
        public XUserGetTokenAndSignatureUtf16HttpHeader(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; }
        public string Value { get; }
    }
}
