using System;

namespace Unity.GameCore
{
    public class XblMultiplayerSessionMemberRole
    {
        internal XblMultiplayerSessionMemberRole(Interop.XblMultiplayerSessionMemberRole interopHandle)
        {
            this.InteropHandle = interopHandle;

            this.RoleTypeName = interopHandle.roleTypeName.GetString();
            this.RoleName = interopHandle.roleName.GetString();
        }

        public string RoleTypeName { get; }
        public string RoleName { get; }

        internal Interop.XblMultiplayerSessionMemberRole InteropHandle { get; }
    }
}
