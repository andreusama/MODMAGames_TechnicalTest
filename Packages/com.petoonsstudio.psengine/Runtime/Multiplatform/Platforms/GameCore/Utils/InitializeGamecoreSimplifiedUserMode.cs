using UnityEngine;

namespace PetoonsStudio.PSEngine.Multiplatform.GameCore
{
    public class InitializeGamecoreSimplifiedUserMode : MonoBehaviour
    {
#if UNITY_GAMECORE
        private async void Start()
        {
            await GameCoreManager.InitializeSimplifiedUserMode();
        }
#endif
    }
}

