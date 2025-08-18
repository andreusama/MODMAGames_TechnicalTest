using Unity.GameCore;
using UnityEngine;
using UnityEngine.UI;

namespace SpeechSynthesizer
{
    public class StoreVoiceInformation : MonoBehaviour
    {
        public Text Description;
        public Text DisplayName;
        public Text Gender;
        public Text VoiceId;
        public Text Language;
        public XSpeechSynthesizerVoiceInformation VoiceInformation;
        public SpeechSynthesizerManager SpeechSynthesizerManager;

        void Start()
        {
            gameObject.GetComponent<Button>().onClick.AddListener(SetCustomVoiceButton);
        }

        void SetCustomVoiceButton()
        {
            SpeechSynthesizerManager.SpeechSynthesizerSetCustomVoice(this);
        }
    }
}
