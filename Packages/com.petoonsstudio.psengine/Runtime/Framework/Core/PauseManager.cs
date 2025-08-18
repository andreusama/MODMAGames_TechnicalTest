using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using PetoonsStudio.PSEngine.EnGUI;
using PetoonsStudio.PSEngine.Input;
using PetoonsStudio.PSEngine.Timeline;
using PetoonsStudio.PSEngine.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace PetoonsStudio.PSEngine.Framework
{
    public struct PSPauseEvent
    {
        public enum Type { PAUSED, RESUMED }

        public Type EventName;

        public PSPauseEvent(Type newName)
        {
            EventName = newName;
        }
    }

    public class PauseManager : Singleton<PauseManager>
    {
        [SerializeField, Tooltip("Should affect all Players or the Instigator")]
        protected bool m_IsGlobal;
        [SerializeField] protected InputActionReference m_Action;
        [SerializeField] protected InputActionReference m_BackAction;
        [SerializeField] protected AssetReferenceGameObject m_Panel;
        [SerializeField] protected bool m_CloseImmediately = false;
        [SerializeField] protected MMSoundManager.MMSoundManagerTracks[] m_TracksToPause;
        [Header("Feedbacks")]
        [SerializeField] protected MMF_Player m_PauseFB;
        [SerializeField] protected MMF_Player m_UnpauseFB;

        protected bool m_IsInstantiating = false;
        private GameObject m_PanelGameObject;

        protected EnGUIPanel m_CurrentPanel;
        protected InputDevice m_OwnerDevice;
        protected PlayerInput m_OwnerInput;
        protected InputSystemUIInputModule m_InputModule;

        public virtual bool IsAvailable
        {
            get
            {
                if (SceneLoaderManager.InstanceExists && SceneLoaderManager.Instance.IsLoading)
                    return false;

                if (CutsceneController.InstanceExists && CutsceneController.Instance.CutsceneRunning)
                    return false;

                if (m_PanelGameObject == null)
                    return false;

                return true;
            }
        }

        /// Accessors
        public bool GamePaused { get; protected set; }

        public InputDevice OwnerDevice => m_OwnerDevice;

        protected virtual void InstantiatePanel()
        {
            if (m_CurrentPanel != null || m_IsInstantiating)
                return;

            m_IsInstantiating = true;

            m_CurrentPanel = EnGUIPanel.CreatePanel(m_PanelGameObject, EnGUIManager.Instance.transform);
            m_IsInstantiating = false;

            m_CurrentPanel.OnPanelDestroy += OnDestroyPanel;
        }

        protected virtual void DestroyPanel()
        {
            if (m_CurrentPanel == null || m_IsInstantiating)
                return;

            m_CurrentPanel.ClosePanel(m_CloseImmediately);
        }

        protected virtual void OnEnable()
        {
            m_Action.action.performed += ActionPerformed;
        }

        protected virtual void OnDisable()
        {
            m_Action.action.performed -= ActionPerformed;
        }

        protected virtual void Start()
        {
            StartCoroutine(PreloadPanel());
        }

        private IEnumerator PreloadPanel()
        {
            var asyncOp = Addressables.LoadAssetAsync<GameObject>(m_Panel);

            yield return asyncOp;

            m_PanelGameObject = asyncOp.Result;
        }

        private void GiveControl()
        {
            if (InputManager.InstanceExists)
            {
                foreach (var input in PlayerInput.all)
                {
                    if (input == m_OwnerInput)
                        InputManager.Instance.InputMapsController.GoUIState(input.playerIndex, addToStack: true);
                    else
                        input.DeactivateInput();
                }
            }
        }

        private void RestoreControl()
        {
            if (InputManager.InstanceExists)
            {
                foreach (var input in PlayerInput.all)
                {
                    if (input == m_OwnerInput)
                        InputManager.Instance.InputMapsController.RestorePreviousState(input.playerIndex);
                    else
                        input.DeactivateInput();
                }
            }
        }

        /// <summary>
        /// Action performed callback
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void ActionPerformed(InputAction.CallbackContext obj)
        {
            if (!obj.performed)
                return;

            if (GamePaused)
            {
                if (!CheckCancel(obj))
                    EndPause(obj.control.device);
            }
            else
            {
                if (IsAvailable)
                    StartPause(obj.control.device);
            }
        }

        protected bool CheckCancel(InputAction.CallbackContext obj)
        {
            string pressedKey = InputControlPath.ToHumanReadableString(obj.control.path, InputControlPath.HumanReadableStringOptions.OmitDevice).ToLower();

            InputAction cancelAction = m_BackAction.action;

            foreach (var binding in cancelAction.bindings)
            {
                string cancelKey = InputControlPath.ToHumanReadableString(binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice).ToLower();

                if (cancelKey == pressedKey)
                    return true;
            }

            return false;
        }

        public virtual void StartPause(InputDevice device)
        {
            GamePaused = true;
            m_OwnerDevice = device;
            m_OwnerInput = InputManager.GetInputWithDevice(device);

            GiveControl();
            InstantiatePanel();
            PSEventManager.TriggerEvent(new PSPauseEvent(PSPauseEvent.Type.PAUSED));

            m_PauseFB?.PlayFeedbacks();

            SetTrackStates(true);
        }

        public virtual void EndPause(InputDevice device = null)
        {
            DestroyPanel();
        }

        protected virtual void OnDestroyPanel()
        {
            m_CurrentPanel.OnPanelDestroy -= OnDestroyPanel;

            RestoreControl();
            PSEventManager.TriggerEvent(new PSPauseEvent(PSPauseEvent.Type.RESUMED));
            m_UnpauseFB?.PlayFeedbacks();
            SetTrackStates(false);

            m_CurrentPanel = null;
            GamePaused = false;
            m_OwnerDevice = null;
            m_OwnerInput = null;
        }

        protected virtual void SetTrackStates(bool pause)
        {
            foreach (var track in m_TracksToPause)
            {
                if (pause)
                {
                    MMSoundManager.Instance.CacheTrackState(track);
                    MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.PauseTrack, track);
                }
                else
                {
                    MMSoundManager.Instance.RestorePreviousTrackState(track);
                }
            }
        }
    }
}