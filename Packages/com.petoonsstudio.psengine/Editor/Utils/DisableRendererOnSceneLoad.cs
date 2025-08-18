using KBCore.Refs;
using System;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public class DisableRendererOnSceneLoad : MonoBehaviour
    {
        [Flags]
        public enum Case
        {
            Build = 1, Editor = 2
        }

        [SerializeField, Self] private Renderer m_Renderer;
        [Tooltip("Disable cases")]
        [SerializeField] private Case m_DisableCase = Case.Build;

        void Awake()
        {
            if (m_Renderer == null)
                return;

            if (m_DisableCase.HasFlag(Case.Build))
            {
                if (!Application.isEditor)
                    m_Renderer.enabled = false;
            }

            if (m_DisableCase.HasFlag(Case.Editor))
            {
                if (Application.isEditor)
                    m_Renderer.enabled = false;
            }
        }

        private void OnValidate()
        {
            this.ValidateRefs();
        }
    }
}

