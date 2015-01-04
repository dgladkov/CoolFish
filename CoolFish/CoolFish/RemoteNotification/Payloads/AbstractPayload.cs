using System;
using CoolFishNS.RemoteNotification.Analytics;
using CoolFishNS.Utilities;

namespace CoolFishNS.RemoteNotification.Payloads
{
    [Serializable]
    public abstract class AbstractPayload
    {
        public string BotVersion;
        public string ExternalIP;
        public string PayloadType;
        public string UserId;

        protected AbstractPayload()
        {
            PayloadType = GetType().Name;
            ExternalIP = UserIdentity.Instance.ExternalIp;
            UserId = UserIdentity.Instance.UserId;
            BotVersion = Constants.Version.Value;
        }
    }
}