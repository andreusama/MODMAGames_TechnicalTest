using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Framework
{
    [System.Serializable]
    public class SerializedGameMode<T> where T : IGameMode
    {
        public string ApplicationVersion;
        public DateTime SaveDate;

        public SerializedGameMode()
        {
            ApplicationVersion = string.Empty;
            SaveDate = new DateTime();
        }
    }

    [System.Serializable]
    public class SerializedGameModePreview<T> where T : IGameMode
    {

    }
}