using System;

namespace Unity.GameCore
{
    public struct XSpeechSynthesizerVoiceInformation
    {
        public string Description { get; }
        public string DisplayName { get; }
        public XSpeechSynthesizerVoiceGender Gender { get; }
        public string VoiceId { get; }
        public string Language { get; }

        internal XSpeechSynthesizerVoiceInformation(Interop.XSpeechSynthesizerVoiceInformation interop)
        {
            Description = interop.Description.GetString();
            DisplayName = interop.DisplayName.GetString();
            Gender = interop.Gender;
            VoiceId = interop.VoiceId.GetString();
            Language = interop.Language.GetString();
        }
    }    
}
