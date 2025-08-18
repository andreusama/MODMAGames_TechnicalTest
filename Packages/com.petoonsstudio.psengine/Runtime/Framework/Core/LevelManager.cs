using PetoonsStudio.PSEngine.Utils;

namespace PetoonsStudio.PSEngine.Framework
{
    public class LevelManager<T> : Singleton<LevelManager<T>> where T : LevelManager<T>
    {
        protected new static T _instance;

        public new static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                }
                return _instance;
            }
        }
    }
}