using System;
using System.Collections.Generic;

namespace CoolFishNS.RemoteNotification.Payloads
{
    [Serializable]
    public class AnalyticsPayload : AbstractPayload
    {
        public Dictionary<string, string> Data;
        public double EventDuration;
        public string EventType;
    }
}