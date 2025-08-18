using UnityEngine;

#if UNITY_GAMECORE
using Unity.GameCore;
using UnityEngine.GameCore;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform
{
    public static class GameCoreUtilities
    {
#if UNITY_GAMECORE
        public enum GameCoreNetworkState
        {
            Online,
            Offline
        }

        public const int GAMESAVE_INVALID_INDEX = -1;

        /// <summary>
        /// Check if current hardware is Xbox One/ Xbox One S / Xbox One X / Xbox One X Dev kit
        /// </summary>
        /// <returns></returns>
        public static bool IsXboxOne()
        {
            switch (Hardware.version)
            {
                case HardwareVersion.XboxOne:
                case HardwareVersion.XboxOneS:
                case HardwareVersion.XboxOneX:
                case HardwareVersion.XboxOneXDevkit:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Check if current hardware is Xbox Series S/ Xbox Series X / Xbox Scarlett Dev kit
        /// </summary>
        /// <returns></returns>
        public static bool IsXboxSeries()
        {
            switch (Hardware.version)
            {
                case HardwareVersion.XboxSeriesS:
                case HardwareVersion.XboxSeriesX:
                case HardwareVersion.XboxScarlettDevkit:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Check if the concole where it's running is from Scarlet or greater generation.
        /// </summary>
        /// <returns></returns>
        public static bool IsScarletOrGreaterGeneration()
        {
            return (Hardware.version > HardwareVersion.XboxOneXDevkit) ? true : false;
        }
       
        /// <summary>
        /// Check if Xbox has no connection.
        /// As stated in the documentation, a better approach should be pinging and checking the result.
        /// </summary>
        /// <returns>Returns the Online State of the console</returns>
        public static GameCoreNetworkState GetOnlineState()
        {
            int hResult = GameCoreOperationResults.Invalid;
            hResult = Networking.GetConnectivityHint(out var networkConnection);
            if (!HR.SUCCEEDED(hResult))
            {
                Debug.Log($"[ONLINE] Error on getting Conectivity Hint:{GameCoreOperationResults.GetName(hResult)}");
                return GameCoreNetworkState.Offline;
            }
            return networkConnection.connectivityLevel == NetworkingConnectivityLevelHint.None ? GameCoreNetworkState.Offline : GameCoreNetworkState.Online;
        }

        /// <summary>
        /// Check if Index of Game Save is valid
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool IndexIsValid(int index) => (index > GAMESAVE_INVALID_INDEX);
#endif
    }
}

