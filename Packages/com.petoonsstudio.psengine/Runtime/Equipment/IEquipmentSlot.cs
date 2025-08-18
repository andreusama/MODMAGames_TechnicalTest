using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Equipment
{
    public interface IEquipmentItem 
    {
        void OnItemEquip(Transform t);
        void OnItemUnequip(Transform t);
    }

    [System.Serializable]
    public class EquipmentSlot<T> where T : IEquipmentItem
    {
        public T Current;

        public delegate void SlotChangeDelegate(T item);

        public event SlotChangeDelegate OnSlotEquip;
        public event SlotChangeDelegate OnSlotUnequip;

        public bool IsEmpty => Current == null;

        public void Equip(T newItem, Transform t)
        {
            if (Current != null)
                Unequip(t);

            Current = newItem;
            Current.OnItemEquip(t);
            OnSlotEquip?.Invoke(newItem);
        }

        public void Unequip(Transform t)
        {
            if (Current == null)
                return;

            var item = Current;

            Current.OnItemUnequip(t);
            Current = default;
            OnSlotUnequip?.Invoke(item);
        }
    }
}

