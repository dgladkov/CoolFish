using System.Collections.Generic;
using System.Threading.Tasks;
using CoolFishNS.RemoteNotification.Payloads;
using CoolFishNS.Utilities;

namespace CoolFishNS.RemoteNotification.Analytics
{
    public class AnalyticsManager
    {
        private readonly SQSManager manager;

        public AnalyticsManager()
        {
            manager = new SQSManager();
        }

        public void SendAnalyticsEvent(double eventDurationMS, string eventType, Dictionary<string, string> data = null)
        {
            Task.Run(() =>
            {
                manager.SendAnalyticsPayload(new AnalyticsPayload
                {
                    EventDuration = eventDurationMS,
                    EventType = eventType,
                    Data = data,
                });
            });
        }
    }
}