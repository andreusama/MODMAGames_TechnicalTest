using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using PetoonsStudio.PSEngine.EnGUI;
using PetoonsStudio.PSEngine.Input;
using PetoonsStudio.PSEngine.Timeline;
using PetoonsStudio.PSEngine.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace PetoonsStudio.PSEngine.Framework
{
    public struct MMDebugEvent
    {
        public enum Type { OPENED, CLOSED }

        public Type EventName;

        public MMDebugEvent(Type newName)
        {
            EventName = newName;
        }
    }

    public struct DebugOption
    {
        public string Description;
        public Action Action;

        public DebugOption(string description, Action action)
        {
            Description = description;
            Action = action;
        }
    }

    public class DebugManager : PersistentSingleton<DebugManager>
    {
#if PETOONS_DEBUG || UNITY_EDITOR
        [SerializeField, Tooltip("Should affect all Players or the Instigator")]
        protected bool m_IsGlobal;
        [SerializeField] protected InputActionReference m_Action;
        [SerializeField] protected AssetReferenceGameObject m_Panel;
        [SerializeField] protected bool m_CloseImmediately = false;
        [SerializeField] protected MMSoundManager.MMSoundManagerTracks[] m_TracksToPause;

        [Header("Feedbacks")]
        [SerializeField] protected MMF_Player m_PauseFB;
        [SerializeField] protected MMF_Player m_UnpauseFB;

        protected Dictionary<int, List<DebugOption>> m_AvailableOptions = new();
        protected bool m_IsInstantiating = false;
        private GameObject m_PanelGameObject;

        protected GUIDebugPanel m_CurrentPanel;
        protected InputDevice m_OwnerDevice;
        protected PlayerInput m_OwnerInput;

        public virtual bool IsAvailable
        {
            get
            {
                if (EventSystem.current == null)
                    return false;

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

        public virtual void AddOption(UnityEngine.Object owner, DebugOption option)
        {
            int id = owner.GetInstanceID();
            if (m_AvailableOptions.ContainsKey(id))
                m_AvailableOptions[id].Add(option);
            else
                m_AvailableOptions.Add(id, new List<DebugOption>() { option });
        }

        protected virtual void DestroyPanel()
        {
            if (m_CurrentPanel == null || m_IsInstantiating)
                return;

            m_CurrentPanel.ClosePanel(m_CloseImmediately);
        }

        protected virtual void CreateOptions()
        {
            m_AvailableOptions.Clear();

            AddOption(this, new DebugOption("Profile", ProfileMenu));
        }

        private void ProfileMenu()
        {
            m_CurrentPanel.OpenProfileMenu();
        }

        protected virtual void InstantiatePanel()
        {
            if (m_CurrentPanel != null || m_IsInstantiating)
                return;

            CreateOptions();

            m_IsInstantiating = true;
            m_CurrentPanel = EnGUIPanel.CreatePanel(m_PanelGameObject, EnGUIManager.Instance.transform) as GUIDebugPanel;

            List<DebugOption> totalOptions = new();

            foreach (var options in m_AvailableOptions.Values)
            {
                totalOptions.AddRange(options);
            }

            m_CurrentPanel.BaseContent.SetOptions(totalOptions);
            m_IsInstantiating = false;

            m_CurrentPanel.OnPanelDestroy += OnDestroyPanel;
        }

        protected virtual void OnEnable()
        {

            m_Action.action.Enable();
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

        /// <summary>
        /// Action performed callback
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void ActionPerformed(InputAction.CallbackContext obj)
        {
            if (!obj.performed)
                return;

            if (GamePaused)
                EndPause(obj.control.device);
            else
            {
                if (IsAvailable)
                    StartPause(obj.control.device);
            }
        }

        public virtual void StartPause(InputDevice device)
        {
            GamePaused = true;
            m_OwnerDevice = device;
            m_OwnerInput = InputManager.GetInputWithDevice(device);

            GiveControl();
            InstantiatePanel();

            PSEventManager.TriggerEvent(new MMDebugEvent(MMDebugEvent.Type.OPENED));

            m_PauseFB?.PlayFeedbacks();

            SetTrackStates(true);
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

        public virtual void EndPause(InputDevice device = null)
        {
            DestroyPanel();
        }

        protected virtual void OnDestroyPanel()
        {
            m_CurrentPanel.OnPanelDestroy -= OnDestroyPanel;
            
            RestoreControl();
            PSEventManager.TriggerEvent(new MMDebugEvent(MMDebugEvent.Type.CLOSED));
            m_UnpauseFB?.PlayFeedbacks();
            SetTrackStates(false);

            GamePaused = false;
            m_OwnerDevice = null;
            m_OwnerInput = null;
            m_CurrentPanel = null;
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
#endif
    }
}