using PetoonsStudio.PSEngine.Utils;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Multiplatform.PS5
{
    [CreateAssetMenu(menuName = "Petoons Studio/PSEngine/Multiplatform/PS5/Configuration", fileName = "PS5Configuration")]
    public class PS5Config : PlatformBaseConfiguration
    {
#if UNITY_PS5
        [Header("Activity cards")]
        [Tooltip("In order to use Activities in PS5, provide an implementation for the project.")]
        [ClassSelector(typeof(PS5ActivityHandler))]
        public string PS5ActivityHandler;
#endif
    }
}
