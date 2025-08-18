using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public class ObjectReplacerResolverData
    {
        public const string REPLACEMENT_NAME = "Replacement";
        public UnityEngine.Object Replacement = null;
    }

    public class ObjectReplacerDistanceResolverData : ObjectReplacerResolverData
    {
        public const string DISTANCE_NAME = "Distance";
        public float Distance = 0;
    }
}
