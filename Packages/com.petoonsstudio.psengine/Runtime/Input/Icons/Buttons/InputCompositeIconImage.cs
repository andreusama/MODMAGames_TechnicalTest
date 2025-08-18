using PetoonsStudio.PSEngine.Input;
using PetoonsStudio.PSEngine.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PetoonsStudio.PSEngine.Input
{
    [System.Serializable]
    public struct InputIconData
    {
        public Image m_Image;
        public TMP_Text m_KeyboardKeyText;
    }

    public class InputCompositeIconImage : MonoBehaviour,
        PSEventListener<NewProviderEvent>, PSEventListener<MMRebindEvent>, PSEventListener<MMBindingsResetEvent>
    {
        [SerializeField, Tooltip("Starts at 0.")] protected int m_PlayerID = 0;
        [SerializeField] protected int m_IndexValue;
        [SerializeField] protected InputActionReference m_InputAction;
        [SerializeField] protected string m_MapName;

        [Header("Icons")]
        [SerializeField] protected InputIconData m_Input;
        [SerializeField] protected InputIconData m_Modifier;

        void OnEnable()
        {
            this.PSEventStartListening<NewProviderEvent>();
            this.PSEventStartListening<MMRebindEvent>();
            this.PSEventStartListening<MMBindingsResetEvent>();

            UpdateCurrentBinding();
        }

        void OnDisable()
        {
            this.PSEventStopListening<NewProviderEvent>();
            this.PSEventStopListening<MMRebindEvent>();
            this.PSEventStopListening<MMBindingsResetEvent>();
        }

        public void UpdateCurrentBinding()
        {
            foreach (var binding in InputManager.Instance.InputAsset.FindAction(m_InputAction.name).bindings)
            {
                if (InputManager.Instance.IsBindingMatchingCurrentScheme(binding) && binding.name == "Modifier")
                {
                    SetInputIcon(binding, ref m_Modifier);
                }

                if (InputManager.Instance.IsBindingMatchingCurrentScheme(binding) && binding.name == "Button")
                {
                    SetInputIcon(binding, ref m_Input);
                }
            }
        }

        protected virtual void SetInputIcon(InputBinding binding, ref InputIconData input)
        {
            BindingIconData bindingData = IconServiceProvider.Instance.GetBindingIcon(binding, m_MapName);

            if (bindingData.IconSprite == null)
                return;

            if (bindingData.IsKeyboardBinding)
            {
                if (string.IsNullOrEmpty(bindingData.BindingKeyText))
                {
                    input.m_KeyboardKeyText.gameObject.SetActive(false);
                }
                else
                {
                    input.m_KeyboardKeyText.gameObject.SetActive(true);
                    input.m_KeyboardKeyText.text = bindingData.BindingKeyText;
                }
            }
            else
            {
                input.m_KeyboardKeyText.gameObject.SetActive(false);
            }

            input.m_Image.sprite = bindingData.IconSprite;
        }

        public void SetAction(InputActionReference action)
        {
            m_InputAction = action;
        }

        public void OnPSEvent(NewProviderEvent eventType)
        {
            if (eventType.NewProvider != null)
            {
                UpdateCurrentBinding();
            }
        }

        public void OnPSEvent(MMRebindEvent eventType)
        {
            //Actions with different bindings are not equal (this caused problems when swapping)
            if (eventType.Action.name == m_InputAction.action.name)
                UpdateCurrentBinding();
        }

        public void OnPSEvent(MMBindingsResetEvent eventType)
        {
            if (eventType.PlayerID == m_PlayerID || eventType.PlayerID == -1)
                UpdateCurrentBinding();
        }
    }
}
