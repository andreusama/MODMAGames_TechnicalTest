using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblPermissionCheckResult
    {
        internal XblPermissionCheckResult(Interop.XblPermissionCheckResult interopStruct)
        {
            this.IsAllowed = interopStruct.isAllowed.Value;
            this.TargetXuid = interopStruct.targetXuid;
            this.TargetUserType = interopStruct.targetUserType;
            this.PermissionRequested = interopStruct.permissionRequested;
            this.Reasons = interopStruct.GetReasons(x => new XblPermissionDenyReasonDetails(x));
        }

        public bool IsAllowed { get; }
        public UInt64 TargetXuid { get; }
        public XblAnonymousUserType TargetUserType { get; }
        public XblPermission PermissionRequested { get; }
        public XblPermissionDenyReasonDetails[] Reasons { get; }
    }
}