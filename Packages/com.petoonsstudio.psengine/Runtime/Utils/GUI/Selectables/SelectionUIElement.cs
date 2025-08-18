using PetoonsStudio.PSEngine.EnGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace PetoonsStudio.PSEngine.Utils
{
    public class SelectionUIElement<T> : Selectable, IValueChanger
    {
        [Serializable]
        public struct Option
        {
            public T Value;
            public UnityEvent OnSelect;

            public Option(T value, UnityAction action)
            {
                this.Value = value;
                this.OnSelect = new UnityEvent();
                this.OnSelect.AddListener(action);
            }
        }

        [SerializeField] protected TextMeshProUGUI m_TextSelection;
        [SerializeField] protected bool m_LocalizeSelectionString;
        [SerializeField] protected LocalizedStringTable m_StringTable;
        [SerializeField] protected UnityEvent m_OnValueChange;

        protected List<Option> m_SelectionOptions;
        protected int m_CurrentSelection;
        protected UnityEvent<string> m_OnValueChangeAction = new UnityEvent<string>();

        public List<Option> Options
        {
            get
            {
                return m_SelectionOptions;
            }
        }

        public T Value
        {
            get
            {
                return m_SelectionOptions[m_CurrentSelection].Value;
            }
        }

        public UnityEvent<string> OnValueChange
        {
            get
            {
                return m_OnValueChangeAction;
            }
            set
            {
                m_OnValueChangeAction = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            m_SelectionOptions = new List<Option>();
        }

        public void Setup(List<T> options, UnityAction action)
        {
            m_SelectionOptions.Clear();

            foreach (var element in options)
            {
                m_SelectionOptions.Add(new Option(element, action));
            }
        }

        public override void OnMove(AxisEventData eventData)
        {
            base.OnMove(eventData);

            switch (eventData.moveDir)
            {
                case MoveDirection.Left:
                    PreviousOption();
                    break;
                case MoveDirection.Right:
                    NextOption();
                    break;
            }
        }

        public virtual void NextOption()
        {
            m_CurrentSelection++;
            if (m_CurrentSelection >= m_SelectionOptions.Count)
            {
                m_CurrentSelection = 0;
            }

            OnValueChange?.Invoke(OptionToString());
            m_OnValueChange?.Invoke();

            ExecuteSelection();
            UpdateLabel();
        }

        public virtual void PreviousOption()
        {
            m_CurrentSelection--;
            if (m_CurrentSelection < 0)
            {
                m_CurrentSelection = m_SelectionOptions.Count - 1;
            }

            OnValueChange?.Invoke(OptionToString());
            m_OnValueChange?.Invoke();

            ExecuteSelection();
            UpdateLabel();
        }

        public void UpdateSelection(T value)
        {
            for (int i = 0; i < m_SelectionOptions.Count; i++)
            {
                if (m_SelectionOptions[i].Value.Equals(value))
                {
                    m_CurrentSelection = i;
                    break;
                }
            }

            UpdateLabel();
        }

        protected virtual void UpdateLabel()
        {
            if (m_LocalizeSelectionString)
            {
                LocalizationUtils.SetLocalizedGUIText(m_TextSelection, m_StringTable.GetTable().TableCollectionName, m_SelectionOptions[m_CurrentSelection].Value.ToString());
            }
            else
            {
                m_TextSelection.text = OptionToString();
            }
        }

        protected void ExecuteSelection()
        {
            m_SelectionOptions[m_CurrentSelection].OnSelect.Invoke();
        }

        protected virtual string OptionToString()
        {
            return m_SelectionOptions[m_CurrentSelection].Value.ToString();
        }
    }
}
