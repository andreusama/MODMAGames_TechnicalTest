using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

namespace PetoonsStudio.PSEngine.Utils
{
    public class ButtonHold : MonoBehaviour
    {
        [SerializeField] private InputActionReference m_Action;
        [SerializeField] private float m_HoldTime = 1f;

        [Header("References")]
        [SerializeField] private Image m_Icon;
        [SerializeField] private Image m_Filler;
        [SerializeField] private TextMeshProUGUI m_Label;

        [Header("Event")]
        [SerializeField] private UnityEvent OnHoldEvent;

        protected bool m_Skipping;
        protected float m_CurrentHoldTime;

        /// <summary>
        /// On Enable
        /// </summary>
        protected virtual void OnEnable()
        {
            m_Action.action.performed += SkipAction_performed;
            m_Action.action.canceled += SkipAction_canceled;

            m_Action.action.Enable();
        }

        /// <summary>
        /// On Disable
        /// </summary>
        protected virtual void OnDisable()
        {
            m_Action.action.performed -= SkipAction_performed;
            m_Action.action.canceled -= SkipAction_canceled;

            m_Action.action.Disable();
        }

        /// <summary>
        /// Skip Performed
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void SkipAction_performed(InputAction.CallbackContext obj)
        {
            m_Icon.enabled = true;
            m_Filler.enabled = true;
            m_Label.enabled = true;

            m_Skipping = true;
        }

        /// <summary>
        /// Skip Canceled
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void SkipAction_canceled(InputAction.CallbackContext obj)
        {
            m_Skipping = false;

            m_Icon.enabled = false;
            m_Filler.enabled = false;
            m_Label.enabled = false;

            m_Filler.fillAmount = 0f;
            m_CurrentHoldTime = 0f;
        }

        /// <summary>
        /// Update
        /// </summary>
        void Update()
        {
            if (m_Skipping)
            {
                m_CurrentHoldTime += Time.deltaTime;
                float mapValue = MMMaths.Remap(m_CurrentHoldTime, 0f, m_HoldTime, 0f, 1f);
                m_Filler.fillAmount = mapValue;

                if (m_CurrentHoldTime > m_HoldTime)
                {
                    m_Skipping = false;

                    OnHoldEvent.Invoke();

                    m_Icon.enabled = false;
                    m_Filler.enabled = false;
                    m_Label.enabled = false;

                    m_Filler.fillAmount = 0f;
                    m_CurrentHoldTime = 0f;
                }
            }
        }
    }
}
