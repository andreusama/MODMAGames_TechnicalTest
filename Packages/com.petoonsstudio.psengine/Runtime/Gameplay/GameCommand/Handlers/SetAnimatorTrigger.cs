using KBCore.Refs;
using System;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public class SetAnimatorTrigger : GameCommandReceiver
    {
        [SerializeField, Self] private Animator m_Animator;
        [SerializeField] private string m_TriggerName;

        public override void Execute()
        {
            if (m_Animator) m_Animator.SetTrigger(m_TriggerName);

            EndAction();
        }
    }
}
