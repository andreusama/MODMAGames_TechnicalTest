using NodeCanvas.BehaviourTrees;
using NodeCanvas.StateMachines;
using PetoonsStudio.PSEngine.Utils;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using UnityEngine.Rendering;

namespace PetoonsStudio.PSEngine.Timeline
{
    [Serializable]
    public class StopAnimatorPlayableBehaviour : PlayableBehaviour
    {
        public bool ShouldRestoreState = true;
        public string RestoreState = "Idle";

        private Animator m_BoundGameObject;

        private bool m_IsBound;

        public static ScriptPlayable<StopAnimatorPlayableBehaviour> Create(PlayableGraph graph, int inputCount)
        {

            return ScriptPlayable<StopAnimatorPlayableBehaviour>.Create(graph, inputCount);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (m_BoundGameObject == null)
                m_BoundGameObject = playerData as Animator;

            int inputCount = playable.GetInputCount();
            bool hasInput = false;
            for (int i = 0; i < inputCount; i++)
            {
                if (playable.GetInputWeight(i) > 0)
                {
                    hasInput = true;
                    break;
                }
            }

            if (hasInput)
            {
                if (!m_IsBound)
                {
                    SetBoundGameObject();
                    m_IsBound = true;
                }
            }
            else
            {
                if (m_IsBound)
                {
                    RestoreBoundGameObject();
                    m_IsBound = false;
                }
            }
        }

        private void RestoreBoundGameObject()
        {
            if (m_BoundGameObject == null)
                return;

            m_BoundGameObject.speed = 1f;
        }

        private void SetBoundGameObject()
        {
            if (m_BoundGameObject == null)
                return;

            m_BoundGameObject.speed = 0f;

            if (ShouldRestoreState)
                m_BoundGameObject.Play(RestoreState);
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            RestoreBoundGameObject();
        }
    }
}

