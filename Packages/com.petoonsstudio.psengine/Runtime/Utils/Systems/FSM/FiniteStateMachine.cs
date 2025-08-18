using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public class FiniteStateMachine<T>
    {
        protected FSMState<T> m_CurrentState;
        protected FSMState<T> m_PreviousState;

        public FSMState<T> CurrentState { get { return m_CurrentState; } }
        public FSMState<T> PreviousState { get { return m_PreviousState; } }

        public delegate void StateChange();
        public event StateChange OnStateChange;

        public virtual void Initialize(FSMState<T> startingState)
        {
            m_CurrentState = startingState;
            startingState.Enter();
        }

        public virtual void Update()
        {
            m_CurrentState.Update();
        }

        public virtual void FixedUpdate()
        {
            m_CurrentState.FixedUpdate();
        }

        public virtual void OnAnimatorMove()
        {
            m_CurrentState.OnAnimatorMove();
        }

        public virtual void ChangeState(FSMState<T> newState)
        {
            if (newState == m_CurrentState)
            {
                return;
            }

            m_CurrentState.Exit();

            m_PreviousState = m_CurrentState;

            m_CurrentState = newState;

            newState.Enter();

            OnStateChange?.Invoke();
        }

        public virtual void RestorePreviousState()
        {
            if (m_PreviousState == null) return;
            ChangeState(m_PreviousState);
        }
    }

    public delegate void FSMStateEnterDelegate();
    public delegate void FSMStateExitDelegate();

    public class FSMState<T>
    {
        public event FSMStateEnterDelegate OnEnter;
        public event FSMStateExitDelegate OnExit;

        protected FiniteStateMachine<T> m_StateMachine;
        protected T m_Context;

        public FSMState(T context, FiniteStateMachine<T> stateMachine)
        {
            m_StateMachine = stateMachine;
            m_Context = context;
        }

        public virtual void Enter()
        {
            OnEnter?.Invoke();
        }

        public virtual void Update()
        {

        }

        public virtual void FixedUpdate()
        {

        }

        public virtual void OnAnimatorMove()
        {

        }

        public virtual void Exit()
        {
            OnExit?.Invoke();
        }
    }
}

