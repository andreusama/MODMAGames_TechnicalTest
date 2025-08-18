using KBCore.Refs;
using PetoonsStudio.PSEngine.Utils;
using TMPro;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Input
{
    [RequireComponent(typeof(TMP_Text))]
    public class TMPSpriteAssetChange : MonoBehaviour, PSEventListener<NewProviderEvent>
    {
        [SerializeField, Self] private TMP_Text m_Text;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        void Start()
        {
            RefreshSpriteAsset();
        }

        void OnEnable()
        {
            this.PSEventStartListening();
        }

        void OnDisable()
        {
            this.PSEventStopListening();
        }

        public void OnPSEvent(NewProviderEvent eventType)
        {
            RefreshSpriteAsset();
        }

        private void RefreshSpriteAsset()
        {
            m_Text.spriteAsset = IconServiceProvider.Instance.GetTextSpriteAsset();
        }
    }
}

