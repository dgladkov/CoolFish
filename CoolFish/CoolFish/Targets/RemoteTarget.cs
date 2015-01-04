using System;
using CoolFishNS.RemoteNotification.Payloads;
using CoolFishNS.Utilities;
using NLog;
using NLog.Common;
using NLog.Targets;

namespace CoolFishNS.Targets
{
    public class RemoteTarget : TargetWithLayout
    {
        private readonly SQSManager manager = new SQSManager();

        protected override void Write(AsyncLogEventInfo[] logEvents)
        {
            foreach (AsyncLogEventInfo logEvent in logEvents)
            {
                Write(logEvent);
            }
        }

        protected override void Write(AsyncLogEventInfo logEvent)
        {
            Write(logEvent.LogEvent);
        }

        protected override void Write(LogEventInfo logEvent)
        {
            if ((logEvent.Level == LogLevel.Error || logEvent.Level == LogLevel.Fatal) && logEvent.Exception != null)
            {
                SendPayload(logEvent.Level, logEvent.FormattedMessage, logEvent.Exception);
            }
        }

        private void SendPayload(LogLevel level, string message, Exception exception)
        {
            manager.SendLoggingPayload(new LoggingPayload
            {
                StackTrace = exception.StackTrace,
                Message = message,
                LogLevel = level.Name,
                ExceptionType = exception.GetType().Name
            });
        }
    }
}