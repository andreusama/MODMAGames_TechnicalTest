using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public class SelectionUIElementWithHiddenOptions<T> : SelectionUIElement<T>
    {
        [SerializeField] protected List<T> m_HiddenOptions;

        public override void NextOption()
        {
            int timesChanged = 0;
            do
            {
                m_CurrentSelection++;
                if (m_CurrentSelection >= m_SelectionOptions.Count)
                {
                    m_CurrentSelection = 0;
                }

                timesChanged++;
                if (timesChanged >= m_SelectionOptions.Count)
                {
                    Debug.LogError("All options are hidden!");
                    break;
                }
            }
            while (m_HiddenOptions.Contains(m_SelectionOptions[m_CurrentSelection].Value));


            UpdateLabel();
            ExecuteSelection();
        }

        public override void PreviousOption()
        {
            int timesChanged = 0;
            do
            {
                m_CurrentSelection--;
                if (m_CurrentSelection < 0)
                {
                    m_CurrentSelection = m_SelectionOptions.Count - 1;
                }

                timesChanged++;
                if (timesChanged >= m_SelectionOptions.Count)
                {
                    Debug.LogError("All options are hidden!");
                    break;
                }
            }
            while (m_HiddenOptions.Contains(m_SelectionOptions[m_CurrentSelection].Value));

            UpdateLabel();
            ExecuteSelection();
        }
    }
}
