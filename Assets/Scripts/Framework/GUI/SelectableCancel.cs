using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EnGUI
{
    public class SelectableCancel : MonoBehaviour, ICancelHandler
    {
        public UnityEvent OnCancelEvent;

        public EnGUIContent Content { get; set; }

        public void OnCancel(BaseEventData eventData)
        {
            Content.OnCancel();
            OnCancelEvent?.Invoke();
        }
    }
}