using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblVerifyStringResult
    {
        internal XblVerifyStringResult(Interop.XblVerifyStringResult interopStruct)
        {
            this.ResultCode = interopStruct.resultCode;
            this.FirstOffendingSubstring = interopStruct.firstOffendingSubstring.GetString();
        }

        public XblVerifyStringResultCode ResultCode { get; }
        public string FirstOffendingSubstring { get; }
    }
}
