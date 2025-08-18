using System;
using System.Collections.Generic;
using Unity.GameCore;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SpeechSynthesizer
{
    public class SpeechSynthesizeSceneManager : MonoBehaviour
    {
        public GameObject m_VoiceInformationDash;
        public List<GameObject> m_VoiceInformationUIDashGO = new List<GameObject>();
        public ScrollRect m_VoiceInformationUIScrollRect;

        public AudioSource m_AudioSource;

        public Text m_UserText;
        public Text m_SsmlText;

        public bool m_SpeechSynthesizeCreated;

        [SerializeField]
        EventSystem m_EventSystem;

        public void CreateVoiceInformationDash(List<XSpeechSynthesizerVoiceInformation> voiceInformationList, SpeechSynthesizerManager speechSynthesizerManager)
        {
            foreach (var item in voiceInformationList)
            {
                GameObject tempObject = Instantiate(m_VoiceInformationDash);
                m_VoiceInformationUIDashGO.Add(tempObject);
                tempObject.transform.SetParent(m_VoiceInformationUIScrollRect.content.gameObject.transform, false);
                tempObject.SetActive(true);

                StoreVoiceInformation storeVoiceInformation = tempObject.gameObject.GetComponent<StoreVoiceInformation>();
                storeVoiceInformation.SpeechSynthesizerManager = speechSynthesizerManager;
                storeVoiceInformation.Description.text = item.Description;
                storeVoiceInformation.DisplayName.text = item.DisplayName;
                storeVoiceInformation.Gender.text = item.Gender.ToString();
                storeVoiceInformation.VoiceId.text = item.VoiceId;
                storeVoiceInformation.Language.text = item.Language;
                storeVoiceInformation.VoiceInformation = item;
            }
            m_VoiceInformationUIScrollRect.verticalNormalizedPosition = 0;
        }

        public string SsmlUI(string voice)
        {
            //https://docs.microsoft.com/en-us/dotnet/api/system.speech.synthesis.speechsynthesizer.speakssml?view=netframework-4.8

            DateTime dt = DateTime.Now;

            string language = voice;

            string str = "<speak version=\"1.0\"\n";
            str += " xmlns=\"http://www.w3.org/2001/10/synthesis\"\n";
            str += " xml:lang=\"" + language + "\"\n >";
            str += "<say-as type=\"date:mdy\"> " + dt.Date + "</say-as>\n";
            str += "</speak>";

            m_SsmlText.text = str;
            return str;
        }

        public void DestroyDash()
        {
            foreach (var item in m_VoiceInformationUIDashGO)
            {
                Destroy(item);
            }
            m_VoiceInformationUIDashGO.Clear();
        }
    }
}
