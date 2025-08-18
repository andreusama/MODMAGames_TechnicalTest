using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeBool
    {
        internal NativeBool(bool value)
        {
            this.value = (byte)(value ? 1 : 0);
        }

        internal bool Value => value != 0;

        private Byte value;
    }
}
