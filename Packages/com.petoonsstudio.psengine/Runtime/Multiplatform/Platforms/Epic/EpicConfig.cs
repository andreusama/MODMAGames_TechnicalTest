using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Multiplatform
{
    [CreateAssetMenu(menuName = "Petoons Studio/PSEngine/Multiplatform/Epic/Configuration", fileName = "EpicConfiguration")]
    public class EpicConfig : PlatformBaseConfiguration
    {
#if UNITY_EDITOR
        [Tooltip("Only use this when making an epic game than is not intended to run with Epic Games Launcher (ex: Steam). Create an executable that is used as a transient launcher application to run the actual game client executable.")]
        public bool SetBootstrap = false;
#endif
    }

}
