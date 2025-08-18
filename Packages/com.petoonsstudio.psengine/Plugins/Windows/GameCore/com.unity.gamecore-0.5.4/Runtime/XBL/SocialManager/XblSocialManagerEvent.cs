using System;

namespace Unity.GameCore
{
    public class XblSocialManagerEvent
    {
        internal XblSocialManagerEvent(Interop.XblSocialManagerEvent interopEvent)
        {
            this.User = new XUserHandle(interopEvent.user);
            this.EventType = interopEvent.eventType;
            this.Hr = interopEvent.hr;
            this.LoadedGroup = new XblSocialManagerUserGroupHandle(interopEvent.loadedGroup);


            this.UsersAffected = Array.ConvertAll(
                interopEvent.GetUserArray(),
                u => new XblSocialManagerUser(u));
        }

        public XUserHandle User { get; }
        public XblSocialManagerEventType EventType { get; }
        public Int32 Hr { get; }
        public XblSocialManagerUserGroupHandle LoadedGroup { get; }
        public XblSocialManagerUser[] UsersAffected { get; }
    }
}
