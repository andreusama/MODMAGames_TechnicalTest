using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using RTLTMPro;
using System;
using KBCore.Refs;

namespace PetoonsStudio.PSEngine.Utils
{
    [RequireComponent(typeof(TMP_Text))]
    public class RTLModifier : MonoBehaviour
    {
        [SerializeField, Self] private TMP_Text m_Text;

        protected readonly FastStringBuilder finalText = new FastStringBuilder(RTLSupport.DefaultBufferSize);
        
        private void OnValidate()
        {
            this.ValidateRefs();
        }

        void OnEnable()
        {
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
        }

        void OnDisable()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
        }

        void OnTextChanged(UnityEngine.Object obj)
        {
            if (m_Text != null && TextUtils.IsRTLInput(m_Text.text))
            {
                finalText.Clear();
                RTLSupport.FixRTL(m_Text.text, finalText);
                finalText.Reverse();
                m_Text.text = finalText.ToString();
            }
        }

        private void OnLocaleChanged(UnityEngine.Localization.Locale obj)
        {
            RTLMetadata data = obj.Metadata.GetMetadata<RTLMetadata>();
            
            if(data == null)
            {
                m_Text.isRightToLeftText = false;
            }
            else
            {
                m_Text.isRightToLeftText = data.IsRTL;
            }
        }
    }
}

