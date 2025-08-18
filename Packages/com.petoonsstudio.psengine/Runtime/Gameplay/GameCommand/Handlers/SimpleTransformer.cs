using System;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public abstract class SimpleTransformer : GameCommandReceiver
    {
        public enum LoopType
        {
            Once,
            PingPong,
            Repeat
        }

        [SerializeField] protected LoopType m_LoopType;

        [SerializeField] protected float m_Duration = 1;
        [SerializeField] protected AnimationCurve m_AccelCurve;

        [SerializeField] protected bool m_Activate = false;
        [SerializeField] protected GameCommandSender m_OnStartCommand, m_OnStopCommand;

        [SerializeField] protected AudioSource m_OnStartAudio, m_OnEndAudio;

        [Range(0, 1)]
        [SerializeField] protected float m_PreviewPosition;

        private float m_Time = 0f;
        private float m_Position = 0f;
        private float m_Direction = 1f;

        [ContextMenu("Test Start Audio")]
        void TestPlayAudio()
        {
            if (m_OnStartAudio != null) m_OnStartAudio.Play();
        }

        public override void Execute()
        {
            m_Activate = true;
            if (m_OnStartCommand != null) m_OnStartCommand.Send(transform);
            if (m_OnStartAudio != null) m_OnStartAudio.Play();

            EndAction();
        }

        public void FixedUpdate()
        {
            if (m_Activate)
            {
                m_Time = m_Time + (m_Direction * Time.deltaTime / m_Duration);
                switch (m_LoopType)
                {
                    case LoopType.Once:
                        LoopOnce();
                        break;
                    case LoopType.PingPong:
                        LoopPingPong();
                        break;
                    case LoopType.Repeat:
                        LoopRepeat();
                        break;
                }
                
                PerformTransform(m_Position);
            }
        }

        public virtual void PerformTransform(float position)
        {

        }

        void LoopPingPong()
        {
            m_Position = Mathf.PingPong(m_Time, 1f);
        }

        void LoopRepeat()
        {
            m_Position = Mathf.Repeat(m_Time, 1f);
        }

        void LoopOnce()
        {
            m_Position = Mathf.Clamp01(m_Time);
            if (m_Position >= 1)
            {
                enabled = false;
                if (m_OnStopCommand != null) m_OnStopCommand.Send(transform);
                m_Direction *= -1;
            }
        }
    }
}
