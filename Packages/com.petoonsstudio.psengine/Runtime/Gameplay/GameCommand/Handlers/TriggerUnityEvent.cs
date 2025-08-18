using System;
using UnityEngine;
using UnityEngine.Events;

namespace PetoonsStudio.PSEngine.Gameplay
{

    public class TriggerUnityEvent : GameCommandReceiver
    {
        [SerializeField] private UnityEvent m_UnityEvent;

        public override void Execute()
        {
            m_UnityEvent.Invoke();

            EndAction();
        }
    }
}
