using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.EnGUI
{
    public abstract class EnGUICarrouselContent : MonoBehaviour
    {
        [SerializeField] protected bool m_IsAvailable = true;
        [SerializeField, Self] private EnGUIContent m_Content;

        public EnGUIContent Content => m_Content;

        public EnGUIManager.PreviousBehaviour PreviousBehaviour { get; set; }

        void OnValidate()
        {
            this.ValidateRefs();
        }

        public bool IsAvailable
        {
            get { return m_IsAvailable; }
            set { m_IsAvailable = value; }
        }

        public virtual void Open(EnGUIManager.PreviousBehaviour behaviour)
        {
            PreviousBehaviour = behaviour;

            EnGUIManager.Instance.PushContent(Content, behaviour, null);
        }

        public virtual void Close(bool instant)
        {
            EnGUIManager.Instance.RemoveLastContent(instant);
        }

        public abstract int OrderValue { get; }
    }
}
