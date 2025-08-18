using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public partial class SDK
    {
        public static Int32 XClosedCaptionGetProperties(out XClosedCaptionProperties properties)
        {
            Int32 hr = XGRInterop.XClosedCaptionGetProperties(out Interop.XClosedCaptionProperties interopProperties);
            properties = new XClosedCaptionProperties(interopProperties);
            return hr;
        }

        public static Int32 XClosedCaptionSetEnabled(bool enabled)
        {
            return XGRInterop.XClosedCaptionSetEnabled(new NativeBool(enabled));
        }

        public static Int32 XHighContrastGetMode(out XHighContrastMode mode)
        {
            return XGRInterop.XHighContrastGetMode(out mode);
        }

        public static Int32 XSpeechToTextSendString(string speakerName, string content, XSpeechToTextType type)
        {
            return XGRInterop.XSpeechToTextSendString(
                Converters.StringToNullTerminatedUTF8ByteArray(speakerName),
                Converters.StringToNullTerminatedUTF8ByteArray(content),
                type);
        }

        public static Int32 XSpeechToTextSetPositionHint(XSpeechToTextPositionHint position)
        {
            return XGRInterop.XSpeechToTextSetPositionHint(position);
        }
    }
}
