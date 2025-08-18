using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblMultiplayerSessionChangeEventArgs
    {
        internal XblMultiplayerSessionChangeEventArgs(Interop.XblMultiplayerSessionChangeEventArgs interopStruct)
        {
            this.SessionReference = new XblMultiplayerSessionReference(interopStruct.SessionReference);
            this.Branch = interopStruct.GetBranch();
            this.ChangeNumber = interopStruct.ChangeNumber;
        }

        public XblMultiplayerSessionReference SessionReference { get; }
        public string Branch { get; }
        public ulong ChangeNumber { get; }
    }
}
