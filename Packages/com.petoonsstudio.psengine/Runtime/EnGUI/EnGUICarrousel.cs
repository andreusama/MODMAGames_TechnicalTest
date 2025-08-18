using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

namespace PetoonsStudio.PSEngine.EnGUI
{
    public abstract class EnGUICarrousel<T> : MonoBehaviour where T : EnGUICarrouselContent
    {
        [SerializeField] protected InputActionReference m_Next;
        [SerializeField] protected InputActionReference m_Previous;

        public delegate void OpenDelegate();
        public delegate void CloseDelegate();

        public event OpenDelegate OnOpen;
        public event CloseDelegate OnClose;

        protected bool m_IsAddressable;
        [SerializeField, Child] protected List<T> m_AppsContent;

        protected int m_CurrentContentIndex;
        protected int m_NextContentIndex;

        public abstract int ResolveInitialContent(object rule);

        protected virtual void Awake()
        {
            m_AppsContent = m_AppsContent.OrderBy(x => x.OrderValue).ToList();
        }

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public static async Task<EnGUICarrousel<T>> CreateCarrousel(AssetReferenceGameObject reference, Transform parent, object rule,
            EnGUIManager.PreviousBehaviour previousBehaviour)
        {
            var operation = Addressables.InstantiateAsync(reference, parent);

            await operation.Task;

            var content = operation.Result.GetComponent<EnGUICarrousel<T>>();

            content.m_IsAddressable = true;

            return content;
        }

        public virtual IEnumerator OpenCarrousel(object rule, EnGUIManager.PreviousBehaviour behaviour)
        {
            OnOpen?.Invoke();
            m_CurrentContentIndex = ResolveInitialContent(rule);
            m_AppsContent[m_CurrentContentIndex].Open(behaviour);

            yield return null;
        }

        public void CloseCarrousel()
        {
            StartCoroutine(CloseCarrousel_Internal());
        }

        public virtual IEnumerator CloseCarrousel_Internal()
        {
            EnableNavigation(false);

            m_AppsContent[m_CurrentContentIndex].Content.OnLoseControl += OnContentLoseControl;
            m_AppsContent[m_CurrentContentIndex].Close(false);

            yield return null;
        }

        protected virtual void OnContentLoseControl()
        {
            m_AppsContent[m_CurrentContentIndex].Content.OnLoseControl -= OnContentLoseControl;

            OnClose?.Invoke();

            if (m_IsAddressable)
                Addressables.ReleaseInstance(gameObject);
            else
                gameObject.SetActive(false);
        }

        protected virtual void PreviousAppContent(InputAction.CallbackContext obj)
        {
            PreviousAppContent();
        }

        protected virtual void PreviousAppContent()
        {
            EnableNavigation(false);

            m_NextContentIndex = m_CurrentContentIndex;
            do
            {
                m_NextContentIndex--;
                if (m_NextContentIndex < 0)
                    m_NextContentIndex = m_AppsContent.Count - 1;

            } while (!m_AppsContent[m_NextContentIndex].IsAvailable);

            MoveToNextContent();
        }

        protected virtual void NextAppContent(InputAction.CallbackContext obj)
        {
            NextAppContent();
        }

        protected virtual void NextAppContent()
        {
            EnableNavigation(false);

            m_NextContentIndex = m_CurrentContentIndex;
            do
            {
                m_NextContentIndex++;
                if (m_NextContentIndex > m_AppsContent.Count - 1)
                    m_NextContentIndex = 0;

            } while (!m_AppsContent[m_NextContentIndex].IsAvailable);

            MoveToNextContent();
        }

        protected void MoveToNextContent()
        {
            m_AppsContent[m_CurrentContentIndex].Content.OnDisableContent += OnCurrentContentDisable;
            m_AppsContent[m_NextContentIndex].Content.OnGainControl += OnNextContentEnable;

            m_AppsContent[m_CurrentContentIndex].Close(true);
        }

        private void OnNextContentEnable()
        {
            m_AppsContent[m_NextContentIndex].Content.OnGainControl -= OnNextContentEnable;

            EnableNavigation(true);

            m_CurrentContentIndex = m_NextContentIndex;
        }

        private void OnCurrentContentDisable()
        {
            m_AppsContent[m_CurrentContentIndex].Content.OnDisableContent -= OnCurrentContentDisable;

            m_AppsContent[m_NextContentIndex].Open(m_AppsContent[m_CurrentContentIndex].PreviousBehaviour);
        }

        protected virtual void OnEnable()
        {
            EnableNavigation(true);
        }

        protected virtual void OnDisable()
        {
            EnableNavigation(false);
        }

        public virtual void EnableNavigation(bool enable)
        {
            if (PlayerInput.all.Count > 0)
            {
                PlayerInput player = PlayerInput.all[0];

                if (enable)
                {
                    player.actions.FindAction(m_Next.action.id).Enable();
                    player.actions.FindAction(m_Next.action.id).performed += NextAppContent;

                    player.actions.FindAction(m_Previous.action.id).Enable();
                    player.actions.FindAction(m_Previous.action.id).performed += PreviousAppContent;
                }
                else
                {
                    player.actions.FindAction(m_Next.action.id).Disable();
                    player.actions.FindAction(m_Next.action.id).performed -= NextAppContent;

                    player.actions.FindAction(m_Previous.action.id).Disable();
                    player.actions.FindAction(m_Previous.action.id).performed -= PreviousAppContent;
                }
            }
            else
            {
                if (enable)
                {
                    m_Next.action.Enable();
                    m_Next.action.performed += NextAppContent;

                    m_Previous.action.Enable();
                    m_Previous.action.performed += PreviousAppContent;
                }
                else
                {
                    m_Next.action.Disable();
                    m_Next.action.performed -= NextAppContent;

                    m_Previous.action.Disable();
                    m_Previous.action.performed -= PreviousAppContent;
                }
            }
        }
    }
}
