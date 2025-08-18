using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //struct XSpeechSynthesizerVoiceInformation
    //{
    //    _Field_z_ const char* Description;
    //    _Field_z_ const char* DisplayName;
    //    XSpeechSynthesizerVoiceGender Gender;
    //    _Field_z_ const char* VoiceId;
    //    _Field_z_ const char* Language;
    //};
    [StructLayout(LayoutKind.Sequential)]
    internal struct XSpeechSynthesizerVoiceInformation
    {
        internal UTF8StringPtr Description;
        internal UTF8StringPtr DisplayName;
        internal XSpeechSynthesizerVoiceGender Gender;
        internal UTF8StringPtr VoiceId;
        internal UTF8StringPtr Language;     
    }
}
