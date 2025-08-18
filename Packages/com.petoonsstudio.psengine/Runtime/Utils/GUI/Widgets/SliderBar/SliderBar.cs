using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

namespace PetoonsStudio.PSEngine.Utils
{
    [AddComponentMenu("Petoons Studio/PSEngine/Utils/GUI/Slider Bar")]
    [RequireComponent(typeof(Slider))]
    public class SliderBar : MonoBehaviour
    {
        [SerializeField, Self] protected Slider m_Slider;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        /// <summary>
        /// Updates bar
        /// </summary>
        /// <param name="currentValue"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        public virtual void UpdateBar(float currentValue, float minValue, float maxValue)
        {
            m_Slider.minValue = minValue;
            m_Slider.maxValue = maxValue;
            m_Slider.value = currentValue;
        }
    }
}

