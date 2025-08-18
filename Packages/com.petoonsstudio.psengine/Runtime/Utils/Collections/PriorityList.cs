using System;
using System.Collections;
using System.Collections.Generic;

namespace PetoonsStudio.PSEngine.Utils
{
    /// <summary>
    /// A priority list.
    /// </summary>
    public class PriorityList<T> : IEnumerable where T : IComparable<T>
    {
        /// <summary>
        /// The internal list used by the queue. Use with care.
        /// </summary>
        public readonly List<T> items;

        public List<T> Items => items;
        public int Count => items.Count;

        public bool Contains (T item)
        {
            return items.Contains(item);
        }

        public PriorityList()
        {
            items = new List<T>();
        }

        public bool Empty
        {
            get { return items.Count == 0; }
        }

        public T First
        {
            get
            {
                if (items.Count >= 1)
                {
                    return items[0];
                }
                return default;
            }
        }

        public void Add(T item)
        {
            lock (this)
            {
                items.Add(item);
                SiftDown(0, items.Count - 1);
            }
        }

        public T RemoveFirst()
        {
            lock (this)
            {
                T item;
                var last = items[items.Count - 1];
                items.RemoveAt(items.Count - 1);
                if (items.Count > 0)
                {
                    item = items[0];
                    items[0] = last;
                    SiftUp(0);
                }
                else
                {
                    item = last;
                }
                return item;
            }
        }

        public void Remove(T element)
        {
            lock (this)
            {
                if (!items.Contains(element))
                    return;

                var index = items.IndexOf(element);
                items.RemoveAt(index);
                if (index >= items.Count)
                    index--;

                if (items.Count > 0)
                {
                    SiftUp(index);
                }
            }
        }

        int Compare(T A, T B)
        {
            return A.CompareTo(B);
        }

        void SiftDown(int startpos, int pos)
        {
            var newitem = items[pos];
            while (pos > startpos)
            {
                var parentpos = (pos - 1) >> 1;
                var parent = items[parentpos];
                if (Compare(parent, newitem) <= 0)
                    break;
                items[pos] = parent;
                pos = parentpos;
            }
            items[pos] = newitem;
        }

        void SiftUp(int pos)
        {
            var endpos = items.Count;
            var startpos = pos;
            var newitem = items[pos];
            var childpos = 2 * pos + 1;
            while (childpos < endpos)
            {
                var rightpos = childpos + 1;
                if (rightpos < endpos && Compare(items[rightpos], items[childpos]) <= 0)
                    childpos = rightpos;
                items[pos] = items[childpos];
                pos = childpos;
                childpos = 2 * pos + 1;
            }
            items[pos] = newitem;
            SiftDown(startpos, pos);
        }

        public void Clear()
        {
            items.Clear();
        }

        public IEnumerator GetEnumerator()
        {
            return items.GetEnumerator();
        }
    }
}