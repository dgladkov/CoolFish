using System;

namespace CoolFishNS.RemoteNotification.Payloads
{
    [Serializable]
    public class LoggingPayload : AbstractPayload
    {
        public string ExceptionType;
        public string LogLevel;
        public string Message;
        public string StackTrace;
    }
}