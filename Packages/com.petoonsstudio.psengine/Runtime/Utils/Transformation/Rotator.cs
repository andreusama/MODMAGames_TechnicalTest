using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private Transform m_BaseObject;
        [SerializeField] public Vector3 m_Rotation = Vector3.one;
        [SerializeField] public float m_Velocity = 1f;
        [SerializeField] public bool m_IgnoreTimeScale = false;

        private void Awake()
        {
            if(m_BaseObject == null)
                m_BaseObject = transform;
        }

        void Update()
        {
            m_BaseObject.Rotate(m_Rotation * m_Velocity * (m_IgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime));
        }
    }
}

