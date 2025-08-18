using PetoonsStudio.PSEngine.Utils;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Multiplatform.GameCore
{
    public class GameCoreConfig : PlatformBaseConfiguration
    {
        [Header("Application")]
        [Tooltip("Configure if the apllication uses Simple User Mode or Advance User mode")]
        public bool IsSimpleUserMode = true;

        [ClassSelector(typeof(IGamecoreControllerHandler))]
        public string GamecoreControllerHandler;
    }

}
