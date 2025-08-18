using ParadoxNotion.Design;
using System;

namespace PetoonsStudio.PSEngine.Utils
{
    public class SelectAssetNodeCanvasAttribute : DrawerAttribute
    {
        public Type AssetType;
        public string ParameterName;

        public SelectAssetNodeCanvasAttribute(Type assetType, string parameterName)
        {
            AssetType = assetType;
            ParameterName = parameterName;
        }
    }
}
