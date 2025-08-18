using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PetoonsStudio.PSEngine.Utils
{
    [RequireComponent(typeof(Selectable))]
    public class CancelHandler : MonoBehaviour, ICancelHandler
    {
        [SerializeField] private UnityEvent m_OnCancel;

        public void OnCancel(BaseEventData eventData)
        {
            m_OnCancel.Invoke();
        }
    }
}