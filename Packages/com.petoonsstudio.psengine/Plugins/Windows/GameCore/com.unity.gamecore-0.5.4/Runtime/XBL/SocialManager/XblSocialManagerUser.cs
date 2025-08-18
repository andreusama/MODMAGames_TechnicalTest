using System;

namespace Unity.GameCore
{
    public class XblSocialManagerUser
    {
        internal XblSocialManagerUser(Interop.XblSocialManagerUser interopUser)
        {
            this.XboxUserId = interopUser.xboxUserId;
            this.IsFavorite = interopUser.isFavorite;
            this.IsFollowingUser = interopUser.isFollowingUser;
            this.IsFollowedByCaller = interopUser.isFollowedByCaller;
            this.DisplayName = Interop.Converters.ByteArrayToString(interopUser.displayName);
            this.RealName = Interop.Converters.ByteArrayToString(interopUser.realName);
            this.DisplayPicUrlRaw = Interop.Converters.ByteArrayToString(interopUser.displayPicUrlRaw);
            this.UseAvatar = interopUser.useAvatar;
            this.Gamerscore = Interop.Converters.ByteArrayToString(interopUser.gamerscore);
            this.Gamertag = Interop.Converters.ByteArrayToString(interopUser.gamertag);
            this.ModernGamertag = Interop.Converters.ByteArrayToString(interopUser.modernGamertag);
            this.ModernGamertagSuffix = Interop.Converters.ByteArrayToString(interopUser.modernGamertagSuffix);
            this.UniqueModernGamertag = Interop.Converters.ByteArrayToString(interopUser.uniqueModernGamertag);
            this.PresenceRecord = new XblSocialManagerPresenceRecord(interopUser.presenceRecord);
            this.TitleHistory = new XblTitleHistory(interopUser.titleHistory);
            this.PreferredColor = new XblPreferredColor(interopUser.preferredColor);
        }

        public UInt64 XboxUserId { get; }
        public bool IsFavorite { get; }
        public bool IsFollowingUser { get; }
        public bool IsFollowedByCaller { get; }
        public string DisplayName { get; }
        public string RealName { get; }
        public string DisplayPicUrlRaw { get; }
        public bool UseAvatar { get; }
        public string Gamerscore { get; }
        public string Gamertag { get; }
        public string ModernGamertag { get; }
        public string ModernGamertagSuffix { get; }
        public string UniqueModernGamertag { get; }

        public XblSocialManagerPresenceRecord PresenceRecord { get; }
        public XblTitleHistory TitleHistory { get; }
        public XblPreferredColor PreferredColor { get; }
    }
}
