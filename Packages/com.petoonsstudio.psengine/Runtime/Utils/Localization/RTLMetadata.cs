using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Metadata;

namespace PetoonsStudio.PSEngine.Utils
{
    [Metadata(AllowedTypes = MetadataType.Locale)] // Hint to the editor to only show this type for a Locale
    [Serializable]
    public class RTLMetadata : IMetadata
    {
        public bool IsRTL;
    }
}