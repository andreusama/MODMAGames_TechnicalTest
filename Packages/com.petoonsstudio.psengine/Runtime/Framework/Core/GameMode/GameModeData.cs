using PetoonsStudio.PSEngine.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PetoonsStudio.PSEngine.Framework
{
    [CreateAssetMenu(fileName = "New GameMode Data", menuName = "Petoons Studio/PSEngine/Config/GameMode Data")]
    public class GameModeData : ScriptableObject
    {
        public int SlotNumber = 1;

        [Header("Scenes")]
        public AssetReferenceT<SceneGroup> InitialScene;

        [Header("Save Data")]
        public string SaveFolderName = "/SaveData";
        public string SaveFileExtension = ".dat";
        public string SaveFileName = "BARSaveData";
    }
}