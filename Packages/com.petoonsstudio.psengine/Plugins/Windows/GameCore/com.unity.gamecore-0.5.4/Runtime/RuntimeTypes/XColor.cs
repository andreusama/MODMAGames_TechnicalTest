using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XColor
    {
        internal XColor(Interop.XColor interopStruct)
        {
            this.A = interopStruct.A;
            this.R = interopStruct.R;
            this.G = interopStruct.G;
            this.B = interopStruct.B;
            this.Value = BitConverter.ToUInt32(new byte[] { interopStruct.A, interopStruct.R, interopStruct.G, interopStruct.B }, 0);
        }

        public Byte A { get; }
        public Byte R { get; }
        public Byte G { get; }
        public Byte B { get; }
        public UInt32 Value { get; }
    }
}
