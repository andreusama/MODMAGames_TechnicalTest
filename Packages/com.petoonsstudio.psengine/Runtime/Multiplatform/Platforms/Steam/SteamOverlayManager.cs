using UnityEngine;
using PetoonsStudio.PSEngine.Framework;

#if STANDALONE_STEAM
using Steamworks;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform.Steam
{
    public class SteamOverlayManager : MonoBehaviour
    {
#if STANDALONE_STEAM

        protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;
		protected float m_InitialTimeScale;

		protected virtual void OnEnable()
		{
            if (PlatformManager.Instance.SteamConfig.PauseOnOverlay)
            {
				m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
			}
		}

		/// <summary>
		/// Pause game when steam overlay opened
		/// </summary>
		/// <param name="pCallback"></param>
		protected virtual void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
		{
			if (pCallback.m_bActive != 0)
			{
				m_InitialTimeScale = Time.timeScale;
				Time.timeScale = 0f;
			}
			else
			{
				Time.timeScale = m_InitialTimeScale;
			}
		}
#endif
	}
}
