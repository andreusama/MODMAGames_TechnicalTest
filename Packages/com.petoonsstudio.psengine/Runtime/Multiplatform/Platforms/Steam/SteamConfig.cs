using UnityEngine;
using PetoonsStudio.PSEngine.Framework;
using System;

#if STANDALONE_STEAM
using Steamworks;
using PetoonsStudio.PSEngine.Utils;
using PetoonsStudio.PSEngine.Multiplatform.Steam;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform
{
    [CreateAssetMenu(menuName = "Petoons Studio/PSEngine/Multiplatform/Steam/Configuration", fileName ="SteamConfiguration")]
    public class SteamConfig : PlatformBaseConfiguration
    {
#if STANDALONE_STEAM
        [Header("Application")]
        [Tooltip("Application ID from Dev portal Steamworks.")]
        public uint SteamId;

        [Header("Features")]
        [Tooltip("Define the behaviour when Steam overlay opens.")]
        [Header("Overlay")]
        public bool PauseOnOverlay;

        [ConditionalHide(nameof(PauseOnOverlay), false)]
        [ClassSelector(typeof(SteamOverlayManager), includeParent:true)]
        public string SteamOverlayManager;
#endif
    }
}