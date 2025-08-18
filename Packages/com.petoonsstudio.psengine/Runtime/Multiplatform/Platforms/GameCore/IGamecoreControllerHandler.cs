using UnityEngine;

namespace PetoonsStudio.PSEngine.Multiplatform.GameCore
{
    public interface IGamecoreControllerHandler
    {

#if UNITY_GAMECORE
        void GameCoreDeviceStateChanged(GXDKDeviceStateChange newDeviceState);
#endif
    }
}
