using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public class CameraBillboard : MonoBehaviour
    {
        private Camera m_CachedCamera;
        private Camera Camera => m_CachedCamera == null ? (m_CachedCamera = Camera.main) : m_CachedCamera;

        void Awake()
        {
            m_CachedCamera = Camera.main;
        }

        private void LateUpdate()
        {
            if (Camera == null) return;
            transform.forward = m_CachedCamera.transform.forward;
        }
    }
}