using PetoonsStudio.PSEngine.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Lumin;

namespace PetoonsStudio.PSEngine.Multiplatform
{
    [System.Serializable]
    public class PlatformID
    {
        public Platform Platform;
        public string ID;
    }

    [CreateAssetMenu(fileName = "DLCXX", menuName = "Petoons Studio/PSEngine/Framework/DLC")]
    public class DownloadableContent : ScriptableObject
    {
        [Tooltip("Internal string to identify unique DLC.")]
        public string ID = "DLCXX";

        [Tooltip("Determines if has content in the downloadable package or not.")]
        public bool HasContent = true;

        [Tooltip("List of Platforms with his ID, notice that one platform can have only one ID.")]
        public List<PlatformID> PlatformIds = new List<PlatformID>();

        public string this[Platform platform]
        {
            get
            {
                var platformID = PlatformIds.Find(x => x.Platform.HasFlag(platform));
                return (platformID == null) ? string.Empty: platformID.ID;
            }
        }
    }
}
