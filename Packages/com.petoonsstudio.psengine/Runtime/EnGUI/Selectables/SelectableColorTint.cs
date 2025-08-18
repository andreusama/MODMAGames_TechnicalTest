using KBCore.Refs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PetoonsStudio.PSEngine.EnGUI
{
    public class SelectableColorTint : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private Graphic m_Graphic;

        [SerializeField, Self] private Selectable m_Selectable;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            m_Graphic.color = m_Selectable.colors.normalColor;
        }

        public void OnSelect(BaseEventData eventData)
        {
            m_Graphic.color = m_Selectable.colors.selectedColor;
        }
    }
}