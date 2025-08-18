using System;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public class SelectInterfaceAssetAttribute : PropertyAttribute
    {
        public Type AssetType;
        public Type AssetFilter;
        public string ParameterName;

        public SelectInterfaceAssetAttribute(Type assetType, Type filterType, string parameterName)
        {
            AssetType = assetType;
            AssetFilter = filterType;
            ParameterName = parameterName;
        }
    }
}
