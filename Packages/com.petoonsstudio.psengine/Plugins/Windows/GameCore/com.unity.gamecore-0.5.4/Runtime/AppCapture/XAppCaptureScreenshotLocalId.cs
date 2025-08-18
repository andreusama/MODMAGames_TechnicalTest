using System;

namespace Unity.GameCore
{
    [System.Serializable]
    public class XAppScreenshotLocalId
    {
        public byte[] Value {get;}

        public XAppScreenshotLocalId( byte[] value )
        {
            Value = value;
        }
    }
}