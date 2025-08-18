using PetoonsStudio.PSEngine.Framework;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Multiplatform
{
    [DefaultExecutionOrder(-1)]
    public class PlatformObjectEnabler : MonoBehaviour
    {
        [Header("Enabled on selected platforms")]
        public Platform Platforms;

        /// <summary>
        /// Awake
        /// </summary>
        void Awake()
        {
#if UNITY_SWITCH
            if (!Platforms.HasFlag(Platform.Switch))
                gameObject.SetActive(false);
#elif UNITY_PS4
            if (!Platforms.HasFlag(Platform.Ps4))
                gameObject.SetActive(false);
#elif UNITY_PS5
            if (!Platforms.HasFlag(Platform.Ps5))
                gameObject.SetActive(false);
#elif UNITY_GAMECORE_XBOXONE
            if (!Platforms.HasFlag(Platform.XboxOne))
                gameObject.SetActive(false);
#elif UNITY_GAMECORE_XBOXSERIES
            if (!Platforms.HasFlag(Platform.XboxSeries))
                gameObject.SetActive(false);
#elif STANDALONE_STEAM
            if (!Platforms.HasFlag(Platform.Steam))
                gameObject.SetActive(false);     
#elif STANDALONE_EPIC
            if (!Platforms.HasFlag(Platform.Epic))
                gameObject.SetActive(false);   
#elif STANDALONE_GOG
            if (!Platforms.HasFlag(Platform.Gog))
                gameObject.SetActive(false);  
#elif MICROSOFT_GAME_CORE
            if (!Platforms.HasFlag(Platform.WindowsStore))
                gameObject.SetActive(false); 
#elif UNITY_EDITOR
            if (!Platforms.HasFlag(Platform.Editor))
                gameObject.SetActive(false);
#elif UNITY_STANDALONE
             if (!Platforms.HasFlag(Platform.Standalone))
                gameObject.SetActive(false);
#endif
        }
    }
}

