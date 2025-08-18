using PetoonsStudio.PSEngine.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Multiplatform
{
    [DefaultExecutionOrder(-1)]
    public class PlatformComponentsEnabler : MonoBehaviour
    {
        [Tooltip("This monobehaviours will be disabled before executing Awake")]
        public List<MonoBehaviour> Components;
        [Header("Enabled on selected platforms")]
        public Platform Platforms;

        /// <summary>
        /// Awake
        /// </summary>
        void Awake()
        {
#if UNITY_SWITCH
            EnableComponents(Platforms.HasFlag(Platform.Switch));
#elif UNITY_PS4
            EnableComponents(Platforms.HasFlag(Platform.Ps4));
#elif UNITY_PS5
            EnableComponents(Platforms.HasFlag(Platform.Ps5));
#elif UNITY_GAMECORE_XBOXONE
            EnableComponents(Platforms.HasFlag(Platform.XboxOne));
#elif UNITY_GAMECORE_XBOXSERIES
            EnableComponents(Platforms.HasFlag(Platform.XboxSeries));
#elif STANDALONE_STEAM
            EnableComponents(Platforms.HasFlag(Platform.Steam));  
#elif STANDALONE_GOG
            EnableComponents(Platforms.HasFlag(Platform.Gog));  
#elif STANDALONE_EPIC
            EnableComponents(Platforms.HasFlag(Platform.Epic));  
#elif MICROSOFT_GAME_CORE
            EnableComponents(Platforms.HasFlag(Platform.WindowsStore));  
#elif UNITY_EDITOR
            EnableComponents(Platforms.HasFlag(Platform.Editor));
#elif UNITY_STANDALONE
            EnableComponents(Platforms.HasFlag(Platform.Standalone));
#endif
        }

        private void EnableComponents(bool active)
        {
            foreach (var comp in Components)
                comp.enabled = active;
        }
    }
}

