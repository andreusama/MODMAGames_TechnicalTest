using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblPermissionDenyReasonDetails
    {
        internal XblPermissionDenyReasonDetails(Interop.XblPermissionDenyReasonDetails interopStruct)
        {
            this.Reason = interopStruct.reason;
            this.RestrictedPrivilege = interopStruct.restrictedPrivilege;
            this.RestrictedPrivacySetting = interopStruct.restrictedPrivacySetting;
        }

        public XblPermissionDenyReason Reason { get; }
        public XblPrivilege RestrictedPrivilege { get; }
        public XblPrivacySetting RestrictedPrivacySetting { get; }
    }
}