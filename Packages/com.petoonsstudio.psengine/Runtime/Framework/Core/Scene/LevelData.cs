using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

namespace PetoonsStudio.PSEngine.Framework
{
    [CreateAssetMenu(fileName = "new Level Data", menuName = "Petoons Studio/PSEngine/Framework/Level Data")]
    public class LevelData : ScriptableObject
    {
        public string ID;
        public LocalizedString LevelName;
        public AssetReferenceT<SceneGroup> Scene;
    }
}
