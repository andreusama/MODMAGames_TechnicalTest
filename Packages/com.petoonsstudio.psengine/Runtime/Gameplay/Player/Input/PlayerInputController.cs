using KBCore.Refs;
using PetoonsStudio.PSEngine.Input;
using PetoonsStudio.PSEngine.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;

namespace PetoonsStudio.PSEngine.Gameplay
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputController : MonoBehaviour, PSEventListener<MMRebindEvent>, PSEventListener<MMBindingsResetEvent>
    {
        [SerializeField]
        protected InputFiniteStateMachine m_InputFSM = new();
        [SerializeField, ClassSelector(typeof(InputState), false, false)]
        protected string m_InitialInputState = typeof(GameplayInputState).AssemblyQualifiedName;

        [SerializeField]
        protected Dictionary<Type, InputState> m_States = new();

        private Stack<Type> m_InputStack;
        private InputState m_CurrentInputState;

        [SerializeField, Self] protected PlayerInput m_PlayerInput;

        /// <summary>
        /// Get the current PlayerIndex, by default will use the PlayerInput playerIndex
        /// </summary>
        /// <returns></returns>
        protected virtual int PlayerIndex => m_PlayerInput.playerIndex;

        protected virtual void Awake()
        {
            m_InputStack = new Stack<Type>();
            InitializeStateMachine();
        }

        protected virtual void OnEnable()
        {
            if (m_CurrentInputState == null)
                m_CurrentInputState = m_States[Type.GetType(m_InitialInputState)];

            m_InputFSM.Initialize(m_CurrentInputState);

            this.PSEventStartListening<MMRebindEvent>();
            this.PSEventStartListening<MMBindingsResetEvent>();
        }

        protected virtual void OnDisable()
        {
            this.PSEventStopListening<MMRebindEvent>();
            this.PSEventStopListening<MMBindingsResetEvent>();
        }

        public virtual void Clear()
        {
            m_InputStack.Clear();

            ChangeTo(Type.GetType(m_InitialInputState), false);
        }

        protected virtual void Start()
        {
            SetupPlayerInput();
        }

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        #region MAP CONTROL
        protected virtual void InitializeStateMachine()
        {
            AddStates();
        }

        protected virtual void AddStates()
        {
            UIInputState uIInputState = new(m_PlayerInput, m_InputFSM);
            m_States.Add(typeof(UIInputState), uIInputState);
            CutsceneInputState cutsceneInputState = new(m_PlayerInput, m_InputFSM);
            m_States.Add(typeof(CutsceneInputState), cutsceneInputState);
            GameplayInputState gameplayInputState = new(m_PlayerInput, m_InputFSM);
            m_States.Add(typeof(GameplayInputState), gameplayInputState);
            DialogueInputState dialogueInputState = new(m_PlayerInput, m_InputFSM);
            m_States.Add(typeof(DialogueInputState), dialogueInputState);
        }

        public virtual void RestorePreviousState()
        {
            if (m_InputStack.Count == 0) return;

            var newInput = m_InputStack.Pop();

            ChangeTo(newInput, false);
        }

        public void ChangeTo<T>(bool addToStack = true) where T : InputState
        {
            ChangeTo(typeof(T), addToStack);
        }

        public void ChangeTo(Type type, bool addToStack = true)
        {
            if (!m_States.TryGetValue(type, out InputState newState))
                return;

            var UIInputModule = FindObjectOfType<InputSystemUIInputModule>();

            if (UIInputModule != null)
                UIInputModule.enabled = newState.RequiresInputUIModule;

            if (addToStack)
                m_InputStack.Push(m_InputFSM.CurrentState.GetType());

            m_InputFSM.ChangeState(newState);
            m_CurrentInputState = newState;
        }

        public virtual void AddMapToState(string map)
        {
            (m_InputFSM.CurrentState as InputState).AddNewMap(map);
        }

        public virtual void RemoveMapToState(string map)
        {
            (m_InputFSM.CurrentState as InputState).RemoveMap(map);
        }

        #endregion

        #region REBINDING
        /// <summary>
        /// Used to Update scheme of PlayerInput and pull rebind changes
        /// </summary>
        /// <param name="schemeName"></param>
        protected virtual void SetupPlayerInput(string schemeName = null)
        {
            if (InputManager.InstanceExists)
            {
                if (schemeName == null)
                    InputManager.Instance.UpdateBindingMask(m_PlayerInput.actions);
                else
                    InputManager.Instance.ChangeBindingMask(schemeName, m_PlayerInput.actions);
            }

            PullInputActionAssets();
        }

        /// <summary>
        /// Used to update current Input Action asset when a Rebind event or Binding reset event is recieved
        /// </summary>
        protected void PullInputActionAssets()
        {
            if (InputManager.InstanceExists)
                InputManager.Instance.UpdateInputAssetWithRebindChanges(m_PlayerInput.actions, PlayerIndex);
        }

        public void OnPSEvent(MMRebindEvent eventType)
        {
            if (eventType.PlayerID == PlayerIndex)
                PullInputActionAssets();
        }

        public void OnPSEvent(MMBindingsResetEvent eventType)
        {
            if (eventType.PlayerID == PlayerIndex || eventType.PlayerID == -1)
                m_PlayerInput.actions.RemoveAllBindingOverrides();
        }
        #endregion
    }
}