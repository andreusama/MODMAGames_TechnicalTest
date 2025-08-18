using UnityEngine;
using UnityEngine.InputSystem.UI;

namespace PetoonsStudio.PSEngine.Input
{
    public class UIInputModuleUpdateScheme : MonoBehaviour
    {
        [SerializeField] private InputSystemUIInputModule m_InputSystemModule;

        private void OnEnable()
        {
            UpdateScheme();
        }

        public void SetScheme(string scheme)
        {
            InputManager.Instance.ChangeBindingMask(scheme, m_InputSystemModule.actionsAsset);
        }

        private void UpdateScheme()
        {
            InputManager.Instance.UpdateBindingMask(m_InputSystemModule.actionsAsset);
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (m_InputSystemModule == null)
            {
                m_InputSystemModule = GetComponent<InputSystemUIInputModule>();
            }
        }
#endif
    }
}
