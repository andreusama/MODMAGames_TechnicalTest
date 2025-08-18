using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PetoonsStudio.PSEngine.Framework;

namespace PetoonsStudio.PSEngine.EnGUI
{
    [RequireComponent(typeof(Button))]
    public class GUIDebugOption : MonoBehaviour
    {
        private Button m_Button;

        [SerializeField] private TextMeshProUGUI m_Text;

        private Action m_DebugAction;

        public Selectable Selectable => m_Button;

        private void Awake()
        {
            m_Button = GetComponent<Button>();
            m_Button.onClick.AddListener(ExecuteAction);
        }

        public void Setup(DebugOption option)
        {
            m_Text.text = option.Description;

            if (option.Action != null)
            {
                m_DebugAction = option.Action;
            }
        }

        private void ExecuteAction()
        {
            m_DebugAction?.Invoke();
        }
    }
}
