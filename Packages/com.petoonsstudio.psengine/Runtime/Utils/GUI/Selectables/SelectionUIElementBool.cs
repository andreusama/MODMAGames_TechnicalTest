using System;
using UnityEngine.Events;

namespace PetoonsStudio.PSEngine.Utils
{
    [Serializable]
    public class SelectionUIElementBool : SelectionUIElement<bool> 
    {
        private const string m_TrueValue = "On";
        private const string m_FalseValue = "Off";

        public void Setup(UnityAction action)
        {
            m_SelectionOptions.Clear();

            m_SelectionOptions.Add(new Option(true, action));
            m_SelectionOptions.Add(new Option(false, action));
        }

        protected override string OptionToString()
        {
            if (m_SelectionOptions[m_CurrentSelection].Value == true)
                return m_TrueValue;
            else
                return m_FalseValue;
        }
    };
}