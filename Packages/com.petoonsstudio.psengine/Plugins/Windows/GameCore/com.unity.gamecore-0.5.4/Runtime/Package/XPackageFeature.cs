using System;

namespace Unity.GameCore
{
    public class XPackageFeature
    {
        internal XPackageFeature(Interop.XPackageFeature interopStruct)
        {
            this.Id = interopStruct.id.GetString();
            this.DisplayName = interopStruct.displayName.GetString();
            this.Tags = interopStruct.tags.GetString();
            this.Hidden = interopStruct.hidden.Value;
        }

        public string Id { get; }

        public string DisplayName { get; }

        public string Tags { get; }

        public bool Hidden { get; }
    }
}
