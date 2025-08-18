using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Inventory
{
    public class Inventory<T> where T : InventoryItem
    {
        protected List<T> m_Items;

        public List<T> Items => m_Items;

        public Inventory()
        {
            m_Items = new List<T>();
        }

        public void UpdateItemQuantity(string item, int quantity = 1)
        {
            T listedItem = FindItem(item);
            if (listedItem == null)
                return;

            if (listedItem.MaxQuantity == 0)
                return;
            else if (listedItem.MaxQuantity < 0)
                listedItem.StackedItem.Quantity += quantity;
            else
                listedItem.StackedItem.Quantity = Mathf.Clamp(listedItem.StackedItem.Quantity + quantity, 0, listedItem.MaxQuantity);
        }

        public void AddItem(T item)
        {
            m_Items.Add(item);
        }

        public void RemoveItem(string item, int quantity = 1)
        {
            if (HasItem(item))
            {
                T listedItem = FindItem(item);

                listedItem.StackedItem.Quantity -= quantity;

                if (listedItem.StackedItem.Quantity <= 0)
                {
                    m_Items.Remove(listedItem);
                }
            }
        }

        public bool HasItem(string item)
        {
            T listedItem = FindItem(item);

            if (listedItem != null && listedItem.StackedItem.Quantity > 0)
            {
                return true;
            }

            return false;
        }

        public T FindItem(string item)
        {
            foreach (var element in m_Items)
            {
                if (element.StackedItem.ItemID == item)
                    return element;
            }

            return null;
        }

        public bool TryGetItem(string id, out T item)
        {
            foreach (var element in m_Items)
            {
                if (element.StackedItem.ItemID == id)
                {
                    item = element;
                    return true;
                }
            }

            item = null;
            return false;
        }
    }
}