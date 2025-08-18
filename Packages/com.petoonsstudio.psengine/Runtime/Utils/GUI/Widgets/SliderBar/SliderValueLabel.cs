using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

namespace PetoonsStudio.PSEngine.Utils
{
    [AddComponentMenu("Petoons Studio/PSEngine/Utils/GUI/Slider Value Label")]
    [RequireComponent(typeof(Slider))]
    public class SliderValueLabel : MonoBehaviour
    {
        public Text Label;

        [SerializeField, Self] private Slider m_Slider;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public void UpdateLabel()
        {
            Label.text = m_Slider.value.ToString();
        }
    }
}