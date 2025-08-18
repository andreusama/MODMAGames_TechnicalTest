using KBCore.Refs;
using Unity.VectorGraphics;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Input
{
    [RequireComponent(typeof(SVGImage))]
    public class InputIconSVGImage : InputIconRenderer
    {
        [SerializeField, Self] protected SVGImage m_Renderer;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        protected override void UpdateSprite(Sprite icon)
        {
            //m_Renderer.color = new Color(m_Renderer.color.r, m_Renderer.color.g, m_Renderer.color.b, 1f);
            m_Renderer.sprite = icon;
        }

        protected override bool IsVisible()
        {
            return m_Renderer.isActiveAndEnabled;
        }
    }
}