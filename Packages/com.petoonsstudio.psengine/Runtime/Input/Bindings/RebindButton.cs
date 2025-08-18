using PetoonsStudio.PSEngine.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PetoonsStudio.PSEngine.Input
{
    [RequireComponent(typeof(Button))]
    public class RebindButton : MonoBehaviour
    {
        [SerializeField, Tooltip("Should direct reference be used?")] private bool m_UseReference = true;
        [SerializeField, ConditionalHide("m_UseReference", false)] private InputActionReference m_ActionReference;
        [SerializeField, ConditionalHide("m_UseReference", true)] private string m_ActionReferencePath;
        
        [SerializeField] private int m_BindingIndex = 0;
        [SerializeField] private InputIconRenderer m_InputIconRenderer;

        public InputAction Action { get => m_ActionReference.action; }
        public int BindingIndex { get => m_BindingIndex; }

        void Start()
        {
            if (m_UseReference)
                m_InputIconRenderer.SetAction(m_ActionReference);
            else
                m_InputIconRenderer.SetAction(m_ActionReferencePath);
        }

        public void UpdateBindingIcon()
        {
            m_InputIconRenderer.UpdateCurrentBinding();
        }
    }
}

