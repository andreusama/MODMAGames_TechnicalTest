using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblUserProfile
    {
        internal XblUserProfile(Interop.XblUserProfile interopStruct)
        {
            this.XboxUserId = interopStruct.xboxUserId;
            this.AppDisplayName = Converters.ByteArrayToString(interopStruct.appDisplayName);
            this.AppDisplayPictureResizeUri = Converters.ByteArrayToString(interopStruct.appDisplayPictureResizeUri);
            this.GameDisplayName = Converters.ByteArrayToString(interopStruct.gameDisplayName);
            this.GameDisplayPictureResizeUri = Converters.ByteArrayToString(interopStruct.gameDisplayPictureResizeUri);
            this.Gamerscore = Converters.ByteArrayToString(interopStruct.gamerscore);
            this.Gamertag = Converters.ByteArrayToString(interopStruct.gamertag);
            this.ModernGamertag = Converters.ByteArrayToString(interopStruct.modernGamertag);
            this.ModernGamertagSuffix = Converters.ByteArrayToString(interopStruct.modernGamertagSuffix);
            this.UniqueModernGamertag = Converters.ByteArrayToString(interopStruct.uniqueModernGamertag);
        }

        public UInt64 XboxUserId { get; }
        public string AppDisplayName { get; }
        public string AppDisplayPictureResizeUri { get; }
        public string GameDisplayName { get; }
        public string GameDisplayPictureResizeUri { get; }
        public string Gamerscore { get; }
        public string Gamertag { get; }
        public string ModernGamertag { get; }
        public string ModernGamertagSuffix { get; }
        public string UniqueModernGamertag { get; }
    }
}
