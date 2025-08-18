using KBCore.Refs;
using UnityEngine;
using UnityEngine.Events;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public class Magnetable : MonoBehaviour
    {
        [SerializeField] protected Transform m_BaseObject;
        [SerializeField, Tooltip("Time it should take to reach the magnet")]
        protected float m_MagnetizeTime = 0.5f;

        [SerializeField] protected UnityEvent m_OnMagnetStart;
        [SerializeField] protected UnityEvent m_OnMagnetComplete;

        protected Magnet m_Magnet;

        protected float m_MagnetizedTime;
        protected Vector3 m_Velocity;

        public bool IsMagnetized { get; private set; }

        protected virtual void Awake()
        {
            if (m_BaseObject == null)
                m_BaseObject = transform;
        }

        protected virtual void OnDisable()
        {
            IsMagnetized = false;
        }

        public void Magnetize(Magnet magnet)
        {
            IsMagnetized = true;
            m_Magnet = magnet;
            m_MagnetizedTime = 0f;

            m_OnMagnetStart?.Invoke();
        }

        protected virtual void Update()
        {
            if (!IsMagnetized)
                return;

            float distance = Vector3.Distance(m_Magnet.Target.position, m_BaseObject.position);

            if (distance > m_Magnet.Distance)
            {
                var newPosition = Vector3.SmoothDamp(m_BaseObject.position, m_Magnet.Target.position, ref m_Velocity,
                    Mathf.Max(0f, m_MagnetizeTime - m_MagnetizedTime));

                m_BaseObject.position = newPosition;

                m_MagnetizedTime += Time.deltaTime;
            }
            else
            {
                IsMagnetized = false;
                m_OnMagnetComplete?.Invoke();
            }
        }
    }
}