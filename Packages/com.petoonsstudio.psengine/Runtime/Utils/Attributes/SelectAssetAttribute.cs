using System;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public class SelectAssetAttribute : PropertyAttribute
    {
        public Type AssetType;
        public string ParameterName;

        public SelectAssetAttribute(Type assetType, string parameterName)
        {
            AssetType = assetType;
            ParameterName = parameterName;
        }
    }
}