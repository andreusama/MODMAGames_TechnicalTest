using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private float m_Lifetime;

        private float m_CurrentTime;

        public delegate void FireEvent();
        public event FireEvent Fire;

        void OnEnable()
        {
            ResetLifetime();
        }

        void Update()
        {
            m_CurrentTime += Time.deltaTime;

            if (m_CurrentTime >= m_Lifetime)
            {
                Fire?.Invoke();
                enabled = false;
            }
        }

        public void ResetLifetime()
        {
            m_CurrentTime = 0f;
        }
    }
}