using UnityEngine;
using UnityEngine.EventSystems;

namespace PetoonsStudio.PSEngine.Utils
{
    public class SmartScrollRectItem : MonoBehaviour, ISelectHandler
    {
        public delegate void SmartScrollEvent(SmartScrollRectItem item);
        public event SmartScrollEvent OnSelectItem;

        public void OnSelect(BaseEventData eventData)
        {
            OnSelectItem.Invoke(this);
        }
    }
}
