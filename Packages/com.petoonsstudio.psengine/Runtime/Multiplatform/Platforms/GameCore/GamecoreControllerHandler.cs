using UnityEngine;
#if UNITY_GAMECORE
using Unity.GameCore;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform.GameCore
{
    public class GamecoreControllerHandler : MonoBehaviour, IGamecoreControllerHandler
    {
#if UNITY_GAMECORE
        private void OnEnable()
        {
            GameCoreManager.Instance.OnDeviceStateChanged += GameCoreDeviceStateChanged;
        }

        private void OnDisable()
        {
            GameCoreManager.Instance.OnDeviceStateChanged -= GameCoreDeviceStateChanged;
        }

		public virtual void GameCoreDeviceStateChanged(GXDKDeviceStateChange newDeviceState)
		{
            if (GXDKInput.GetNumActiveGamepads() > 0) return;

            Debug.Log($"[DEVICE STATE CHANGED] Showing picker UI");
            SDK.XUserFindControllerForUserWithUiAsync(GameCoreManager.Instance.XboxUser.XboxUserHandle, FindControllerForUserCompleted);
        }

		public virtual void FindControllerForUserCompleted(int result, GXDKAppLocalDeviceId devideId){}
#endif
	}
}