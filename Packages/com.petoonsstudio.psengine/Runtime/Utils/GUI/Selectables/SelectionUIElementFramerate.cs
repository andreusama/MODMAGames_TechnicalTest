using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Utils;
using System;

namespace PetoonsStudio.PSEngine.Utils
{
    [Serializable]
    public class SelectionUIElementFramerate : SelectionUIElement<int> 
    {
        protected override void UpdateLabel()
        {
            if (m_LocalizeSelectionString)
            {
                LocalizationUtils.SetLocalizedGUIText(m_TextSelection, m_StringTable.GetTable().TableCollectionName, m_SelectionOptions[m_CurrentSelection].Value.ToString() + "fps");
            }
            else
            {
                if (m_SelectionOptions[m_CurrentSelection].Value == 0)
                    m_TextSelection.text = "Unlimited";
                else
                    m_TextSelection.text = $"{m_SelectionOptions[m_CurrentSelection].Value} fps";
            }
        }
    }
}