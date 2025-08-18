using PetoonsStudio.PSEngine.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PetoonsStudio.PSEngine.Input
{
    public abstract class InputIconRenderer : MonoBehaviour,
        PSEventListener<NewProviderEvent>, PSEventListener<MMRebindEvent>, PSEventListener<MMBindingsResetEvent>
    {
        [SerializeField] protected int m_IndexValue;
        [SerializeField] protected bool m_UseReference = true;
        [SerializeField, ConditionalHide("m_UseReference", false)] protected InputActionReference m_InputAction;
        [SerializeField, ConditionalHide("m_UseReference", true)] protected string m_InputActionPath;
        [SerializeField] protected TMP_Text m_KeyboardKeyText;
        [SerializeField] protected string m_MapName = "GAME";

        public Sprite CurrentSprite { get; protected set; }
        public UnityEvent<Sprite> OnSpriteUpdated;
        protected InputBinding m_CurrentBinding;

        private int m_PlayerID = 0;

        protected abstract void UpdateSprite(Sprite icon);
        protected abstract bool IsVisible();

        private const string m_InputActionConnector = "/";

        protected virtual void Awake()
        {
            if (m_UseReference)
            {
                m_InputActionPath = m_InputAction.name;
            }
        }

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

        protected void SetInputIcon()
        {
            BindingIconData bindingData = IconServiceProvider.Instance.GetBindingIcon(m_CurrentBinding, m_MapName);

            if (bindingData.IconSprite == null)
            {
                return;
            }

            CurrentSprite = bindingData.IconSprite;

            if (bindingData.IsKeyboardBinding)
            {
                if (string.IsNullOrEmpty(bindingData.BindingKeyText))
                {
                    m_KeyboardKeyText.gameObject.SetActive(false);
                }
                else
                {
                    m_KeyboardKeyText.gameObject.SetActive(true);
                    m_KeyboardKeyText.text = bindingData.BindingKeyText;
                    m_KeyboardKeyText.ForceMeshUpdate();
                }
            }
            else
            {
                m_KeyboardKeyText.gameObject.SetActive(false);
            }

            UpdateSprite(CurrentSprite);
            OnSpriteUpdated?.Invoke(CurrentSprite);
        }

        /// <summary>
        /// Update the current binding by getting the Input action asset, finding the action and showing the correct binding icon.
        /// </summary>
        public void UpdateCurrentBinding()
        {
            int count = 0;

            //Get the InputAction Asset from which we will take the actions bindings
            var inputActionAsset = InputManager.Instance.GetInputAssetFromPlayerID(m_PlayerID);

            //Update the Input Action asset if there are Rebind controller and changes for the PlayerID
            InputManager.Instance.UpdateInputAssetWithRebindChanges(inputActionAsset, m_PlayerID);

            foreach (var binding in inputActionAsset.FindAction(m_InputActionPath).bindings)
            {
                if (InputManager.Instance.IsBindingMatchingCurrentScheme(binding))
                {
                    m_CurrentBinding = binding;

                    if (count >= m_IndexValue)
                    {
                        SetInputIcon();
                        break;
                    }
                    count++;
                }
            }
        }

        /// <summary>
        /// Set the Action that will show this renderer and update the binding.
        /// </summary>
        /// <param name="action"></param>
        public void SetAction(InputActionReference action)
        {
            m_InputActionPath = action.name;

            UpdateCurrentBinding();
        }

        /// <summary>
        /// Set the Action that will show this renderer and update the binding.
        /// </summary>
        /// <param name="action"></param>
        public void SetAction(string action)
        {
            m_InputActionPath = action;

            UpdateCurrentBinding();
        }

        /// <summary>
        /// Set the playerID that will show this renderer and update the binding.
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="update"></param>
        public void SetPlayerID(int playerID)
        {
            m_PlayerID = playerID;
            
            UpdateCurrentBinding();
        }

        protected bool IsSameInputAction(InputAction action, string actionPath)
        {
            return (action.actionMap.name + m_InputActionConnector + action.name) == actionPath;
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
            if (IsSameInputAction(eventType.Action, m_InputActionPath) && eventType.PlayerID == m_PlayerID || eventType.PlayerID == -1)
                UpdateCurrentBinding();
        }

        public void OnPSEvent(MMBindingsResetEvent eventType)
        {
            if (eventType.PlayerID == m_PlayerID || eventType.PlayerID == -1)
                UpdateCurrentBinding();
        }
    }
}