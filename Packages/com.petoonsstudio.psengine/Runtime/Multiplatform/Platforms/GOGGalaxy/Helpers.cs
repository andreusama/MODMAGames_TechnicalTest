using System;

namespace PetoonsStudio.PSEngine.Multiplatform.GOG
{
    public static class Listener
    {
#if STANDALONE_GOG
        public static void Create<T>(ref T listener) where T : new()
        {
            if (listener == null)
                listener = new T();
        }

        public static void Dispose<T>(ref T listener) where T : IDisposable
        {
            if (listener != null)
                listener.Dispose();
            listener = default(T);
        }
#endif
    }
}
