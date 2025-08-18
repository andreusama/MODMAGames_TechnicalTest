using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    internal struct XPackageFeature
    {
        internal readonly UTF8StringPtr id;
        internal readonly UTF8StringPtr displayName;
        internal readonly UTF8StringPtr tags;
        internal readonly NativeBool hidden;

        internal XPackageFeature(
          Unity.GameCore.XPackageFeature publicObject,
          DisposableCollection disposableCollection)
        {
            this.id = new UTF8StringPtr(publicObject.Id, disposableCollection);
            this.displayName = new UTF8StringPtr(publicObject.DisplayName, disposableCollection);
            this.tags = new UTF8StringPtr(publicObject.Tags, disposableCollection);
            this.hidden = new NativeBool(publicObject.Hidden);
        }
    }
}
