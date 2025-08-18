using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

[assembly: AlwaysLinkAssembly]

namespace UnityEngine.InputSystem.XInput
{
    /// <summary>
    /// Adds support for GXDK controllers.
    /// </summary>
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif 
#if UNITY_DISABLE_DEFAULT_INPUT_PLUGIN_INITIALIZATION
    public
#else
    internal
#endif
    static class GXDKSupport
    {
        static GXDKSupport()
        {
            // Base layout for GXDK gamepad.
            // InputSystem.RegisterLayout<XInputController>();

#if UNITY_EDITOR || UNITY_GAMECORE
            InputSystem.RegisterLayout<GXDKGamepad>(
                matches: new InputDeviceMatcher()
                    .WithDeviceClass("GXDKGamepad")
                    .WithInterface("GXDK"));
#endif

        }

        [RuntimeInitializeOnLoadMethod]
        private static void InitializeInPlayer() { }
    }
}
