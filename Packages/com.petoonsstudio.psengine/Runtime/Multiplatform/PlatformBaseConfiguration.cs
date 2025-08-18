using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Utils;
using System;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Multiplatform
{
    public class PlatformBaseConfiguration : ScriptableObject
    {
        [Header("Internal services")]
        [Tooltip("Active and set a custom IStorage implementation to overide platform IStorage service.")]
        public bool OverrideStorage = false;

        [Tooltip("Provide a class that implements IStorage for the platform.")]
        [ConditionalHide(nameof(OverrideStorage), false)]
        [ClassSelector(typeof(IStorage))]
        public string StorageService;
    }

}
