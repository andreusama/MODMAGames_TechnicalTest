using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public class Bouncer : MonoBehaviour
    {
        [SerializeField] private Transform m_BaseObject;
        [SerializeField] private float m_Speed = 2f;
        [SerializeField] private float m_Amplitude = 0.1f;
        [SerializeField] private bool m_IgnoreTimeScale;

        private float m_CurrentTime;
        private float m_BaseYLocalPosition;

        public float Speed { get { return m_Speed; } set { m_Speed = value; } }
        public float Amplitude { get { return m_Amplitude; } set { m_Amplitude = value; } }
        public bool IgnoreTimeScale { get { return m_IgnoreTimeScale; } set { m_IgnoreTimeScale = value; } }

        void Awake()
        {
            if (m_BaseObject == null)
                m_BaseObject = transform;

            m_BaseYLocalPosition = m_BaseObject.localPosition.y;
        }

        void Update()
        {
            m_BaseObject.localPosition = new Vector3(m_BaseObject.localPosition.x, m_BaseYLocalPosition + (Mathf.Sin(m_CurrentTime * m_Speed) * m_Amplitude), m_BaseObject.localPosition.z);

            if (m_IgnoreTimeScale)
                m_CurrentTime += Time.unscaledDeltaTime;
            else
                m_CurrentTime += Time.deltaTime;
        }
    }
}
