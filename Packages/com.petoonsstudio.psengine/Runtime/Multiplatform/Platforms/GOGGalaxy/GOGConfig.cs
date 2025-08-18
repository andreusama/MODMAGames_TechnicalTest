using UnityEngine;

namespace PetoonsStudio.PSEngine.Multiplatform
{
    [CreateAssetMenu(menuName = "Petoons Studio/PSEngine/Multiplatform/GOG/Configuration", fileName = "GOGConfiguration")]
    public class GOGConfig : PlatformBaseConfiguration
    {
        [Header("Application")]
        [Tooltip("Client ID, unique for each application, get it at GOG Dev portal.")]
        public string ClientID;
        [Tooltip("Client secret, unique for each application, get it at GOG Dev portal.")]
        public string ClientSecret;
    }
}
