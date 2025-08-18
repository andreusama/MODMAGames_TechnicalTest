using UnityEngine;

#if UNITY_PS5
using PlatformInput = UnityEngine.PS5.PS5Input;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform.PS5
{
    public class GamePad : MonoBehaviour
    {
#if UNITY_PS5
        public static GamePad activeGamePad = null;
        public int playerId = -1;

        private static bool enableInput = true;
        private static float timeout = 0;

        public static void EnableInput(bool enable)
        {
            if (enable != enableInput)
            {
                enableInput = enable;

                if (enable == true)
                {
                    timeout = 1.0f;
                }
            }
        }

        public static bool IsInputEnabled { get { return enableInput; } }

        public bool IsConnected
        {
#if UNITY_PS4 || UNITY_PS5
            get { return PlatformInput.PadIsConnected(playerId); }
#else
            get { return false; }
#endif
        }

        private int stickID;
        private bool hasSetupGamepad = false;

#if UNITY_PS5
        public PlatformInput.LoggedInUser loggedInUser;
#endif
        void Awake()
        {
            // Stick ID is the player ID + 1
            stickID = playerId + 1;

            ToggleGamePad(false);
        }

        void Update()
        {
            if (timeout > 0.0f)
            {
                timeout -= Time.deltaTime;
            }

#if UNITY_PS5
            if (PlatformInput.PadIsConnected(playerId))
            {
                // Set the gamepad to the start values for the player
                if (!hasSetupGamepad)
                {
                    ToggleGamePad(true);
                }
                else
                {
#if UNITY_PS5
                    loggedInUser = PlatformInput.RefreshUsersDetails(playerId);
#endif
                }

                if (activeGamePad == null)
                {
                    activeGamePad = this;
                }
            }
            else if (hasSetupGamepad)
            {
                ToggleGamePad(false);
            }
#endif
        }

        // Toggle the gamepad between connected and disconnected states
        void ToggleGamePad(bool active)
        {
            if (active)
            {
                // Set 3D Text to whoever's using the pad
#if UNITY_PS5
                loggedInUser = PlatformInput.RefreshUsersDetails(playerId);
#endif

                hasSetupGamepad = true;
            }
            else
            {
                hasSetupGamepad = false;
            }
        } 
#endif
    }
}