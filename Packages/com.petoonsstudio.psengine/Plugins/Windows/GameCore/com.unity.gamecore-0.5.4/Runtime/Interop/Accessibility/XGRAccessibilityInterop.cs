using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    partial class XGRInterop
    {
        //STDAPI XClosedCaptionGetProperties(
        //    _Out_ XClosedCaptionProperties* properties
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XClosedCaptionGetProperties(
            out XClosedCaptionProperties properties);

        //STDAPI XClosedCaptionSetEnabled(
        //    _In_ bool enabled
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XClosedCaptionSetEnabled(
            NativeBool enabled);

        //STDAPI XHighContrastGetMode(
        //    _Out_ XHighContrastMode* mode
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XHighContrastGetMode(
            out XHighContrastMode mode);

        //STDAPI XSpeechToTextSendString(
        //    _In_z_ const char* speakerName,
        //    _In_z_ const char* content,
        //    _In_ XSpeechToTextType type
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XSpeechToTextSendString(
            byte[] speakerName,
            byte[] content,
            XSpeechToTextType type);

        //STDAPI XSpeechToTextSetPositionHint(
        //    _In_ XSpeechToTextPositionHint position
        //    ) noexcept;
        [DllImport(ThunkDllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 XSpeechToTextSetPositionHint(
            XSpeechToTextPositionHint position);
    }
}
