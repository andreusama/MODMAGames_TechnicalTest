using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

[assembly: AlwaysLinkAssembly]

namespace UnityEngine.InputSystem.XInput
{
    /// <summary>
    /// Adds support for XboxOne controllers.
    /// </summary>
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif 
#if UNITY_DISABLE_DEFAULT_INPUT_PLUGIN_INITIALIZATION
    public
#else
    internal
#endif
    static class XboxOneSupport
    {
        static XboxOneSupport()
        {
            // Base layout for Xbox-style gamepad.
            // InputSystem.RegisterLayout<XInputController>();

#if UNITY_EDITOR || UNITY_XBOXONE
            InputSystem.RegisterLayout<XboxOneGamepad>(
                matches: new InputDeviceMatcher()
                    .WithDeviceClass("XboxOneGamepad")
                    .WithInterface("Xbox"));
#endif

        }

        [RuntimeInitializeOnLoadMethod]
        private static void InitializeInPlayer() { }
    }
}
