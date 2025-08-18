using CrossSceneReference;
using PetoonsStudio.PSEngine.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Framework
{
    [System.Serializable]
    public class PersistenceParams
    {
        public bool ShouldPersist;
        public bool IsCustomGuid = false;
        [ConditionalHide("IsCustomGuid")] public string CustomGuid;
    }
}
