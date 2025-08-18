using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetoonsStudio.PSEngine.Utils
{
    public abstract class TimedCache<T> : IDisposable
    {
        protected List<string> m_Buffer;    //This behaves as a Queue but allows access to all items
        protected Dictionary<string, T> m_Cache;
        protected Dictionary<string, int> m_TicksUnaccessed;

        private List<string> m_ItemsToRemove;
        private bool m_IsCanceled = false;

        protected readonly int TICKS_TO_REMOVE;
        protected readonly int MAX_BUFFER_SIZE;
        protected readonly float CACHE_UPDATE_INTERVAL;

        public TimedCache(int maxBufferSize = 15, int ticksToRemove = 60, float updateInterval = 0.5f)
        {
            m_Buffer = new List<string>();
            m_Cache = new Dictionary<string, T>();
            m_TicksUnaccessed = new Dictionary<string, int>();
            m_ItemsToRemove = new List<string>();

            MAX_BUFFER_SIZE = maxBufferSize;
            TICKS_TO_REMOVE = ticksToRemove;
            CACHE_UPDATE_INTERVAL = updateInterval;

            UpdateCache();
        }

        public virtual T Get(string id)
        {
            if (m_Cache.TryGetValue(id, out T data))
                return data;

            data = LoadAsset(id);

            AddItem(id, data);

            return data;
        }

        protected virtual void AddItem(string id, T item)
        {
            m_Cache.Add(id, item);
            m_Buffer.Add(id);
            m_TicksUnaccessed.Add(id, 0);

            if (m_Buffer.Count >= MAX_BUFFER_SIZE)
            {
                RemoveItem(m_Buffer[0]);
            }
        }

        protected virtual void RemoveItem(string discardedID)
        {
            m_Buffer.Remove(discardedID);
            m_TicksUnaccessed.Remove(discardedID);

            ReleaseAsset(m_Cache[discardedID]);
            m_Cache.Remove(discardedID);
        }

        protected abstract T LoadAsset(string id);

        protected abstract void ReleaseAsset(T asset);

        public virtual void Clear()
        {
            foreach (var item in m_Cache)
            {
                ReleaseAsset(item.Value);
            }

            m_Cache.Clear();
            m_Buffer.Clear();
            m_TicksUnaccessed.Clear();
        }

        public void Dispose()
        {
            m_IsCanceled = true;
            Clear();
        }

        protected void Tick()
        {
            if (m_Buffer.Count == 0)
                return;

            foreach(var item in m_Buffer)
            {
                m_TicksUnaccessed[item]++;

                if (m_TicksUnaccessed[item] >= TICKS_TO_REMOVE)
                    m_ItemsToRemove.Add(item);
            }

            foreach(var item in m_ItemsToRemove)
            {
                RemoveItem(item);
            }

            m_ItemsToRemove.Clear();
        }

        private async void UpdateCache()
        {
            while(!m_IsCanceled)
            {
                Tick();
                await Task.Delay(TimeSpan.FromSeconds(CACHE_UPDATE_INTERVAL));
            }
        }
    }
}