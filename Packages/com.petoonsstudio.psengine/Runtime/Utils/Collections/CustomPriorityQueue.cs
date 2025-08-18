using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    class CustomPriorityQueue<Value>
    {
        class ItemWithPriority : IComparable<ItemWithPriority>
        {
            public ItemWithPriority(Value t, double priority)
            {
                Item = t;
                Priority = priority;
            }

            public Value Item { get; private set; }
            public double Priority { get; private set; }

            public int CompareTo(ItemWithPriority other)
            {
                return Priority.CompareTo(other.Priority);
            }
        }

        PriorityQueue<ItemWithPriority> q = new PriorityQueue<ItemWithPriority>();

        public int Count => q.items.Count;

        public void Enqueue(Value item, double priority)
        {
            q.Push(new ItemWithPriority(item, priority));
        }

        public Value Dequeue()
        {
            return q.Pop().Item;
        }
    }
}
