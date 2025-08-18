using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef struct XblMultiplayerSessionNumberAttribute
    //{
    //    char name[XBL_MULTIPLAYER_SEARCH_HANDLE_MAX_FIELD_LENGTH];
    //    double value;
    //}
    //XblMultiplayerSessionNumberAttribute;
    [StructLayout(LayoutKind.Sequential)]
    internal struct XblMultiplayerSessionNumberAttribute
    {
        private unsafe fixed Byte name[XblInterop.XBL_MULTIPLAYER_SEARCH_HANDLE_MAX_FIELD_LENGTH];
        internal readonly double value;

        internal string GetName() { unsafe { fixed (Byte* ptr = this.name) { return Converters.BytePointerToString(ptr, XblInterop.XBL_MULTIPLAYER_SEARCH_HANDLE_MAX_FIELD_LENGTH); } } }

        internal XblMultiplayerSessionNumberAttribute(Unity.GameCore.XblMultiplayerSessionNumberAttribute publicObject)
        {
            unsafe
            {
                fixed (Byte* ptr = this.name)
                {
                    Converters.StringToNullTerminatedUTF8FixedPointer(publicObject.Name, ptr, XblInterop.XBL_MULTIPLAYER_SEARCH_HANDLE_MAX_FIELD_LENGTH);
                }
            }

            value = publicObject.Value;
        }
    }
}