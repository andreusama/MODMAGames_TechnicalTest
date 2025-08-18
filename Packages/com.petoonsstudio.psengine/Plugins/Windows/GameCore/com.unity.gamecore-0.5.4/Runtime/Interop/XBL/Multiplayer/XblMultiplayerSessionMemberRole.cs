﻿using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef struct XblMultiplayerSessionMemberRole
    //{
    //    const char* roleTypeName;
    //    const char* roleName;
    //}
    //XblMultiplayerSessionMemberRole;
    [StructLayout(LayoutKind.Sequential)]
    internal struct XblMultiplayerSessionMemberRole
    {
        internal UTF8StringPtr roleTypeName;
        internal UTF8StringPtr roleName;

        internal XblMultiplayerSessionMemberRole(GameCore.XblMultiplayerSessionMemberRole publicObject, DisposableCollection disposableCollection)
        {
            roleTypeName = new UTF8StringPtr( publicObject.RoleTypeName, disposableCollection );
            roleName = new UTF8StringPtr(publicObject.RoleName, disposableCollection);
        }
    }
}
